using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    private Dictionary<KeyCode, ICommand> keyDownCommandMap = new();
    private Dictionary<KeyCode, ICommand> keyUpCommandMap = new();
    private Dictionary<KeyCode, ICommand> keyHoldCommandMap = new();

    public void BindKeyDownCommand(KeyCode key, ICommand command) => keyDownCommandMap[key] = command;

    public void BindKeyUpCommand(KeyCode key, ICommand command) =>  keyUpCommandMap[key] = command;
    
    public void BindKeyHoldCommand(KeyCode key, ICommand command) => keyHoldCommandMap[key] = command;


    void Update()
    {
        foreach (var pair in keyDownCommandMap)
            if (Input.GetKeyDown(pair.Key)) pair.Value.Execute();

        foreach (var pair in keyUpCommandMap)
            if (Input.GetKeyUp(pair.Key)) pair.Value.Execute();

        foreach (var pair in keyHoldCommandMap)
            if (Input.GetKey(pair.Key)) pair.Value.Execute();
    }

}
