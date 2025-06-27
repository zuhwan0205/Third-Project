using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 스탯")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float standHeight = 2.0f;

    [Header("카메라 / 민감도")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("카메라 위치 (연출)")]
    [SerializeField] private float cameraCrouchHeight = 1.0f;
    [SerializeField] private float cameraStandHeight = 1.8f;

    [Header("UI")]
    [SerializeField] private Text healthText;
    [SerializeField] private GameScene_PlayerUI ui;

    private float currentHealth;
    private float currentHungry;
    private float xRotation = 0f;

    private CharacterController characterController;
    public CameraShake cameraShake { get; private set; }
    public WeaponController weaponController { get; private set; }
    public InputManager input { get; private set; }
    private PlayerInteraction playerInteraction;

    // 움직임 관련 변수들
    private Vector3 velocity;
    private bool isGrounded;
    private float gravity = -9.81f;
    private float groundDistance = 0.4f;
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private LayerMask groundMask;

    public float Health => currentHealth;
    public float Hungry => currentHungry;
    public float SprintSpeed => sprintSpeed;

    #region 유니티 생명주기 함수
    private void Awake()
    {
        input = GetComponent<InputManager>();
        playerInteraction = GetComponent<PlayerInteraction>();
        weaponController = GetComponent<WeaponController>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        InitializePlayer();
        SetupInputBindings();
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        UpdateUI();
        SyncCameraShake();
    }

    #endregion

    #region 초기화
    private void InitializePlayer()
    {
        currentHealth = maxHealth;
        currentHungry = 100f;

        // 카메라 설정
        if (playerCamera != null)
        {
            playerCamera.tag = "MainCamera";
            playerCamera.enabled = true;
        }

        // 카메라 셰이크 설정
        cameraShake = Camera.main?.GetComponent<CameraShake>();
        if (cameraShake != null) 
            cameraShake.SetCrouchAndStandHeight(cameraCrouchHeight, cameraStandHeight);
        
        // UI 설정
        if (ui == null)
            ui = FindObjectOfType<GameScene_PlayerUI>();
            
        if (ui == null)
        {
            Debug.LogError("[PlayerController] GameScene_PlayerUI를 찾을 수 없습니다!");
            return;
        }

        ui.gameObject.SetActive(true);
        ui.Initialize(maxHealth, 100f, this);

        // Ground Check Transform이 없으면 생성
        if (groundCheckTransform == null)
        {
            GameObject groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(transform);
            groundCheck.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheckTransform = groundCheck.transform;
        }
    }

    private void SetupInputBindings()
    {
        var buffer = input.inputBuffer;

        // 키 다운 커맨드
        input.BindKeyDownCommand(KeyCode.Space, new JumpCommand(buffer));
        input.BindKeyDownCommand(KeyCode.Mouse0, new AttackCommand(buffer));
        input.BindKeyDownCommand(KeyCode.Mouse1, new AimStartCommand(buffer));
        input.BindKeyDownCommand(KeyCode.R, new ReloadCommand(buffer));
        input.BindKeyDownCommand(KeyCode.E, new InteractionCommand(buffer));
        input.BindKeyDownCommand(KeyCode.Alpha1, new AxeQuickSlotCommand(buffer));
        input.BindKeyDownCommand(KeyCode.Alpha2, new ShortSwordQuickSlotCommand(buffer));
        input.BindKeyDownCommand(KeyCode.Alpha3, new PistolQuickSlotCommand(buffer));
        input.BindKeyDownCommand(KeyCode.Alpha4, new ShotgunQuickSlotCommand(buffer));
        input.BindKeyDownCommand(KeyCode.Alpha5, new BowQuickSlotCommand(buffer));

        // 키 업 커맨드
        input.BindKeyUpCommand(KeyCode.Mouse1, new AimEndCommand(buffer));
        input.BindKeyUpCommand(KeyCode.W, new StopMoveCommand(buffer));
        input.BindKeyUpCommand(KeyCode.A, new StopMoveCommand(buffer));
        input.BindKeyUpCommand(KeyCode.S, new StopMoveCommand(buffer));
        input.BindKeyUpCommand(KeyCode.D, new StopMoveCommand(buffer));

        // 키 홀드 커맨드
        input.BindKeyHoldCommand(KeyCode.A, new MoveLeftCommand(buffer));
        input.BindKeyHoldCommand(KeyCode.D, new MoveRightCommand(buffer));
        input.BindKeyHoldCommand(KeyCode.W, new MoveForwardCommand(buffer));
        input.BindKeyHoldCommand(KeyCode.S, new MoveBackCommand(buffer));
        input.BindKeyHoldCommand(KeyCode.LeftShift, new SprintStartCommand(buffer));
        input.BindKeyHoldCommand(KeyCode.LeftControl, new CrouchStartCommand(buffer));
    }
    #endregion

    #region 움직임 제어
    private void HandleMovement()
    {
        // 땅 체크
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 입력 버퍼에서 움직임 데이터 가져오기
        var inputBuffer = input.inputBuffer;
        
        // 이동
        float moveSpeed = inputBuffer.IsSprinting ? sprintSpeed : (inputBuffer.IsCrouching ? crouchSpeed : walkSpeed);
        Vector3 move = new Vector3(inputBuffer.MovementInput.x, 0, inputBuffer.MovementInput.y);
        move = transform.TransformDirection(move);
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // 점프
        if (inputBuffer.IsJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(2f * -gravity);
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // 마우스 X 회전 (Yaw)
        float mouseX = inputBuffer.MouseX * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        // 캐릭터 높이 조절
        characterController.height = inputBuffer.IsCrouching ? crouchHeight : standHeight;

        // 기타 액션 처리
        if (inputBuffer.IsAttacking) Attack();
        if (inputBuffer.IsAiming) AimStart(); else AimEnd();
        if (inputBuffer.IsReloading) Reload();
        if (inputBuffer.IsInteracting) Interaction();
        if (inputBuffer.QuickSlotIndex >= 0) SelectItemSlot(inputBuffer.QuickSlotIndex);
    }

    private void HandleMouseLook()
    {
        // 카메라 피치(xRotation) 처리
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -65f, 70f);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    private void SyncCameraShake()
    {
        if (cameraShake == null) return;
        cameraShake.SetCrouchAndSprint(input.inputBuffer.IsCrouching, input.inputBuffer.IsSprinting);
    }

    private void UpdateUI()
    {
        if (ui != null)
        {
            ui.SetHealth(currentHealth);
            ui.SetHunger(currentHungry);
        }
    }
    #endregion

    #region 공격 / 에임 / 재장전
    public void Attack() => weaponController?.Attack();
    public void AimStart() => weaponController?.Aim();
    public void AimEnd() => weaponController?.AimCancel();
    public void Reload() => weaponController?.Reload();
    #endregion

    #region 체력/힐
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);
        if (currentHealth <= 0f)
            Debug.Log("게임 오버!");
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
    #endregion

    #region 상호작용
    public void Interaction()
    {
        playerInteraction?.Interaction();
    }
    #endregion

    #region 아이템 슬롯
    public void SelectItemSlot(int slotIndex)
    {
        if (weaponController.ownedWeapons[slotIndex] == false) return;
        weaponController.EquipWeaponByIndex(slotIndex);
    }
    #endregion
}