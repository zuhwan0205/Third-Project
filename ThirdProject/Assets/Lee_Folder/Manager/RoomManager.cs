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

public class RoomManager : MonoBehaviour
{
   public static RoomManager Instance { get; private set; }

   [SerializeField] private IntroTextBank introTextBank;
   [SerializeField] private NaturalQuestion naturalQuestions;
   [SerializeField] private RewardQuestion positiveQuestions;
   [SerializeField] private MonsterQuestion negativeQuestions;
   
   [SerializeField] private float typingSpeed = 0.05f;
   [SerializeField] private float delayBetweenLines = 1.5f;

   [Header("Spawn Settings")]
   public Transform[] spawnPoints; // Inspector에서 각 방의 스폰 포인트 할당 (9개)

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
               
               // 각 질문마다 개별 스폰
               SpawnByQuestion(question, room.view.roomIndex);
           }
           
           StartCoroutine(QuestionTimeoutCoroutine());
       });
   }

   private void SpawnByQuestion(RoomQuestion question, int currentRoomIndex)
   {
       // 생존한 방들만 가져오기
       var aliveRoomIndices = new List<int>();
       
       for (int i = 0; i < rooms.Length; i++)
       {
           if (rooms[i].state.isAlive)
           {
               aliveRoomIndices.Add(i);
           }
       }
       
       if (aliveRoomIndices.Count == 0)
       {
           Debug.LogWarning("[RoomManager] 생존한 방이 없어서 스폰하지 않습니다!");
           return;
       }
       
       // 랜덤한 생존 방 선택
       int randomRoomIndex = aliveRoomIndices[Random.Range(0, aliveRoomIndices.Count)];
       
       if (question.type == QuestionType.Positive)
       {
           SpawnRewards(randomRoomIndex, question.positiveRewards);
           Debug.Log($"[RoomManager] Room {currentRoomIndex}의 {question.type} 질문 → Room {randomRoomIndex}에 아이템 스폰!");
       }
       else if (question.type == QuestionType.Negative)
       {
           SpawnMonsters(randomRoomIndex, question.monsterList);
           Debug.Log($"[RoomManager] Room {currentRoomIndex}의 {question.type} 질문 → Room {randomRoomIndex}에 몬스터 스폰!");
       }
       else
       {
           Debug.Log($"[RoomManager] Room {currentRoomIndex}의 {question.type} 질문 → 스폰 없음");
       }
   }

   private void SpawnRewards(int roomIndex, List<RewardItem> rewards)
   {
       if (rewards == null || rewards.Count == 0) return;
       
       Vector3 spawnPos = GetSpawnPosition(roomIndex);
       
       foreach (var reward in rewards)
       {
           if (reward.itemPrefab == null) continue;
           
           if (Random.Range(0f, 1f) <= reward.dropChance)
           {
               GameObject spawnedItem = Instantiate(reward.itemPrefab, spawnPos, Quaternion.identity);
               Debug.Log($"[RoomManager] Room {roomIndex}에 '{reward.itemPrefab.name}' 아이템 스폰! (확률: {reward.dropChance:P0})");
               
               // 여러 아이템이 겹치지 않게 위치 조정
               spawnPos += Vector3.right * 1f;
           }
       }
   }

   private void SpawnMonsters(int roomIndex, List<SpawnableMonster> monsters)
   {
       if (monsters == null || monsters.Count == 0) return;
       
       Vector3 spawnPos = GetSpawnPosition(roomIndex);
       
       foreach (var monster in monsters)
       {
           if (monster.monsterPrefab == null) continue;
           
           if (Random.Range(0f, 1f) <= monster.spawnChance)
           {
               GameObject spawnedMonster = Instantiate(monster.monsterPrefab, spawnPos, Quaternion.identity);
               Debug.Log($"[RoomManager] Room {roomIndex}에 '{monster.monsterPrefab.name}' 몬스터 스폰! (확률: {monster.spawnChance:P0})");
           }
       }
   }

   private Vector3 GetSpawnPosition(int roomIndex)
   {
       if (spawnPoints != null && roomIndex < spawnPoints.Length && spawnPoints[roomIndex] != null)
       {
           return spawnPoints[roomIndex].position;
       }
       return new Vector3(roomIndex * 10f, 1f, 0f);
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