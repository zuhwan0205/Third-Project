using UnityEngine;
using Fusion;
using Unity.Cinemachine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera playerCam;
    
    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
        {
            if (playerCam != null)
            {
                playerCam.gameObject.SetActive(false);
            }
            enabled = false;
            return;
        }
        
        if (playerCam != null)
        {
            playerCam.gameObject.SetActive(true);
            
            playerCam.Follow = transform;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }
    
    void Update()
    {
        if (!Object.HasInputAuthority) return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursor();
        }
    }
    
    private void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    public Transform GetCameraTransform()
    {
        return playerCam.transform;
    }

    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}