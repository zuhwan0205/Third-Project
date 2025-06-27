using UnityEngine;

[CreateAssetMenu(menuName = "Game/Intro Text Bank")]
public class IntroTextBank : ScriptableObject
{
    [TextArea]
    public string[] startTexts;
}
