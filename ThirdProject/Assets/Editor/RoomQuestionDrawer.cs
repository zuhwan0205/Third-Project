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

        // 질문 (TextArea 스타일)
        float qHeight = EditorGUI.GetPropertyHeight(questionText, true);
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, qHeight), questionText, true);
        y += qHeight + space;

        // 타입
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, line), type);
        y += line + space;

        // 타입별 속성 출력
        if (type.enumValueIndex == (int)QuestionType.Negative && monsterList != null)
        {
            float mHeight = EditorGUI.GetPropertyHeight(monsterList, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, mHeight), monsterList, true);
        }
        else if (type.enumValueIndex == (int)QuestionType.Positive && positiveRewards != null)
        {
            float pHeight = EditorGUI.GetPropertyHeight(positiveRewards, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, pHeight), positiveRewards, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        float space = EditorGUIUtility.standardVerticalSpacing;

        var questionText = property.FindPropertyRelative("questionText");
        var type = property.FindPropertyRelative("type");
        var monsterList = property.FindPropertyRelative("monsterList");
        var positiveRewards = property.FindPropertyRelative("positiveRewards");

        height += EditorGUI.GetPropertyHeight(questionText, true) + space;
        height += EditorGUIUtility.singleLineHeight + space;

        if (type.enumValueIndex == (int)QuestionType.Negative && monsterList != null)
        {
            height += EditorGUI.GetPropertyHeight(monsterList, true);
        }
        else if (type.enumValueIndex == (int)QuestionType.Positive && positiveRewards != null)
        {
            height += EditorGUI.GetPropertyHeight(positiveRewards, true);
        }

        return height;
    }
}
