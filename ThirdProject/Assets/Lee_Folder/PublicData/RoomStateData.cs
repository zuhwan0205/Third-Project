using Fusion;

[System.Serializable]
public class RoomStateData : INetworkStruct
{
    public int roomIndex;
    public bool hasAnswered = false;
    public bool isAlive = false;
}
