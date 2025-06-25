using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RoomQuestion))]
public class RoomQuestionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;
        float line = EditorGUIUtility.singleLineHeight;
        float space = EditorGUIUtility.standardVerticalSpacing;

        var questionText = property.FindPropertyRelative("questionText");
        var type = property.FindPropertyRelative("type");
        var monsterList = property.FindPropertyRelative("monsterList");
        var positiveRewards = property.FindPropertyRelative("positiveRewards");

        // questionText
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, line), questionText);
        y += line + space;

        // type
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, line), type);
        y += line + space;

        // 조건 분기
        if (type.enumValueIndex == (int)QuestionType.Negative)
        {
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUI.GetPropertyHeight(monsterList, true)), monsterList, true);
        }
        else if (type.enumValueIndex == (int)QuestionType.Positive)
        {
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUI.GetPropertyHeight(positiveRewards, true)), positiveRewards, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 1;

        var type = property.FindPropertyRelative("type");
        var monsterList = property.FindPropertyRelative("monsterList");
        var positiveRewards = property.FindPropertyRelative("positiveRewards");

        if (type.enumValueIndex == (int)QuestionType.Negative)
        {
            height += EditorGUI.GetPropertyHeight(monsterList, true);
        }
        else if (type.enumValueIndex == (int)QuestionType.Positive)
        {
            height += EditorGUI.GetPropertyHeight(positiveRewards, true);
        }

        return height;
    }
}
