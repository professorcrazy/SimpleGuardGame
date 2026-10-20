using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyStates enemy) : base(enemy)
    {
    }
    public override void Enter()
    {
        enemy.Movement.SetSpeed(enemy.ChaseSpeed);
        enemy.Movement.ContinueMoving();
    }

    public override void Tick()
    {

        if (enemy.CurrentTarget == null)
        {
            enemy.ChangeState(enemy.PatrolState);
            return;
        }
        Transform visiblePlayer = enemy.DetectPlayer.GetBestVisiblePlayer();

        if (visiblePlayer != null)
        {
            enemy.SetCurrentTarget(visiblePlayer);
        }
        
        if(enemy.IsCurrentTargetToFarAway())
        {
            enemy.ClearCurrentTarget();
            enemy.ChangeState(enemy.PatrolState);
            return;
        }

        enemy.Movement.MoveTo(enemy.CurrentTarget.position);
    }
}
