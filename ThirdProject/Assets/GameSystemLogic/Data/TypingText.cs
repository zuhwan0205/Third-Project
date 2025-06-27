using TMPro;
using DG.Tweening;
using UnityEngine;

public static class TypingText
{
    public static Tween Type(TextMeshPro textUI, string content, float speed)
    {
        textUI.text = "";
        string current = "";

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < content.Length; i++)
        {
            char c = content[i];
            seq.AppendCallback(() =>
            {
                current += c;
                textUI.text = current;
            });
            
            seq.AppendInterval(speed);
        }

        return seq;
    }
}