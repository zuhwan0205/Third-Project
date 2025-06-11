using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;

public class SpawnManager : MonoBehaviourPun
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private Transform[] spawnPoints;
    private List<int> usedIndices = new List<int>();

    private void Awake()
    {
        Instance = this;
    }

    public void AssignAllSpawnPositions()
    {
        Debug.Log("[SpawnManager] 테스트용 스폰 시작");

        int dummyPlayerCount = 3; // 테스트용 플레이어 수
        for (int i = 0; i < dummyPlayerCount; i++)
        {
            int index = GetUniqueRandomIndex();
            if (index == -1)
            {
                Debug.LogWarning("스폰 위치 부족");
                return;
            }

            Vector3 pos = spawnPoints[index].position;
            Quaternion rot = spawnPoints[index].rotation;

            GameObject go = Instantiate(Resources.Load<GameObject>("Player_LeeTest"), pos, rot);
            go.name = $"DummyPlayer_{i + 1}";
        }
    }
    
    /*public void AssignAllSpawnPositions()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            int index = GetUniqueRandomIndex();
            if (index == -1) return;

            Vector3 pos = spawnPoints[index].position;
            Quaternion rot = spawnPoints[index].rotation;
            
            photonView.RPC(nameof(RPC_SpawnAt), player, pos, rot);
        }
    }*/
    
    /*[PunRPC]
    private void RPC_SpawnAt(Vector3 pos, Quaternion rot)
    {
        PhotonNetwork.Instantiate("Player", pos, rot);
    }*/

    private int GetUniqueRandomIndex()
    {
        int max = spawnPoints.Length;
        if (usedIndices.Count >= max) return -1;

        int rand;
        do { rand = Random.Range(0, max); } while (usedIndices.Contains(rand));

        usedIndices.Add(rand);
        return rand;
    }
}
