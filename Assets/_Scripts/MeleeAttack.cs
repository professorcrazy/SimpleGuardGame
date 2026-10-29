using UnityEngine;

public class MeleeAttack : EnemyAttack
{
    [Header("Melee")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int damage = 50;

    private void Reset()
    {
        // Melee er som standard ikke et moving attack.
    }

    public override bool CanUse(Transform target)
    {
        if (target == null)
            return false;

        float distance = Vector3.Distance(
            transform.position,
            target.position);

        return distance <= attackRange;
    }

    public override float GetPriority(Transform target)
    {
        if (!CanUse(target))
            return float.MinValue;

        return 100f + Priority;
    }

    protected override void PerformAttack(Transform target)
    {
        Health targetHealth =
            target.GetComponentInChildren<Health>();

        if (targetHealth == null)
        {
            Debug.LogWarning(
                $"{target.name} mangler en Health-komponent.",
                target);
            return;
        }

        targetHealth.TakeDamage(damage);
    }
}