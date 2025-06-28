using System;
using System.Collections;
using UnityEngine;

public class GameScene_Button : MonoBehaviour
{
    [SerializeField] private Animator pushNoBtn;
    [SerializeField] private Animator pushYesBtn;
    [SerializeField] private AudioSource pushBtn;
    public static GameScene_Button instance;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void PushNoButton()
    {
        pushYesBtn.SetTrigger("PushYes");
        pushBtn.Play();
        Debug.Log("PushNo");
    }

    public void PushYesButton()
    {
        pushNoBtn.SetTrigger("PushNo");
        pushBtn.Play();
        Debug.Log("Pushyes");
    }

    
}
