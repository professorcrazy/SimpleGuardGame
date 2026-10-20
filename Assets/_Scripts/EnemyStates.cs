using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class EnemyStates : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GuardMovement guardMovement;
    [SerializeField] private PatrolRoute patrolRoute;
    [SerializeField] private DetectPlayer detectPlayer;

    [Header("State Settings")]
    [SerializeField] private bool startInPatrol = true;

    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;

    [Header("Target Settings")]
    [SerializeField] private float loseTargetDistance = 30f;
    
    [Header("Debug")]
    [SerializeField] private string currentStateName;

    public GuardMovement Movement => guardMovement;
    public PatrolRoute PatrolRoute => patrolRoute;
    public DetectPlayer DetectPlayer => detectPlayer;
    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float LoseTargetDistance => loseTargetDistance;
    public Transform CurrentTarget { get; private set; }
    public Vector3 LastKnownTargetPosition { get; private set; }
    public EnemyState CurrentState { get; private set; }

    public PatrolState PatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public AttackState AttackState { get; private set; }


    private void Awake()
    {
        LookForMissingReferences();
        CreateStates();
    }

    private void Start()
    {
        if (startInPatrol)
        {
            ChangeState(PatrolState);
        }
        else
        {
            ChangeState(ChaseState);
        }
    }
    void LookForMissingReferences()
    {
        if (guardMovement == null)
        {
            guardMovement = GetComponent<GuardMovement>();
        }
        if (patrolRoute == null)
        {
            patrolRoute = GetComponent<PatrolRoute>();
        }
        if(detectPlayer == null)
        {
            detectPlayer = GetComponent<DetectPlayer>();
        }
    }

    private void CreateStates()
    {
        PatrolState = new PatrolState(this);
        ChaseState = new ChaseState(this);
        AttackState = new AttackState(this);
    }
    public void ChangeState(EnemyState newState)
    {
        if(newState == null)
        {
            Debug.LogWarning(
               $"{name} forsøgte at skifte til en null state.",
               this);
            return;
        }
        if(newState == CurrentState)
        {
            return;
        }

        CurrentState?.Exit();

        CurrentState = newState;
        currentStateName = CurrentState.GetType().Name;

        CurrentState.Enter();
    }

    public void SetCurrentTarget(Transform target)
    {
        CurrentTarget = target;
        if (target != null)
        {
            LastKnownTargetPosition = target.position;
        }
    }
    public void ClearCurrentTarget()
    {
        CurrentTarget = null;
    }

    public bool IsCurrentTargetToFarAway()
    {
        if (CurrentTarget == null)
        {
            return false;
        }
        float distanceTotarget = Vector3.Distance(transform.position, CurrentTarget.position);
        return distanceTotarget > loseTargetDistance;
    }
}