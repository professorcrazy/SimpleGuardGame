using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GuardMovement : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float stoppingDistance = 0.5f;

    private NavMeshAgent agent;
    public bool IsOnNavMesh => agent != null && agent.isOnNavMesh;

    public bool HasReachedTarget
    {
        get
        {
            if (!IsOnNavMesh)
            {
                return false;
            }

            return !agent.pathPending &&
                   agent.remainingDistance <= stoppingDistance;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }
    public void MoveTo(Vector3 position)
    {
        if (!IsOnNavMesh)
        {
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public void StopMoving()
    {
        if (!IsOnNavMesh)
        {
            return;
        }

        agent.isStopped = true;
        agent.ResetPath(); //check if this is needed or might mess up ContinueMoving()
    }

    public void ContinueMoving()
    {
        if (!IsOnNavMesh)
        {
            return;
        }
        agent.isStopped = false;
    }

    public void SetSpeed(float speed)
    {
        if (agent == null)
        {
            return;
        }

        agent.speed = speed;
    }

    public Vector3 GetVelocity()
    {
        if (agent == null)
        {
            return Vector3.zero;
        }

        return agent.velocity;
    }
}