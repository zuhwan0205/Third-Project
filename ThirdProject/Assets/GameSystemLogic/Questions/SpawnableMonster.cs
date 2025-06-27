using UnityEngine;

[System.Serializable]
public class SpawnableMonster
{
    public GameObject monsterPrefab;

    [Range(0f, 1f)]
    public float spawnChance;
}
