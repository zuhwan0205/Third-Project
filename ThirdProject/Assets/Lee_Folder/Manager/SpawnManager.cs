using UnityEngine;
using Fusion;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    private void Awake()
    {
        Instance = this;
    }

    public void SpawnAllPlayers(NetworkRunner runner)
    {
        if (!runner.IsServer) return;
        
        foreach (var player in runner.ActivePlayers)
        {
            Vector3 pos = GetSpawnPosition(player);
            runner.Spawn(playerPrefab, pos, Quaternion.identity, inputAuthority: player);
        }
    }

    private Vector3 GetSpawnPosition(PlayerRef player)
    {
        int index = player.RawEncoded % spawnPoints.Length;
        return spawnPoints[index].position;
    }
}