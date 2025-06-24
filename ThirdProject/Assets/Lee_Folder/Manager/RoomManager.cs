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
    
    [SerializeField] private QuestionBank questionBank;
    [SerializeField] private Room[] rooms;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float delayBetweenQuestions = 0.5f;

    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartIntroSequence()
    {

        foreach (var room in rooms)
        {
            room.state = new RoomStateData();
        }

        StartCoroutine(PlayIntroSequence());
    }
    
    private IEnumerator PlayIntroSequence()
    {
        while (currentIndex < questionBank.startTexts.Length)
        {
            string text = questionBank.startTexts[currentIndex];

            int finishedCount = 0;

            foreach (var room in rooms)
            {
                room.view.PlayText(text, typingSpeed, () => { finishedCount++; });
            }

            yield return new WaitUntil(() => finishedCount >= rooms.Length);
            yield return new WaitForSeconds(delayBetweenQuestions);

            currentIndex++;
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}