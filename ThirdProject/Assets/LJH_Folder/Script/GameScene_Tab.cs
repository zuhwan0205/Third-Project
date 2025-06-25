using System;
using UnityEngine;

public class GameScene_Tab : MonoBehaviour
{
    [SerializeField] private GameObject TabPanel;
    private bool TabActive;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Tab();
            TabActive = !TabActive;
        }
    }

    void Tab()
    {
        TabPanel.SetActive(TabActive);
    }
}
