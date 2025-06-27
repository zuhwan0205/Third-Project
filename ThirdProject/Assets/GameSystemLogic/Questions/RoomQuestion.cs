using System.Collections.Generic;
using UnityEngine;

public enum QuestionType { Neutral, Positive, Negative }

[System.Serializable]
public class RoomQuestion
{
    public string questionText;
    public QuestionType type;
    public List<RewardItem> positiveRewards;
    public List<SpawnableMonster> monsterList;
}