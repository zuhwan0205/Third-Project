using UnityEngine;
using Fusion;

public class TestPlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -20f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask = 1;
    
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private NetworkInputData _cachedInput;
    
    [Networked] public Vector3 NetworkPosition { get; set; }
    [Networked] public Quaternion NetworkRotation { get; set; }
    [Networked] public bool PositionSet { get; set; } = false;
    
    public override void Spawned()
    {
        characterController = GetComponent<CharacterController>();
        
        if (characterController == null)
        {
            enabled = false;
            return;
        }
        
        if (Object.HasStateAuthority)
        {
            NetworkPosition = transform.position;
            NetworkRotation = transform.rotation;
            PositionSet = true;
        }
        
        if (PositionSet)
        {
            characterController.enabled = false;
            transform.position = NetworkPosition + Vector3.up * 0.5f;
            transform.rotation = NetworkRotation;
            characterController.enabled = true;
        }

        moveSpeed = 5f;
    }
    
    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority && GetInput<NetworkInputData>(out var input))
        {
            // 입력을 StateAuthority가 처리할 수 있도록 저장
            _cachedInput = input;
        }

        // ✅ 이동은 반드시 StateAuthority에서만 처리
        if (Object.HasStateAuthority)
        {
            HandleMovement(_cachedInput);
            Debug.Log($"[FixedUpdate] Player: {Runner.LocalPlayer} | InputAuth: {Object.HasInputAuthority} | StateAuth: {Object.HasStateAuthority}");
            // 위치 동기화
            NetworkPosition = transform.position;
            NetworkRotation = transform.rotation;
            
        }
    }
    
    private void HandleMovement(NetworkInputData input)
    {
        if (characterController == null) return;

        CheckGroundStatus();

        // ❌ DeltaTime 제거 또는 고정값 사용
        Vector3 move = transform.right * input.MovementInput.x + transform.forward * input.MovementInput.y;
        characterController.Move(move * moveSpeed * 0.016f); // 60FPS 기준 (선택)

        if (input.IsJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * 0.016f;
        characterController.Move(velocity * 0.016f);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    
    private void CheckGroundStatus()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            isGrounded = characterController.isGrounded;
        }
    }
    
    public override void Render()
    {
        // 다른 플레이어들의 움직임을 부드럽게 동기화
        if (!Object.HasInputAuthority && PositionSet)
        {
            // 부드러운 보간으로 자연스러운 움직임
            transform.position = Vector3.Lerp(transform.position, NetworkPosition, Time.deltaTime * 8f);
            transform.rotation = Quaternion.Lerp(transform.rotation, NetworkRotation, Time.deltaTime * 8f);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}