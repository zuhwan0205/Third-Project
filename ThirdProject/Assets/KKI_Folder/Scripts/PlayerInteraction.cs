using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactMask;    
    [SerializeField] private GameObject cursor;

    private bool bInteract;
    private Animator cursorAnimator;

    void Start()
    {
        cursorAnimator = cursor.GetComponent<Animator>();
    }

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactMask))
        {   
            bInteract = true;
            Debug.Log("오브젝트 : " + hit.collider.gameObject.name);

            // 애니메이션 작용
            cursorAnimator.SetBool("bZoom", true);

            if (Input.GetKeyDown(KeyCode.E) && bInteract == true)
            {
                Debug.Log("상호작용!");
            }
        }   
        else 
        {
            if (bInteract) 
            {
                cursorAnimator.SetBool("bZoom", false);
            }
        }
    }
}
