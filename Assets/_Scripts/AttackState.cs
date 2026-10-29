using UnityEngine;

public class AttackState : EnemyState
{
    public AttackState(EnemyStates enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        TryPrepareMovement();
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

        EnemyAttack attack =
            enemy.Combat != null
                ? enemy.Combat.GetBestAttack(
                    enemy.CurrentTarget)
                : null;

        if (attack == null)
        {
            enemy.ChangeState(enemy.ChaseState);
            return;
        }

        if (attack.CanUseWhileMoving)
        {
            enemy.Movement.MoveTo(
                enemy.CurrentTarget.position);
        }
        else
        {
            enemy.Movement.StopMoving();
        }

        attack.TryUse(enemy.CurrentTarget);
    }

    private void TryPrepareMovement()
    {
        if (enemy.Combat == null ||
            enemy.CurrentTarget == null)
        {
            return;
        }

        EnemyAttack attack =
            enemy.Combat.GetBestAttack(
                enemy.CurrentTarget);

        if (attack == null ||
            !attack.CanUseWhileMoving)
        {
            enemy.Movement.StopMoving();
        }
    }
}