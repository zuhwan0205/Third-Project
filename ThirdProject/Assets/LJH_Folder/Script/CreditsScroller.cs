using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroller : MonoBehaviour
{
    
    private float scrollSpeed = 70f;
    
    private float endY = 3600f;

    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        rt.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        
        if (rt.anchoredPosition.y >= endY)
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
