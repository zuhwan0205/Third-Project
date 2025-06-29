using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ending Text Bank")]
public class EndingTextBank : ScriptableObject
{
    [Header("Good Ending")] [TextArea] public string[] goodEndingTexts;
}