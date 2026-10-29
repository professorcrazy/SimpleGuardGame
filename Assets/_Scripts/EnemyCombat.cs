using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Components")]
    [SerializeField]
    private List<EnemyAttack> attacks =
        new List<EnemyAttack>();

    public bool HasUsableAttack(Transform target)
    {
        return GetBestAttack(target) != null;
    }

    public EnemyAttack GetBestAttack(Transform target)
    {
        if (target == null)
            return null;

        EnemyAttack bestAttack = null;
        float bestScore = float.MinValue;

        foreach (EnemyAttack attack in attacks)
        {
            if (attack == null ||
                !attack.IsReady ||
                !attack.CanUse(target))
            {
                continue;
            }

            float score = attack.GetPriority(target);

            if (score > bestScore)
            {
                bestScore = score;
                bestAttack = attack;
            }
        }

        return bestAttack;
    }
}