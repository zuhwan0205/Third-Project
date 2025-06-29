using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathExit : MonoBehaviour
{
    public static DeathExit Instance;

    [SerializeField] private GameObject DeathPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DeathPanel.SetActive(false);
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Death()
    {
        DeathPanel.SetActive(true);
    }
}