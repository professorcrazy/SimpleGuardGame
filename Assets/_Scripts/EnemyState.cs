public abstract class EnemyState
{
    protected readonly EnemyStates enemy;
    protected EnemyState(EnemyStates enemy)
    {
        this.enemy = enemy;
    }

    // Kaldes én gang, når staten starter.
    public virtual void Enter() { }

    // Kaldes én gang, når staten slutter.
    public virtual void Exit() { }

    // Kaldes hver frame, mens statdiet er aktiv.
    public abstract void Tick();
}