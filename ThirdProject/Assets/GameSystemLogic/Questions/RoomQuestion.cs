using System.Collections.Generic;
using UnityEngine;

public enum QuestionType { Nautral, Positive, Negative, Complex }

[System.Serializable]
public class RoomQuestion
{
    public string questionText;
    public QuestionType type;
    public List<RewardItem> positiveRewards;
    public List<SpawnableMonster> monsterList;
}