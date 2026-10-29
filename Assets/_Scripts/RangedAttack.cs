using UnityEngine;

public class RangedAttack : EnemyAttack
{
    [Header("Ranged Attack")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private int damage = 15;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private LayerMask obstacleLayer;

    private void Awake()
    {
        if (firePoint == null)
            firePoint = transform;

        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                $"{name} mangler et Projectile Prefab.",
                this);
        }
    }

    public override bool CanUse(Transform target)
    {
        if (target == null ||
            firePoint == null ||
            projectilePrefab == null)
        {
            return false;
        }

        Vector3 targetPosition = GetTargetPosition(target);
        Vector3 directionToTarget =
            targetPosition - firePoint.position;

        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > attackRange ||
            distanceToTarget <= Mathf.Epsilon)
        {
            return false;
        }

        bool obstacleWasHit = Physics.Raycast(
            firePoint.position,
            directionToTarget.normalized,
            distanceToTarget,
            obstacleLayer,
            QueryTriggerInteraction.Ignore);

        return !obstacleWasHit;
    }

    public override float GetPriority(Transform target)
    {
        if (!CanUse(target))
            return float.MinValue;

        // MeleeAttack har i den tidligere løsning typisk 100.
        // RangedAttack ligger derfor normalt lavere.
        return 50f + Priority;
    }

    protected override void PerformAttack(Transform target)
    {
        if (projectilePrefab == null ||
            firePoint == null ||
            target == null)
        {
            return;
        }

        Vector3 targetPosition = GetTargetPosition(target);
        Vector3 directionToTarget =
            (targetPosition - firePoint.position).normalized;

        Projectile projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(directionToTarget));

        projectile.Initialize(
            damage,
            transform.root,
            directionToTarget,
            projectileSpeed);
    }

    private Vector3 GetTargetPosition(Transform target)
    {
        Collider targetCollider =
            target.GetComponentInChildren<Collider>();

        if (targetCollider != null)
            return targetCollider.bounds.center;

        return target.position;
    }
}