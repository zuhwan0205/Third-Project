using UnityEngine;

[CreateAssetMenu(menuName = "Game/Question Bank")]
public class QuestionBank : ScriptableObject
{
    [TextArea]
    public string[] startTexts;

}
