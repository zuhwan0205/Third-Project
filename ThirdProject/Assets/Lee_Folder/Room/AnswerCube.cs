using UnityEngine;

public class AnswerCube : MonoBehaviour, IInteractable
{
    [SerializeField] private RoomView roomView;
    private bool hasAnswered = false;

    public void Interact() => OnHit();

    public string GetInteractText() => "Press [E] to Answer";

    public void OnHit()
    {
        if (hasAnswered) return;
        hasAnswered = true;

        Debug.Log("[AnswerCube] OnHit called");

        if (roomView != null)
        {
            Debug.Log("[AnswerCube] roomView found, calling SetAliveState(true)");
            roomView.SetAliveState(true);
        }
        else
        {
            Debug.LogWarning("[AnswerCube] RoomView not assigned.");
        }
    }

    public void ResetAnswer() => hasAnswered = false;
}