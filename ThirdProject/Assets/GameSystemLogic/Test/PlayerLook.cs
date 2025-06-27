using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float clampAngle = 80f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle);
        
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        cameraRoot.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}