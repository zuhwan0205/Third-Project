using System.Collections.Generic;
using UnityEngine;
using Fusion;


public class InputManager : NetworkBehaviour
{
    private Dictionary<KeyCode, ICommand> keyDownCommandMap = new();
    private Dictionary<KeyCode, ICommand> keyUpCommandMap = new();
    private Dictionary<KeyCode, ICommand> keyHoldCommandMap = new();

    public void BindKeyDownCommand(KeyCode key, ICommand command) => keyDownCommandMap[key] = command;

    public void BindKeyUpCommand(KeyCode key, ICommand command) =>  keyUpCommandMap[key] = command;
    
    public void BindKeyHoldCommand(KeyCode key, ICommand command) => keyHoldCommandMap[key] = command;


    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false) return;

        foreach (var pair in keyDownCommandMap)
            if (Input.GetKeyDown(pair.Key)) pair.Value.Execute();

        foreach (var pair in keyUpCommandMap)
            if (Input.GetKeyUp(pair.Key)) pair.Value.Execute();

        foreach (var pair in keyHoldCommandMap)
            if (Input.GetKey(pair.Key)) pair.Value.Execute();
    }


    
}
