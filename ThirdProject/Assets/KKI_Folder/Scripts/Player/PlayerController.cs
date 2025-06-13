using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("플레이어 스탯")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpStamina = 3f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxStamina = 20f;
    [SerializeField] private float staminaRecoveryRate = 1f;

    [Header("카메라")]
    [SerializeField] private Transform cameraTransform;

    [Header("UI")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Text healthText;

    private float currentHealth;
    private float currentStamina;
    private float moveSpeed;
    private float xRotation = 0f;
    private bool isSprinting = false;
    private Vector3 velocity;
    private bool isGrounded;
    private CharacterController characterController;
    private CameraShake cameraShake;
    private WeaponController weaponController;

    private Coroutine staminaUICoroutine;

    public float Health => currentHealth;
    public float Stamina => currentStamina;
    public bool IsSprinting => isSprinting;

    #region 유니티 생명주기
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (cameraTransform == null) Debug.LogWarning("cameraTransform 미할당!");

        if (Camera.main != null) 
            cameraShake = Camera.main.GetComponent<CameraShake>();
        else
            Debug.LogWarning("Main Camera 없음!");

        weaponController = GetComponent<WeaponController>();
        if (weaponController == null) Debug.LogWarning("weaponController 미할당!");
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        moveSpeed = walkSpeed;

        UpdateHealthUI();
        UpdateStaminaUI();
        ShowStaminaUI(false);
    }

    private void Update()
    {
        HandleSprint();
        HandleMove();
        HandleJump();
        HandleLook();
        HandleAttack();
    }
    #endregion

    #region 이동/스프린트/점프
    private void HandleSprint()
    {
        bool sprintKey = Input.GetKey(KeyCode.LeftShift);

        if (sprintKey && currentStamina > 0f)
        {
            if (!isSprinting)
            {
                isSprinting = true;
                moveSpeed = sprintSpeed;
                cameraShake?.SetSprinting(true);
                ShowStaminaUI(true);

                // 애니메이션
                weaponController.Sprint(true);
            }
            currentStamina -= Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);
            UpdateStaminaUI();

            if (currentStamina <= 0f)
                StopSprint();
        }
        else
        {
            if (isSprinting)
                StopSprint();

            currentStamina += Time.deltaTime * staminaRecoveryRate;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            UpdateStaminaUI();
        }
    }

    private void StopSprint()
    {
        isSprinting = false;
        moveSpeed = walkSpeed;
        cameraShake?.SetSprinting(false);
        // 코루틴 중복 방지
        if (staminaUICoroutine != null)
            StopCoroutine(staminaUICoroutine);
        staminaUICoroutine = StartCoroutine(HideStaminaUIDelayed(2f));

        // 애니메이션 적용
        if (weaponController != null)
        {
            weaponController.Sprint(false);
        }
    }

    private void HandleMove()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move.normalized * moveSpeed * Time.deltaTime);

        // 애니메이션 적용
        if (weaponController != null)
        {
            weaponController.Move(move);
        }
    }

    private void HandleJump()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetButtonDown("Jump") && isGrounded && currentStamina >= jumpStamina)
        {
            currentStamina -= jumpStamina;
            UpdateStaminaUI();
            ShowStaminaUI(true);

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    #endregion

    #region 카메라/마우스
    private void HandleLook()
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

    #region 공격
    private void HandleAttack()
    {   
        bool bAttack = Input.GetKeyDown(KeyCode.Mouse0);

        if (weaponController == null) return;

        // 왼쪽 키 눌렀을 때 공격
        if (bAttack)
        {
            Debug.Log("공격키 누름");
            weaponController.Attack();
        }
    }
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

    #region 스태미나 UI 관리
    private void UpdateStaminaUI()
    {
        if (staminaSlider != null)
            staminaSlider.value = currentStamina / maxStamina;
    }

    private void ShowStaminaUI(bool flag)
    {
        if (staminaSlider != null && staminaSlider.gameObject.activeSelf != flag)
            staminaSlider.gameObject.SetActive(flag);

        // 이미 보이고 있는데 또 Show 호출할 때 불필요한 코루틴 실행 방지
        if (flag && staminaUICoroutine != null)
        {
            StopCoroutine(staminaUICoroutine);
            staminaUICoroutine = null;
        }
    }

    private IEnumerator HideStaminaUIDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowStaminaUI(false);
    }
    #endregion
}
