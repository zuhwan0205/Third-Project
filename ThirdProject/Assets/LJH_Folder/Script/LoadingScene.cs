using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Unity.VisualScripting;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private TMP_Text gameLoadingText;
    private int secend = 0;
    

    void Start()
    {
        StartCoroutine(WaitingTextRoutine());
    }

    IEnumerator WaitingTextRoutine()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (true)
        {
            dotCount = (dotCount + 1) % 4;
            gameLoadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.5f);
            secend++;
            if(secend > 1) SceneManager.LoadScene("MainScene");
        }
    }
    
}