using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private TMP_Text gameLoadingText;
    private int secend = 0;
    [SerializeField] private Button Check;
    [SerializeField] private TMP_InputField NicknameInput;
    
    private const string PREF_KEY = "PlayerNickname";
    

    void Start()
    {
        Check.onClick.AddListener(ClickedCheck);
    }

    IEnumerator WaitingTextRoutine()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (true)
        {
            dotCount = (dotCount + 1) % 4;
            gameLoadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(1f);
            secend++;
            if(secend > 1) SceneManager.LoadScene("MainScene");
        }
    }

    void ClickedCheck()
    {
        string nick = NicknameInput.text.Trim();
        if (string.IsNullOrEmpty(nick))
        {
            Debug.LogWarning("닉네임을 입력해주세요.");
            return;
        }

        // PlayerPrefs에 저장
        PlayerPrefs.SetString(PREF_KEY, nick);
        PlayerPrefs.Save();
        
        StartCoroutine(WaitingTextRoutine());
    }
    
}