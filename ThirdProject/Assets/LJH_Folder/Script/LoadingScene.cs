using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private TMP_Text gameLoadingText;

    private NetworkRunner runner;

    void Start()
    {
        runner = NetworkRunnerHandler.Instance.GetRunner();

        StartCoroutine(StartHostAndLoadMain());
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
        }
    }

    IEnumerator StartHostAndLoadMain()
    {
        Debug.Log("[LoadingScene] Starting as Host → Create + Join GlobalLobby.");

        var startGameTask = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "GlobalLobby",
            Scene = null,
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>() ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        yield return new WaitUntil(() => runner.IsRunning);

        Debug.Log("[LoadingScene] Host started → MainScene 이동");

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainScene");
    }
}