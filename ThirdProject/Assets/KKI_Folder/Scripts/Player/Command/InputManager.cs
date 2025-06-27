using System.Collections.Generic;
using UnityEngine;
using Fusion;


public class InputManager : NetworkBehaviour
{
    public static InputManager Instance;
    public PlayerInputBuffer inputBuffer = new();

    private Dictionary<KeyCode, ICommand> keyDownCommandMap = new();
    private Dictionary<KeyCode, ICommand> keyUpCommandMap = new();
    private Dictionary<KeyCode, ICommand> keyHoldCommandMap = new();

    public void BindKeyDownCommand(KeyCode key, ICommand command) => keyDownCommandMap[key] = command;
    public void BindKeyUpCommand(KeyCode key, ICommand command) =>  keyUpCommandMap[key] = command;
    public void BindKeyHoldCommand(KeyCode key, ICommand command) => keyHoldCommandMap[key] = command;


    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false) return;

        inputBuffer.IsSprinting = false;
        inputBuffer.IsCrouching = false;

        foreach (var pair in keyDownCommandMap)
            if (Input.GetKeyDown(pair.Key)) pair.Value.Execute();

        foreach (var pair in keyUpCommandMap)
            if (Input.GetKeyUp(pair.Key)) pair.Value.Execute();

        foreach (var pair in keyHoldCommandMap)
            if (Input.GetKey(pair.Key)) pair.Value.Execute();

        // 마우스 입력도 저장 (항상 갱신)
        inputBuffer.MouseX = Input.GetAxis("Mouse X");
        inputBuffer.MouseY = Input.GetAxis("Mouse Y");
    }

    void Awake()
    {
        if (Instance != null && Instance == this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
