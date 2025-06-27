using UnityEngine;

public interface ICommand 
{
    void Execute();
}

public class PlayerInputBuffer
{
    public Vector2 MovementInput;
    public float MouseX;
    public float MouseY;
    public bool IsJumping;
    public bool IsSprinting;
    public bool IsCrouching;
    public bool IsAttacking;
    public bool IsAiming;
    public bool IsReloading;
    public int QuickSlotIndex = -1;
    public bool IsInteracting;

    public void Reset()
    {
        MovementInput = Vector2.zero;
        MouseX = 0;
        MouseY = 0;
        IsJumping = false;
        IsAttacking = false;
        IsAiming = false;
        IsReloading = false;
        QuickSlotIndex = -1;
        IsInteracting = false;
    }
}


#region 움직임
public class MoveLeftCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public MoveLeftCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.MovementInput.x = -1f; }
}

public class MoveRightCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public MoveRightCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.MovementInput.x = 1f; }
}

public class MoveForwardCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public MoveForwardCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.MovementInput.y = 1f; }
}

public class MoveBackCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public MoveBackCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.MovementInput.y = -1f; }
}

public class StopMoveCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public StopMoveCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.MovementInput = Vector2.zero; }
}
#endregion

#region 스프린트/점프/앉기
public class SprintStartCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public SprintStartCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.IsSprinting = true; }
}

public class JumpCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public JumpCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.IsJumping = true; }
}

public class CrouchStartCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public CrouchStartCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.IsCrouching = true;  }
}
#endregion

#region 공격/에임/재장전
public class AttackCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public AttackCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.IsAttacking = true; }
}

public class AimStartCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public AimStartCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.IsAiming = true; }
}

public class AimEndCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public AimEndCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.IsAiming = false; }
}

public class ReloadCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public ReloadCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.IsReloading = true; }
}
#endregion 

#region 상호작용
public class InteractionCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public InteractionCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.IsInteracting = true; }
}
#endregion

#region 퀵 슬롯
public class AxeQuickSlotCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public AxeQuickSlotCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.QuickSlotIndex = 0; }
}

public class ShortSwordQuickSlotCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public ShortSwordQuickSlotCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.QuickSlotIndex = 1; }
}

public class PistolQuickSlotCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public PistolQuickSlotCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.QuickSlotIndex = 2; }
}

public class ShotgunQuickSlotCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public ShotgunQuickSlotCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.QuickSlotIndex = 3; }
}

public class BowQuickSlotCommand : ICommand
{
    private PlayerInputBuffer buffer;
    public BowQuickSlotCommand(PlayerInputBuffer buf) { buffer = buf; }
    public void Execute() { buffer.QuickSlotIndex = 3; }
}

#endregion

