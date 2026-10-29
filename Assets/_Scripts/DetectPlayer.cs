using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private bool detectionEnabled = true;
    [SerializeField] private bool drawVisionCone = true;

    [Header("Vision")]
    [SerializeField] private Transform eyePoint;
    [SerializeField] private float visionRange = 25f;
    [SerializeField, Range(0f, 180f)] private float viewAngle = 55f;

    [Header("Layers")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Performance")]
    [SerializeField] private int maximumTargets = 20;

    private Collider[] detectedColliders;

    private void Awake()
    {
        maximumTargets = Mathf.Max(1, maximumTargets);
        detectedColliders = new Collider[maximumTargets];

        if (eyePoint == null)
            eyePoint = transform;
    }

    public bool DetectionEnabled => detectionEnabled;

    public Transform GetBestVisiblePlayer()
    {
        if (!detectionEnabled || eyePoint == null)
            return null;

        ClearColliderBuffer();

        int colliderCount = Physics.OverlapSphereNonAlloc(
            eyePoint.position,
            visionRange,
            detectedColliders,
            playerLayer,
            QueryTriggerInteraction.Ignore);

        Transform bestPlayer = null;
        float shortestDistance = Mathf.Infinity;

        for (int i = 0; i < colliderCount; i++)
        {
            Collider candidateCollider = detectedColliders[i];

            if (candidateCollider == null)
                continue;

            Transform candidatePlayer = candidateCollider.transform.root;

            if (!IsInsideViewAngle(candidatePlayer))
                continue;

            if (!CanSee(candidatePlayer))
                continue;

            float distance = Vector3.Distance(
                eyePoint.position,
                candidatePlayer.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                bestPlayer = candidatePlayer;
            }
        }

        return bestPlayer;
    }

    public bool CanSee(Transform target)
    {
        if (!detectionEnabled || target == null || eyePoint == null)
            return false;

        Vector3 targetPosition = GetTargetPosition(target);
        Vector3 directionToTarget = targetPosition - eyePoint.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > visionRange ||
            distanceToTarget <= Mathf.Epsilon)
        {
            return false;
        }

        if (!IsInsideViewAngle(targetPosition))
            return false;

        bool obstacleWasHit = Physics.Raycast(
            eyePoint.position,
            directionToTarget.normalized,
            distanceToTarget,
            obstacleLayer,
            QueryTriggerInteraction.Ignore);

        if (drawVisionCone)
        {
            Debug.DrawLine(
                eyePoint.position,
                targetPosition,
                obstacleWasHit ? Color.red : Color.green);
        }

        return !obstacleWasHit;
    }

    private bool IsInsideViewAngle(Transform target)
    {
        return IsInsideViewAngle(GetTargetPosition(target));
    }

    private bool IsInsideViewAngle(Vector3 targetPosition)
    {
        Vector3 directionToTarget =
            targetPosition - eyePoint.position;

        float angleToTarget = Vector3.Angle(
            transform.forward,
            directionToTarget);

        return angleToTarget <= viewAngle;
    }

    private Vector3 GetTargetPosition(Transform target)
    {
        Collider targetCollider =
            target.GetComponentInChildren<Collider>();

        if (targetCollider != null)
            return targetCollider.bounds.center;

        return target.position;
    }

    private void ClearColliderBuffer()
    {
        for (int i = 0; i < detectedColliders.Length; i++)
        {
            detectedColliders[i] = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawVisionCone)
            return;

        Transform originTransform = eyePoint != null
            ? eyePoint
            : transform;

        Vector3 origin = originTransform.position;
        Vector3 forward = transform.forward;

        Gizmos.color = detectionEnabled
            ? new Color(1f, 0.85f, 0f, 0.35f)
            : new Color(0.3f, 0.3f, 0.3f, 0.35f);

        Gizmos.DrawWireSphere(origin, visionRange);

        int rayCount = 24;
        Vector3 previousPoint = GetVisionEndpoint(
            origin,
            forward,
            -viewAngle);

        for (int i = 1; i <= rayCount; i++)
        {
            float t = i / (float)rayCount;
            float angle = Mathf.Lerp(-viewAngle, viewAngle, t);
            Vector3 currentPoint = GetVisionEndpoint(
                origin,
                forward,
                angle);

            Gizmos.DrawLine(origin, currentPoint);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

    private Vector3 GetVisionEndpoint(
        Vector3 origin,
        Vector3 forward,
        float angle)
    {
        Vector3 direction =
            Quaternion.AngleAxis(angle, Vector3.up) * forward;

        if (Physics.Raycast(
                origin,
                direction.normalized,
                out RaycastHit hit,
                visionRange,
                obstacleLayer,
                QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        return origin + direction.normalized * visionRange;
    }
}