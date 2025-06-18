using UnityEngine;

public class SprintState : IPlayerState
{
    private PlayerController player;
    public SprintState(PlayerController p) { player = p; }

    public void Enter()
    {
        player.SetSpeed(player.SprintSpeed);
        player.cameraShake?.SetSprinting(true);
        player.weaponController?.Sprint(true);
    }

    public void HandleInput()
    {

    }

    public void Update()
    {

    }

    public void Exit()
    {

    }

}