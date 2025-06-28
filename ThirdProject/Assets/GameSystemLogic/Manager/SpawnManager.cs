using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform monsterSpawnPoint;
    [SerializeField] private Transform itemSpawnPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SpawnPlayer()
    {
        if (playerPrefab == null || playerSpawnPoint == null)
        {
            Debug.LogError("Player prefab or spawn point is not assigned.");
            return;
        }

        Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
    }

    public void SpawnMonsters(List<SpawnableMonster> monsters)
    {
        if (monsterSpawnPoint == null) return;

        foreach (var data in monsters)
        {
            if (Random.value <= data.spawnChance)
            {
                Instantiate(data.monsterPrefab, monsterSpawnPoint.position, Quaternion.identity);
            }
        }
    }
    public void SpawnMonsters(SpawnableMonster[] monsters)
    {
        if (monsters == null) return;
        SpawnMonsters(monsters.ToList());
    }

    public void SpawnItems(List<RewardItem> items)
    {
        if (itemSpawnPoint == null) return;

        foreach (var data in items)
        {
            if (Random.value <= data.dropChance)
            {
                Instantiate(data.itemPrefab, itemSpawnPoint.position, Quaternion.identity);
            }
        }
    }
    public void SpawnItems(RewardItem[] items)
    {
        if (items == null) return;
        SpawnItems(items.ToList());
    }
}