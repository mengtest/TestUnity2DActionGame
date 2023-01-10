namespace TestUnity2DActionGame.Domain.Enemy
{
    public enum EnemyState
    {
        Alive,
        Dead
    }

    public enum EnemyKind
    {
        Walking,
        Flying,
        Jumping
    }

    public enum MoveKind
    {
        Vertical,
        Holizon,
        Tracking
    }
}
