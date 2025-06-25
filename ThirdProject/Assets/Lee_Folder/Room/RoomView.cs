using UnityEngine;
using TMPro;
using System.Collections;

public class RoomView : MonoBehaviour
{
    [SerializeField] private TextMeshPro screenText;
    public int roomIndex;

    public void SetAliveState(bool isAlive)
    {
        if (RoomManager.Instance == null) return;

        var room = RoomManager.Instance.GetRoom(roomIndex);
        if (room == null || room.state.hasAnswered) return;

        room.state.hasAnswered = true;
        room.state.isAlive = isAlive;
        
    }
    
    public void PlayText(string text, float typingSpeed, System.Action onComplete)
    {
        StartCoroutine(TypeTextRoutine(text, typingSpeed, onComplete));
    }

    private IEnumerator TypeTextRoutine(string text, float speed, System.Action onComplete)
    {
        screenText.text = "";
        foreach (char c in text)
        {
            screenText.text += c;
            yield return new WaitForSeconds(speed);
        }
        onComplete?.Invoke();
    }
}