using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform cameraRoot;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalSpeed = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        Vector3 moveDir = cameraRoot.transform.forward * v + cameraRoot.transform.right * h;
        moveDir.y = 0f;
        moveDir.Normalize();
        
        if (controller.isGrounded)
        {
            verticalSpeed = -1f;
            if (Input.GetButtonDown("Jump"))
            {
                verticalSpeed = 5f;
            }
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
        }

        velocity = moveDir * moveSpeed + Vector3.up * verticalSpeed;
        controller.Move(velocity * Time.deltaTime);
    }
}