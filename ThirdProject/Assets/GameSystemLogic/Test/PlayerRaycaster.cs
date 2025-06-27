using UnityEngine;

public class PlayerRaycaster : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private LayerMask buttonLayerMask;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, buttonLayerMask))
            {
                Debug.Log($"[Raycast] E키로 버튼 감지: {hit.collider.gameObject.name}");
            }
            
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green, 1.0f);
        }
    }
}