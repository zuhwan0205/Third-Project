using System;
using UnityEngine;

public class GameScene_Button : MonoBehaviour
{
    [SerializeField] private Animator pushNoBtn;
    [SerializeField] private Animator pushYesBtn;

    void Start()
    {
        PushNoButton();
        PushYesButton();
    }

    void PushNoButton()
    {
        pushNoBtn.SetTrigger("PushNo");
    }

    void PushYesButton()
    {
        pushYesBtn.SetTrigger("PushYes");
    }
}
