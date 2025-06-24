using UnityEngine;
using Fusion;

public class TestPlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;

    private Rigidbody rb;
    private bool isGrounded;
    private NetworkInputData _cachedInput;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority && GetInput<NetworkInputData>(out var input))
        {
            _cachedInput = input;
        }

        if (!Object.HasStateAuthority) return;

        CheckGroundStatus();
        HandleMovement(_cachedInput);
    }

    private void HandleMovement(NetworkInputData input)
    {
        Vector3 move = new Vector3(input.MovementInput.x, 0, input.MovementInput.y);
        Vector3 moveWorld = transform.TransformDirection(move) * moveSpeed;
        rb.linearVelocity = new Vector3(moveWorld.x, rb.linearVelocity.y, moveWorld.z);

        if (isGrounded && input.IsJumping)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
}