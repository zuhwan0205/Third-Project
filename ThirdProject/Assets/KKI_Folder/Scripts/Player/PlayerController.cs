using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("플레이어 스탯")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float groundCheckDistance = 3;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float standHeight = 2.0f;
    [SerializeField] private float crouchSpeed = 2.5f;

    [Header("카메라 / 민감도")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 2f;
    
    [SerializeField] private float cameraCrouchHeight;
    [SerializeField] private float cameraStandHeight;
    [Header("UI")]
    [SerializeField] private Text healthText;


    // 캐릭터 스탯
    private float moveSpeed;
    private bool isSprinting = false;
    private bool isCrouching = false;

    // 카메라 회전
    private float xRotation = 0f;
    private Vector3 velocity;

    // 점프 관련 변수
    private float jumpBufferTime = 0.2f, jumpBufferCounter = 0f;
    private float groundedGraceTime = 0.15f, groundedCounter = 0f;

    
    private CharacterController characterController;
    public CameraShake cameraShake { get; private set; }
    public WeaponController weaponController { get; private set; }
    private InputManager input;
    private PlayerInteraction playerInteraction;

    // 프로퍼티
    public float Health => currentHealth;
    public float SprintSpeed => sprintSpeed;
    public bool IsSprinting => isSprinting;



    #region 유니티 생명주기
    private void Awake()
    {
        input = GetComponent<InputManager>();
        playerInteraction = GetComponent<PlayerInteraction>();
        characterController = GetComponent<CharacterController>();
        cameraShake = Camera.main?.GetComponent<CameraShake>();
        weaponController = GetComponent<WeaponController>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        moveSpeed = walkSpeed;

        UpdateHealthUI();

        // 나중에 게임 매니저 혹은 다른 곳으로 옮기기
        // KeyDown
        input.BindKeyDownCommand(KeyCode.LeftShift, new SprintStartCommand(this));
        input.BindKeyDownCommand(KeyCode.Space, new JumpCommand(this));
        input.BindKeyDownCommand(KeyCode.Mouse0, new AttackCommand(this));
        input.BindKeyDownCommand(KeyCode.Mouse1, new AimStartCommand(this));
        input.BindKeyDownCommand(KeyCode.R, new ReloadCommand(this));
        input.BindKeyDownCommand(KeyCode.E, new InteractionCommand(this));
        input.BindKeyDownCommand(KeyCode.LeftControl, new CrouchToggleCommand(this));
        input.BindKeyDownCommand(KeyCode.Alpha1, new QuickSlotOneCommand(this));
        input.BindKeyDownCommand(KeyCode.Alpha2, new QuickSlotTwoCommand(this));
        input.BindKeyDownCommand(KeyCode.Alpha3, new QuickSlotThreeCommand(this));
        input.BindKeyDownCommand(KeyCode.Alpha4, new QuickSlotFourCommand(this));

        // KeyUp
        input.BindKeyUpCommand(KeyCode.LeftShift, new SprintEndCommand(this));
        input.BindKeyUpCommand(KeyCode.Mouse1, new AimEndCommand(this));
        input.BindKeyUpCommand(KeyCode.W, new StopMoveCommand(this));
        input.BindKeyUpCommand(KeyCode.A, new StopMoveCommand(this));
        input.BindKeyUpCommand(KeyCode.S, new StopMoveCommand(this));
        input.BindKeyUpCommand(KeyCode.D, new StopMoveCommand(this));


        // KeyHold
        input.BindKeyHoldCommand(KeyCode.A, new MoveLeftCommand(this));
        input.BindKeyHoldCommand(KeyCode.D, new MoveRightCommand(this));
        input.BindKeyHoldCommand(KeyCode.W, new MoveForwardCommand(this));
        input.BindKeyHoldCommand(KeyCode.S, new MoveBackCommand(this));
    }

    private void Update()
    {
        JumpCheck();
        HandleLook();
    }

    #endregion


    #region 이동/스프린트/점프/앉기

    public void MoveLeft()    { Move(Vector2.left); }
    public void MoveRight()   { Move(Vector2.right); }
    public void MoveForward() { Move(Vector2.up); }
    public void MoveBack()    { Move(Vector2.down); }

    private void Move(Vector2 direction)
    {
        Vector3 move = transform.right * direction.x + transform.forward * direction.y;
        characterController.Move(move.normalized * moveSpeed * Time.deltaTime);
        weaponController?.Move(true);
    }

    public void StopMove()
    {
        weaponController?.Move(false);
    }


    public void StartSprint()
    {
        if (!isSprinting)
        {
            isSprinting = true;
            moveSpeed = sprintSpeed;
            cameraShake?.SetSprinting(true);
            weaponController?.Sprint(true);
        }
    }
    
    public void StopSprint()
    {
        if (isSprinting)
        {
            isSprinting = false;
            moveSpeed = walkSpeed;
            cameraShake?.SetSprinting(false);
            weaponController?.Sprint(false);
        }
    }

    public void SetSpeed(float _speed)
    {
        moveSpeed = _speed;
    }

    public void Jump()
    {
        jumpBufferCounter = jumpBufferTime;
    }
    void JumpCheck()
    {
        bool isActuallyGrounded = IsGrounded();
        if (isActuallyGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (isActuallyGrounded)
            groundedCounter = groundedGraceTime;
        else if (groundedCounter > 0)
            groundedCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;

        if (groundedCounter > 0f && jumpBufferCounter > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferCounter = 0f;
            groundedCounter = 0f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        bool isJump = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        return isJump;
    } 

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Ray의 시작점
        Vector3 rayOrigin = transform.position;
        // Ray의 끝점 (아래 방향으로 groundCheckDistance만큼)
        Vector3 rayEnd = rayOrigin + Vector3.down * groundCheckDistance;
        // 선으로 표시
        Gizmos.DrawLine(rayOrigin, rayEnd);
        // 끝점에 구체(점) 표시
        Gizmos.DrawWireSphere(rayEnd, 0.03f);
    }

    public void ToggleCrouch()
    {
        if (isCrouching)
            StandUp();
        else 
            Crouch();
    }

    private void Crouch()
    {
        isCrouching = true;
        characterController.height = crouchHeight;
        moveSpeed = crouchSpeed;

        // 카메라도 낮춰 시점이 자연스럽게
        if (cameraTransform != null)
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraCrouchHeight, cameraTransform.localPosition.z);

    }

    private void StandUp()
    {
        isCrouching = false;
        characterController.height = standHeight;
        moveSpeed = walkSpeed;

        if (cameraTransform != null)
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraStandHeight, cameraTransform.localPosition.z); // 예시값
    }

    #endregion

    #region 카메라/마우스
    public void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 70f);
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
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
        UpdateHealthUI();
        if (currentHealth <= 0f)
            Debug.Log("게임 오버!");
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = currentHealth.ToString("F0");
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
        Debug.Log($"{slotIndex}가 눌려짐");
        // 슬롯에 무기가 있는가?

        // 무기가 있으면 장착
    }

    #endregion
}
