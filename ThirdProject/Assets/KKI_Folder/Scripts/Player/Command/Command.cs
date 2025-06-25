public interface ICommand 
{
    void Execute();
}

#region 움직임
public class MoveLeftCommand : ICommand
{
    private PlayerController player;
    public MoveLeftCommand(PlayerController p) { player = p; }
    public void Execute() { player.MoveLeft(); }
}

public class MoveRightCommand : ICommand
{
    private PlayerController player;
    public MoveRightCommand(PlayerController p) { player = p; }
    public void Execute() { player.MoveRight(); }
}
public class MoveForwardCommand : ICommand
{
    private PlayerController player;
    public MoveForwardCommand(PlayerController p) { player = p; }
    public void Execute() { player.MoveForward(); }
}

public class MoveBackCommand : ICommand
{
    private PlayerController player;
    public MoveBackCommand(PlayerController p) { player = p; }
    public void Execute() { player.MoveBack(); }
}

public class StopMoveCommand : ICommand
{
    private PlayerController player;
    public StopMoveCommand(PlayerController player) { this.player = player; }
    public void Execute() => player.StopMove();
}

#endregion

#region 스프린트/점프/앉기
public class SprintStartCommand : ICommand
{
    private PlayerController player;
    public SprintStartCommand(PlayerController p) {player = p;}
    public void Execute() { player.StartSprint(); }
}

public class SprintEndCommand : ICommand
{
    private PlayerController player;
    public SprintEndCommand(PlayerController p) {player = p;}
    public void Execute() { player.StopSprint(); }
}

public class JumpCommand : ICommand
{
    private PlayerController player;
    public JumpCommand(PlayerController p) {player = p;}
    public void Execute() { player.Jump(); }
}

public class CrouchToggleCommand : ICommand
{
    private PlayerController player;
    public CrouchToggleCommand(PlayerController player) { this.player = player; }
    public void Execute() => player.ToggleCrouch();
}

#endregion

#region 공격/에임/재장전
public class AttackCommand : ICommand
{
    private PlayerController player;
    public AttackCommand(PlayerController p) {player = p;}
    public void Execute() => player.Attack();
}

public class AimStartCommand : ICommand
{
    private PlayerController player;
    public AimStartCommand(PlayerController p) {player = p;}
    public void Execute() => player.AimStart();
}

public class AimEndCommand : ICommand
{
    private PlayerController player;
    public AimEndCommand(PlayerController p) {player = p;}
    public void Execute() => player.AimEnd();
}

public class ReloadCommand : ICommand
{
    private PlayerController player;
    public ReloadCommand(PlayerController p) {player = p;}
    public void Execute() => player.Reload();
}
#endregion 

#region 상호작용
public class InteractionCommand : ICommand
{
    private PlayerController player;
    public InteractionCommand(PlayerController player) { this.player = player; }
    public void Execute() => player.Interaction();
}

#endregion

#region 퀵 슬롯
public class AxeQuickSlotCommand : ICommand
{
    private PlayerController player;
    public AxeQuickSlotCommand(PlayerController player) { this.player = player; }
    public void Execute() => player.SelectItemSlot(0);
}

public class ShortSwordQuickSlotCommand : ICommand
{
    private PlayerController player;
    public ShortSwordQuickSlotCommand(PlayerController player) { this.player = player; }
    public void Execute() => player.SelectItemSlot(1);
}

public class PistolQuickSlotCommand : ICommand
{
    private PlayerController player;
    public PistolQuickSlotCommand(PlayerController player) { this.player = player; }
    public void Execute() => player.SelectItemSlot(2);
}

public class ShotgunQuickSlotCommand : ICommand
{
    private PlayerController player;
    public ShotgunQuickSlotCommand(PlayerController player) { this.player = player; }
    public void Execute() => player.SelectItemSlot(3);
}

public class BowQuickSlotCommand : ICommand
{
    private PlayerController player;
    public BowQuickSlotCommand(PlayerController player) { this.player = player; }
    public void Execute() => player.SelectItemSlot(4);
}

#endregion

