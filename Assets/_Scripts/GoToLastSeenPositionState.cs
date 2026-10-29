using UnityEngine;

public class GoToLastSeenPositionState : EnemyState
{
    public GoToLastSeenPositionState(EnemyStates enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.Movement.SetSpeed(enemy.ChaseSpeed);

        if (enemy.HasLastSeenPosition)
        {
            enemy.Movement.MoveTo(
                enemy.LastSeenPosition);
        }
    }

    public override void Tick()
    {
        if (enemy.CurrentTarget != null &&
            enemy.DetectPlayer.CanSee(enemy.CurrentTarget))
        {
            enemy.UpdateLastSeenPosition();
            enemy.ChangeState(enemy.ChaseState);
            return;
        }

        if (!enemy.HasLastSeenPosition)
        {
            enemy.ChangeState(enemy.PatrolState);
            return;
        }

        if (!enemy.Movement.HasReachedTarget)
            return;

        enemy.ChangeState(enemy.SearchState);
    }
}