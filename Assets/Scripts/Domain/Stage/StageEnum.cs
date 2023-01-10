namespace TestUnity2DActionGame.Domain.Stage1
{
    public enum CharacterKind
    {
        Player,
        Enemy
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
