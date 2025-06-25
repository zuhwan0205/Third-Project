using Fusion;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerInputHandler : NetworkBehaviour
{
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private float rayDistance = 5f;
    [SerializeField] private LayerMask cubeLayer;

    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority) return;
        
        if (!GameManager.Instance.IsInputAllowed) return;

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            ShootRay();
        }
    }

    private void ShootRay()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerCamera not assigned");
            return;
        }

        var camTransform = playerCamera.GetCameraTransform();
        Ray ray = new Ray(camTransform.position, camTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, cubeLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green, 1.5f);

            var cube = hit.collider.GetComponent<AnswerCube>();
            if (cube != null)
            {
                cube.OnHit();
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 1.5f);
        }
    }
}