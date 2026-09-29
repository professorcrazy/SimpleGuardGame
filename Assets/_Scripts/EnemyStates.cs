using UnityEngine;

public class EnemyStates : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        search
    }
    public EnemyState currentState;
    GuardMovement guardMovement;

    public Transform playerTarget;
    [SerializeField] float viewAngle = 55f;
    [SerializeField] float visionRange = 25f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = EnemyState.Patrol;
        guardMovement = GetComponent<GuardMovement>();
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // FixedUpdate is called every physics update
    void FixedUpdate()
    {
        switch(currentState)
        {
            case EnemyState.Idle:
                // Handle Idle state behavior
                guardMovement.SetTargetPosition(transform.position);
                break;
            case EnemyState.Patrol:
                // Handle Patrol state behavior
                if (LookForPlayer())
                {
                    currentState = EnemyState.Chase;
                }
                Patrol();
                break;
            case EnemyState.Chase:
                // Handle Chase state behavior
                if (!LookForPlayer())
                {
                    currentState = EnemyState.Patrol;
                }
                guardMovement.SetTargetPosition(playerTarget.position);

                break;
            case EnemyState.Attack:
                // Handle Attack state behavior
                break;
            case EnemyState.search:
                // Handle search state behavior
                break;
        }

        void Patrol()
        {
            // Implement patrol behavior here
            guardMovement.NextWaypoint();
        }

        bool LookForPlayer()
        {
            // Implement logic to look for the player here
            Debug.DrawLine(transform.position, playerTarget.position, Color.blue);
            Vector3 rightSide = Quaternion.AngleAxis(viewAngle, Vector3.up) * transform.forward;
            Vector3 leftSide = Quaternion.AngleAxis(-viewAngle, Vector3.up) * transform.forward;
            Debug.DrawLine(transform.position, transform.position + (rightSide * visionRange), Color.red);
            Debug.DrawLine(transform.position, transform.position + (leftSide * visionRange), Color.red);
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerTarget.position - transform.position, out hit, visionRange))
            {
                if (hit.transform == playerTarget)
                {
                    Debug.Log("Target in range!");
                    if (Vector3.Dot((playerTarget.position - transform.position), transform.forward) >= Mathf.Cos(viewAngle))
                    {
                        Debug.Log("Target in sight!");
                        guardMovement.SetTargetPosition(playerTarget.position);//agent.SetDestination(playerTarget.position);

                        /*if (agent.remainingDistance < attackrange)
                        {
                            Debug.Log("Attack!");
                        }*/

                        return true;
                    }
                }
            }
            return false;
        }

    }
}
