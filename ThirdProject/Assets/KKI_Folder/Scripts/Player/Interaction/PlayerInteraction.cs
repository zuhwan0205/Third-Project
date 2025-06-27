using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactMask;    
    public Text interactionText;

    private bool bInteract;
    private bool bText;
    private IInteractable currentInteractable;

    void Update()
    {
        if (!Object.HasInputAuthority) return;
        
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactMask))
        {   
            currentInteractable = hit.collider.GetComponent<IInteractable>();

            if (currentInteractable != null)
            {
                //Debug.Log("오브젝트 Interactable : " + hit.collider.gameObject.name);
                bInteract = true;
                // 애니메이션 작용
                CursorManager.Instance.SetZoom(true);

                // UI에 currentInteractable.GetInteractText() 표시
                InteractionTextSetting(true, currentInteractable.GetInteractText());
            }
            else
            {
                bInteract = false;
                CursorManager.Instance.SetZoom(false);
                InteractionTextSetting(false);
            }
        }   
        else 
        {
            bInteract = false;
            currentInteractable = null;
            CursorManager.Instance.SetZoom(false);
            InteractionTextSetting(false);
        }
    }

    public void Interaction()
    {
        if (currentInteractable == null || bInteract == false) return; 

        currentInteractable.Interact();
    }

    private void InteractionTextSetting(bool flag, string interactionText = null)
    {
        if (this.interactionText == null) return;
    
        if (bText == flag) return;
        bText = flag;
    
        this.interactionText.gameObject.SetActive(flag);
        if (interactionText != null)
            this.interactionText.text = interactionText;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Scene에서 플레이어가 바라보는 "정중앙" 방향의 Ray를 그리자!
        if (Camera.main != null)
        {
            // 1. Ray의 시작점: 카메라 위치
            Vector3 rayOrigin = Camera.main.transform.position;
            // 2. Ray의 끝점: 카메라 정면 방향 * interactDistance
            Vector3 rayDir = Camera.main.transform.forward;
            Vector3 rayEnd = rayOrigin + rayDir * interactDistance;

            // 3. 선으로 표시
            Gizmos.DrawLine(rayOrigin, rayEnd);
            // 4. 끝점에 구체 표시
            Gizmos.DrawWireSphere(rayEnd, 0.05f);
        }
    }

}
