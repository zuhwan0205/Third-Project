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
        var yesGaugeChange = property.FindPropertyRelative("yesGaugeChange");
        var noGaugeChange = property.FindPropertyRelative("noGaugeChange");

        // 질문
        float qHeight = EditorGUI.GetPropertyHeight(questionText, true);
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, qHeight), questionText, true);
        y += qHeight + space;

        // 타입
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, line), type);
        y += line + space;

        // 타입별 속성
        if (type.enumValueIndex == (int)QuestionType.Negative && monsterList != null)
        {
            float mHeight = EditorGUI.GetPropertyHeight(monsterList, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, mHeight), monsterList, true);
            y += mHeight + space;
        }
        else if (type.enumValueIndex == (int)QuestionType.Positive && positiveRewards != null)
        {
            float pHeight = EditorGUI.GetPropertyHeight(positiveRewards, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, pHeight), positiveRewards, true);
            y += pHeight + space;
        }

        // 공간 추가 (겹침 방지)
        y += space;

        // 게이지 변화량
        EditorGUI.LabelField(new Rect(position.x, y, position.width, line), "Gauge Settings", EditorStyles.boldLabel);
        y += line + space;

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, line), yesGaugeChange, new GUIContent("Yes"));
        y += line + space;

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, line), noGaugeChange, new GUIContent("No"));

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        float line = EditorGUIUtility.singleLineHeight;
        float space = EditorGUIUtility.standardVerticalSpacing;

        var questionText = property.FindPropertyRelative("questionText");
        var type = property.FindPropertyRelative("type");
        var monsterList = property.FindPropertyRelative("monsterList");
        var positiveRewards = property.FindPropertyRelative("positiveRewards");

        height += EditorGUI.GetPropertyHeight(questionText, true) + space;
        height += line + space;

        if (type.enumValueIndex == (int)QuestionType.Negative && monsterList != null)
        {
            height += EditorGUI.GetPropertyHeight(monsterList, true) + space;
        }
        else if (type.enumValueIndex == (int)QuestionType.Positive && positiveRewards != null)
        {
            height += EditorGUI.GetPropertyHeight(positiveRewards, true) + space;
        }

        // 공간 추가 (겹침 방지)
        height += space;

        // 게이지 UI
        height += line + space; // Label
        height += line + space; // Yes
        height += line;         // No

        return height;
    }
}