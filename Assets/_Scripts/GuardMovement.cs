using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GuardMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float stoppingDistance = 0.5f;

    private NavMeshAgent agent;

    public bool IsOnNavMesh =>
        agent != null && agent.isOnNavMesh;

    public bool HasReachedTarget
    {
        get
        {
            if (!IsOnNavMesh || agent.pathPending)
                return false;

            if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
                return false;

            return agent.remainingDistance <= agent.stoppingDistance;
        }
    }

    public bool HasValidPath =>
        IsOnNavMesh &&
        agent.hasPath &&
        agent.pathStatus != NavMeshPathStatus.PathInvalid;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    public bool MoveTo(Vector3 position)
    {
        if (!IsOnNavMesh)
            return false;

        agent.isStopped = false;
        return agent.SetDestination(position);
    }

    public void StopMoving()
    {
        if (!IsOnNavMesh)
            return;

        agent.isStopped = true;
        agent.ResetPath();
    }

    public void ContinueMoving()
    {
        if (!IsOnNavMesh)
            return;

        agent.isStopped = false;
    }

    public void SetSpeed(float speed)
    {
        if (agent != null)
            agent.speed = speed;
    }
}