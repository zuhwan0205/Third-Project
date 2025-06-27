using UnityEngine;
using TMPro;
using System.Collections;
using Fusion;

public class RoomView : MonoBehaviour
{
    [SerializeField] private TextMeshPro screenText;
    [SerializeField] private AnswerCube answerCube;
    public int roomIndex { get; set; }

    public void SetAliveState(bool isAlive)
    {
        Debug.Log($"[RoomView] SetAliveState({isAlive}) called for room {roomIndex}");
    
        var room = RoomManager.Instance?.GetRoom(roomIndex);
        if (room == null)
        {
            Debug.LogWarning("[RoomView] Room not found for index: " + roomIndex);
            return;
        }

        if (room.state.hasAnswered)
        {
            Debug.LogWarning("[RoomView] Already answered.");
            return;
        }

        Debug.Log($"[RoomView] Calling RoomManager.OnRoomAnswered({roomIndex}, {isAlive})");
        RoomManager.Instance?.OnRoomAnswered(roomIndex, isAlive);
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
    public void ResetAnswerCube()
    {
        if (answerCube != null)
            answerCube.ResetAnswer();
    }
}