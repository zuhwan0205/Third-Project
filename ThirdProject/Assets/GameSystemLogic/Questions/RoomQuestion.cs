using System.Collections.Generic;
using UnityEngine;

public enum QuestionType { Nautral, Positive, Negative, Complex }

[System.Serializable]
public class RoomQuestion
{
    [TextArea]
    public string questionText;
    public QuestionType type;
    public List<RewardItem> positiveRewards;
    public List<SpawnableMonster> monsterList;
    
    public float yesGaugeChange = 0f;
    public float noGaugeChange = 0f;
}