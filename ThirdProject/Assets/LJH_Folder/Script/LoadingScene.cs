using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text gameLoadingText;
    
    void Start() {
        StartCoroutine(LoadLobby());
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

    IEnumerator LoadLobby() {
        // 네트워크 연결 시도
        if (!PhotonNetwork.IsConnected) {
            PhotonNetwork.ConnectUsingSettings();
        }

        while (!PhotonNetwork.IsConnectedAndReady)
            yield return null;
    }
}
