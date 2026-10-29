using UnityEngine;

public class PatrolState : EnemyState
{
    public PatrolState(EnemyStates enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.Movement.SetSpeed(enemy.PatrolSpeed);

        if (enemy.PatrolRoute == null ||
            !enemy.PatrolRoute.HasWaypoints)
        {
            enemy.Movement.StopMoving();
            return;
        }

        enemy.PatrolRoute.ResetRoute();
        MoveToCurrentWaypoint();
    }

    public override void Tick()
    {
        if (enemy.CurrentTarget == null)
        {
            Transform visiblePlayer =
                enemy.DetectPlayer.GetBestVisiblePlayer();

            if (visiblePlayer != null)
            {
                enemy.SetCurrentTarget(visiblePlayer);
                enemy.ChangeState(enemy.ChaseState);
                return;
            }
        }

        if (enemy.PatrolRoute == null ||
            !enemy.PatrolRoute.HasWaypoints)
        {
            return;
        }

        if (!enemy.Movement.HasReachedTarget)
            return;

        enemy.PatrolRoute.GoToNextWaypoint();
        MoveToCurrentWaypoint();
    }

    private void MoveToCurrentWaypoint()
    {
        Transform waypoint = enemy.PatrolRoute.CurrentWaypoint;

        if (waypoint != null)
            enemy.Movement.MoveTo(waypoint.position);
    }
}