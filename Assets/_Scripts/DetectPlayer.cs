using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private Transform eyePoint;
    [SerializeField] private float visionRange = 25f;
    [SerializeField] private float viewAngle = 55f;

    [Header("Layers")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Performance")]
    [SerializeField] private int maximumTargets = 20;

    private Collider[] detectedColliders;
    private void Awake()
    {
        detectedColliders = new Collider[maximumTargets];

        if (eyePoint == null)
            eyePoint = transform;
    }

    private bool IsInsideViewAngle(Transform target)
    {
        Vector3 directionToTarget =
            target.position - eyePoint.position;

        float angleToTarget = Vector3.Angle(
            transform.forward,
            directionToTarget);

        return angleToTarget <= viewAngle;
    }
    private bool HasLineOfSight(Transform target)
    {
        Vector3 directionToTarget =
            target.position - eyePoint.position;

        float distanceToTarget = directionToTarget.magnitude;

        bool obstacleWasHit = Physics.Raycast(
            eyePoint.position,
            directionToTarget.normalized,
            distanceToTarget,
            obstacleLayer,
            QueryTriggerInteraction.Ignore);

        Debug.DrawLine(
            eyePoint.position,
            target.position,
            obstacleWasHit ? Color.red : Color.green);

        return !obstacleWasHit;
    }

    public Transform GetBestVisiblePlayer()
    {
        int numberOfColliders = Physics.OverlapSphereNonAlloc(
            eyePoint.position,
            visionRange,
            detectedColliders,
            playerLayer,
            QueryTriggerInteraction.Ignore);

        Transform bestPlayer = null;
        float shortestDistance = visionRange; // Initialize with the maximum vision range instead of Mathf.Infinity

        for (int i = 0; i < numberOfColliders; i++)
        {
            Collider detectedCollider = detectedColliders[i];

            if (detectedCollider == null)
                continue;

            Transform player = detectedCollider.transform.root;

            if (!IsInsideViewAngle(player))
                continue;

            if (!HasLineOfSight(player)) //chekc if this still works if there is someting behind the player
                continue;

            float distanceToPlayer = Vector3.Distance(
                eyePoint.position,
                player.position);

            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                bestPlayer = player;
            }
        }

        return bestPlayer;
    }
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = eyePoint != null
            ? eyePoint.position
            : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, visionRange);
    }
}
