using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyStates enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.Movement.SetSpeed(enemy.ChaseSpeed);
    }

    public override void Tick()
    {
        if (enemy.CurrentTarget == null)
        {
            enemy.ChangeState(enemy.PatrolState);
            return;
        }

        if (!enemy.DetectPlayer.CanSee(enemy.CurrentTarget))
        {
            if (enemy.HasLastSeenPosition)
            {
                enemy.ChangeState(
                    enemy.GoToLastSeenPositionState);
            }
            else
            {
                enemy.ClearCurrentTarget();
                enemy.ChangeState(enemy.PatrolState);
            }

            return;
        }

        enemy.UpdateLastSeenPosition();

        if (enemy.Combat != null &&
            enemy.Combat.HasUsableAttack(enemy.CurrentTarget))
        {
            enemy.ChangeState(enemy.AttackState);
            return;
        }

        enemy.Movement.MoveTo(
            enemy.CurrentTarget.position);
    }
}