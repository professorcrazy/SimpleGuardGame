using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private bool canUseWhileMoving;
    [SerializeField] private int priority = 0;

    private float nextUseTime;

    public bool CanUseWhileMoving => canUseWhileMoving;
    public bool IsReady => Time.time >= nextUseTime;
    public int Priority => priority;

    public abstract bool CanUse(Transform target);

    public abstract float GetPriority(Transform target);

    protected abstract void PerformAttack(Transform target);

    public bool TryUse(Transform target)
    {
        if (target == null || !IsReady || !CanUse(target))
            return false;

        PerformAttack(target);
        nextUseTime = Time.time + cooldown;
        return true;
    }
}