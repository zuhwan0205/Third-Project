using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraRoot; // CameraRoot를 할당
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float clampAngle = 80f;

    private float rotationX = 0f; // 상하 회전 누적값
    private float rotationY = 0f; // 좌우 회전 누적값

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

        // 본체는 좌우 회전
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        // 카메라는 상하 회전
        cameraRoot.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}