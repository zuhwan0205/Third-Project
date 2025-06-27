using UnityEngine;

[System.Serializable]
public class RewardItem
{
    public GameObject itemPrefab;

    [Range(0f, 1f)]
    public float dropChance;
}
