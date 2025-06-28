using TMPro;
using DG.Tweening;
using UnityEngine;

public static class TypingText
{
    public static Tween Type(TextMeshProUGUI textUI, string content, float speed)
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
    
    public static void UpdateScoreTexts(TextMeshProUGUI yesScoreText, TextMeshProUGUI noScoreText, float yesChange, float noChange)
    {
        if (yesScoreText != null)
        {
            if (yesChange > 0)
            {
                yesScoreText.text = $"+{yesChange}";
                yesScoreText.color = Color.green;
            }
            else if (yesChange < 0)
            {
                yesScoreText.text = $"{yesChange}";
                yesScoreText.color = Color.red;
            }
            else
            {
                yesScoreText.text = "0";
                yesScoreText.color = Color.white;
            }
        }
        
        if (noScoreText != null)
        {
            if (noChange > 0)
            {
                noScoreText.text = $"+{noChange}";
                noScoreText.color = Color.green;
            }
            else if (noChange < 0)
            {
                noScoreText.text = $"{noChange}";
                noScoreText.color = Color.red;
            }
            else
            {
                noScoreText.text = "0";
                noScoreText.color = Color.white;
            }
        }
    }
    
    public static void HideScoreTexts(TextMeshProUGUI yesScoreText, TextMeshProUGUI noScoreText)
    {
        if (yesScoreText != null)
            yesScoreText.text = "";
        
        if (noScoreText != null)
            noScoreText.text = "";
    }
}