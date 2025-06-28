using UnityEngine;

[System.Serializable]
public class FurnitureQuestionData
{
    [TextArea]
    public string questionText;
    public string furnitureID;
    
    public float yesGaugeChange = 0f;
    public float noGaugeChange = 0f;
}
