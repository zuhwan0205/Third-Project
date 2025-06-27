using UnityEngine;
using System.Collections;

[System.Serializable]
public class Room
{
    public RoomView view;
    public RoomStateData state;
}

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private Room[] rooms;
    [SerializeField] private IntroTextBank introTextBank;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float delayBetweenLines = 1.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitRooms();
    }

    private void InitRooms()
    {
        foreach (var room in rooms)
        {
            room.state = new RoomStateData(); // 상태 초기화
        }
    }

    public void StartIntroSequence()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        if (introTextBank == null || introTextBank.startTexts == null || introTextBank.startTexts.Length == 0)
        {
            yield break;
        }

        for (int i = 0; i < introTextBank.startTexts.Length; i++)
        {
            string currentLine = introTextBank.startTexts[i];
            int finishedCount = 0;

            foreach (var room in rooms)
            {
                if (room.view == null) continue;
                room.view.PlayText(currentLine, typingSpeed, () =>
                {
                    finishedCount++;
                });
            }

            yield return new WaitUntil(() => finishedCount >= rooms.Length);
            yield return new WaitForSeconds(delayBetweenLines);
        }
        GameManager.Instance?.OnGameIntroComplete();
    }

    public int RoomCount => rooms.Length;

    public Room GetRoom(int index)
    {
        if (index < 0 || index >= rooms.Length) return null;
        return rooms[index];
    }

    public bool IsRoomAlive(int index)
    {
        return GetRoom(index)?.state.isAlive ?? false;
    }

    public void SetRoomAlive(int index, bool isAlive)
    {
        var room = GetRoom(index);
        if (room != null)
        {
            room.state.isAlive = isAlive;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
