public static class GameModeManager
{
    public enum GameMode { Normal, Hard }

    public static GameMode CurrentMode { get; private set; } = GameMode.Normal;

    public static bool IsHardMode()
    {
        return CurrentMode == GameMode.Hard;
    }
}