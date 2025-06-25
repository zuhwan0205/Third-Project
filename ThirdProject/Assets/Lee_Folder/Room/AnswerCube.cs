using UnityEngine;

public class AnswerCube : MonoBehaviour
{
    private bool hasAnswered = false;

    public void OnHit()
    {
        if (hasAnswered) return;
        hasAnswered = true;
        
        RoomView view = transform.parent.GetComponentInChildren<RoomView>();
        if (view != null)
        {
            view.SetAliveState(true);
        }
        else
        {
            Debug.LogWarning("[AnswerCube] RoomView not found on sibling");
        }
    }
}