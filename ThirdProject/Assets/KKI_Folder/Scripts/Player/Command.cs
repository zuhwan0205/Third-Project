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
#endregion

#region 스프린트/점프
public class SprintStartCommand : ICommand
{
    private PlayerController player;
    public SprintStartCommand(PlayerController p) {player = p;}
    public void Execute() {player.StartSprint();}
}

public class SprintEndCommand : ICommand
{
    private PlayerController player;
    public SprintEndCommand(PlayerController p) {player = p;}
    public void Execute() {player.StopSprint();}
}

public class JumpCommand : ICommand
{
    private PlayerController player;
    public JumpCommand(PlayerController p) {player = p;}
    public void Execute() {player.Jump();}
}
#endregion

#region 공격/에임/재장전
public class AttackCommand : ICommand
{
    private PlayerController player;
    public AttackCommand(PlayerController p) {player = p;}
    public void Execute() {player.Attack();}
}

public class AimStartCommand : ICommand
{
    private PlayerController player;
    public AimStartCommand(PlayerController p) {player = p;}
    public void Execute() {player.AimStart();}
}

public class AimEndCommand : ICommand
{
    private PlayerController player;
    public AimEndCommand(PlayerController p) {player = p;}
    public void Execute() {player.AimEnd();}
}

public class ReloadCommand : ICommand
{
    private PlayerController player;
    public ReloadCommand(PlayerController p) {player = p;}
    public void Execute() {player.Reload();}
}
#endregion 