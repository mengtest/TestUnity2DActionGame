namespace TestUnity2DActionGame.Domain.Player
{
    public enum PlayerState
    {
        Alive,
        Dead,
        StageClear
    }

    public enum MoveState
    {
        Idling,
        Moving,
        Jumping,
        Crouching,
        Climbing,
        Stopping
    }
}
