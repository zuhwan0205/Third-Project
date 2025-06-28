using UnityEngine;

[System.Serializable]
public class ComplexQuestionData
{
    [TextArea]
    public string questionText;
    
    [Header("Monster")]
    public bool spawnMonster;
    public SpawnableMonster[] extraMonsters;

    [Header("Reward")]
    public bool spawnReward;
    public RewardItem[] extraRewards;
}
