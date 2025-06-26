using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class PlayerController : NetworkBehaviour
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

    private NetworkCharacterController _ncc;
    private CharacterController characterController;
    public CameraShake cameraShake { get; private set; }
    public WeaponController weaponController { get; private set; }
    public InputManager input { get; private set; }
    private PlayerInteraction playerInteraction;

    public float Health => currentHealth;
    public float Hungry => currentHungry;
    public float SprintSpeed => sprintSpeed;

    #region 네트워크 콜백 함수
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            playerCamera.tag = "MainCamera";
            playerCamera.enabled = true;

            cameraShake = Camera.main?.GetComponent<CameraShake>();
            if (cameraShake != null) 
                cameraShake.SetCrouchAndStandHeight(cameraCrouchHeight, cameraStandHeight);
            
            ui = FindObjectOfType<GameScene_PlayerUI>();
            if (ui == null)
            {
                Debug.LogError("[PlayerController] GameScene_PlayerUI를 찾을 수 없습니다!");
                return;
            }

            
            ui.gameObject.SetActive(true);
            ui.Initialize(maxHealth, 100f, this);
        }
        else
        {
            playerCamera.enabled = false;
            if (ui != null) ui.gameObject.SetActive(false);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (!GetInput(out NetworkInputData inputData)) return;

        // 이동 및 논리 상태
        float moveSpeed = inputData.IsSprinting ? sprintSpeed : (inputData.IsCrouching ? crouchSpeed : walkSpeed);
        Vector3 move = new Vector3(inputData.MovementInput.x, 0, inputData.MovementInput.y);
        _ncc.Move(move.normalized, moveSpeed);

        // 회전
        float mouseX = inputData.MouseX * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        // 점프
        if (inputData.IsJumping && _ncc.Grounded)
            _ncc.Jump();

        // 논리상 캡슐 높이
        characterController.height = inputData.IsCrouching ? crouchHeight : standHeight;

        // (공격/에임/재장전 등 기타 논리 처리)
        if (inputData.IsAttacking) Attack();
        if (inputData.IsAiming)   AimStart(); else AimEnd();
        if (inputData.IsReloading) Reload();
        if (inputData.IsInteracting) Interaction();
        if (inputData.QuickSlotIndex >= 0) SelectItemSlot(inputData.QuickSlotIndex);
    }

    #endregion

    #region 유니티 생명주기 함수
    private void Awake()
    {
        input = GetComponent<InputManager>();
        playerInteraction = GetComponent<PlayerInteraction>();
        weaponController = GetComponent<WeaponController>();
        _ncc = GetComponent<NetworkCharacterController>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        var buffer = input.inputBuffer;

        // 커맨드 바인딩 (모두 PlayerInputBuffer에만 값 변경!)
        // 키 다운
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

        // 키 업
        input.BindKeyUpCommand(KeyCode.Mouse1, new AimEndCommand(buffer));
        input.BindKeyUpCommand(KeyCode.W, new StopMoveCommand(buffer));
        input.BindKeyUpCommand(KeyCode.A, new StopMoveCommand(buffer));
        input.BindKeyUpCommand(KeyCode.S, new StopMoveCommand(buffer));
        input.BindKeyUpCommand(KeyCode.D, new StopMoveCommand(buffer));

        // 키 홀드
        input.BindKeyHoldCommand(KeyCode.A, new MoveLeftCommand(buffer));
        input.BindKeyHoldCommand(KeyCode.D, new MoveRightCommand(buffer));
        input.BindKeyHoldCommand(KeyCode.W, new MoveForwardCommand(buffer));
        input.BindKeyHoldCommand(KeyCode.S, new MoveBackCommand(buffer));
        input.BindKeyHoldCommand(KeyCode.LeftShift, new SprintStartCommand(buffer));
        input.BindKeyHoldCommand(KeyCode.LeftControl, new CrouchStartCommand(buffer));
    }

    private void Update()
    {
      if (!Object.HasInputAuthority) return;

        if (ui != null)
        {
            ui.SetHealth(currentHealth);
            ui.SetHunger(currentHungry);
        }

        HandleMouseLook();
        SyncCameraShake();
    }

    #endregion

    #region 움직임 제어
    void HandleMouseLook()
    {
        // 카메라 피치(xRotation)는 InputAuthority에서만 로컬 처리!
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -65f, 70f);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    void SyncCameraShake()
    {
        if (cameraShake == null) return;
        cameraShake.SetCrouchAndSprint( input.inputBuffer.IsCrouching, input.inputBuffer.IsSprinting);
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
        playerInteraction.Interaction();
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
