using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using DG.Tweening;

[System.Serializable]
public class Room
{
    public int roomIndex = -1;
    public RoomView view;
    public RoomStateData state;
}

public class RoomManager : NetworkBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private IntroTextBank introTextBank;
    [SerializeField] private NaturalQuestion naturalQuestions;
    [SerializeField] private RewardQuestion positiveQuestions;
    [SerializeField] private MonsterQuestion negativeQuestions;
    
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float delayBetweenLines = 1.5f;

    private Room[] rooms;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        RegisterRooms();
    }

    public int RoomCount => rooms.Length;

    private void RegisterRooms()
    {
        RoomView[] foundRoomViews = GetComponentsInChildren<RoomView>(true);
        rooms = new Room[foundRoomViews.Length];

        for (int i = 0; i < foundRoomViews.Length; i++)
        {
            var view = foundRoomViews[i];
            view.roomIndex = i;

            rooms[i] = new Room
            {
                view = view,
                state = new RoomStateData { roomIndex = i }
            };
            Debug.Log($"[RoomManager] Room {i} registered: {view.gameObject.name}");
        }
    }

    public void StartIntroSequence()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        if (introTextBank == null || introTextBank.startTexts == null || introTextBank.startTexts.Length == 0)
            yield break;

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
    
    public Room GetRoom(int index)
    {
        if (index < 0 || index >= rooms.Length) return null;
        return rooms[index];
    }

    public void SetRoomAlive(int index, bool isAlive)
    {
        var room = GetRoom(index);
        if (room != null)
        {
            room.state.isAlive = isAlive;
        }
    }

    public void StartNextQuestion()
    {
        ResetAllRooms();
    
        DOVirtual.DelayedCall(0.3f, () =>
        {
            foreach (var room in rooms)
            {
                if (!room.state.isAlive) continue;
            
                RoomQuestion question = GetRandomRoomQuestion();
                if (question == null) continue;
        
                room.view.PlayText(question.questionText, typingSpeed, null);
        
                Debug.Log($"[RoomManager] Room {room.view.roomIndex}: {question.questionText} ({question.type})");
            }
        
            // DoTween 대신 코루틴 사용
            Debug.Log("[RoomManager] 10초 타이머 시작!");
            StartCoroutine(QuestionTimeoutCoroutine());
        });
    }
    
    private IEnumerator QuestionTimeoutCoroutine()
    {
        yield return new WaitForSeconds(10f);
        Debug.Log("[RoomManager] 10초 완료! OnQuestionTimeout 호출");
        OnQuestionTimeout();
    }
    
    private RoomQuestion GetRandomRoomQuestion()
    {
        int rand = Random.Range(0, 3);
        RoomQuestion[] sourceArray = null;

        switch ((QuestionType)rand)
        {
            case QuestionType.Positive:
                sourceArray = positiveQuestions.rewardQuestions;
                break;
            case QuestionType.Negative:
                sourceArray = negativeQuestions.monsterQuestions;
                break;
            case QuestionType.Neutral:
            default:
                sourceArray = naturalQuestions.naturalQuestions;
                break;
        }

        if (sourceArray == null || sourceArray.Length == 0)
            return null;

        return sourceArray[Random.Range(0, sourceArray.Length)];
    }
    
    private void ResetAllRooms()
    {
        foreach (var room in rooms)
        {
            if (!room.state.isAlive) continue;
            room.state.hasAnswered = false;
            room.view.ResetAnswerCube();
        }
    }
    
    private void OnQuestionTimeout()
    {
        Debug.Log("[RoomManager] 질문 시간 초과!");
        
        foreach (var room in rooms)
        {
            if (!room.state.isAlive) continue;
            if (!room.state.hasAnswered)
            {
                SetRoomAlive(room.view.roomIndex, false);
            }
        }
        
        var roomStates = GetAllRoomStates();
        GameManager.Instance?.OnQuestionPhaseComplete(roomStates);
    }
    
    private RoomStateData[] GetAllRoomStates()
    {
        RoomStateData[] roomStates = new RoomStateData[rooms.Length];
    
        for (int i = 0; i < rooms.Length; i++)
        {
            roomStates[i] = rooms[i].state;
        }
    
        return roomStates;
    }
    
    public void OnRoomAnswered(int roomIndex, bool isAlive)
    {
        Debug.Log($"[RoomManager] OnRoomAnswered({roomIndex}, {isAlive}) called");
    
        var room = GetRoom(roomIndex);
        if (room == null)
        {
            Debug.LogWarning($"[RoomManager] Room {roomIndex} not found");
            return;
        }

        if (room.state.hasAnswered)
        {
            Debug.LogWarning($"[RoomManager] Room {roomIndex} already answered");
            return;
        }
    
        room.state.hasAnswered = true;
        room.state.isAlive = isAlive;
    
        Debug.Log($"[RoomManager] Room {roomIndex} 상태 업데이트 완료: alive={isAlive}, answered=true");
    }
    
    public void StartSurvivalTest()
    {
        Debug.Log("[RoomManager] 첫 생존 테스트 시작 - 10초 안에 버튼을 누르세요!");
        
        ResetAllRooms();
        
        DOVirtual.DelayedCall(10f, OnQuestionTimeout);
    }
    
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
