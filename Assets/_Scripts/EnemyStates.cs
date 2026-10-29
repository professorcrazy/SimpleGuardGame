using UnityEngine;
using UnityEngine.AI;

public class EnemyStates : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GuardMovement guardMovement;
    [SerializeField] private PatrolRoute patrolRoute;
    [SerializeField] private DetectPlayer detectPlayer;
    [SerializeField] private EnemyCombat enemyCombat;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;

    [Header("Search")]
    [SerializeField] private float searchDuration = 8f;
    [SerializeField] private float searchRadius = 6f;
    [SerializeField] private int searchPointCount = 5;
    [SerializeField] private int searchSampleAttemptsPerPoint = 10;
    [SerializeField] private int searchNavMeshAreaMask = NavMesh.AllAreas;

    [Header("Debug")]
    [SerializeField] private string currentStateName;

    public GuardMovement Movement => guardMovement;
    public PatrolRoute PatrolRoute => patrolRoute;
    public DetectPlayer DetectPlayer => detectPlayer;
    public EnemyCombat Combat => enemyCombat;

    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;

    public float SearchDuration => searchDuration;
    public float SearchRadius => searchRadius;
    public int SearchPointCount => searchPointCount;
    public int SearchSampleAttemptsPerPoint => searchSampleAttemptsPerPoint;
    public int SearchNavMeshAreaMask => searchNavMeshAreaMask;

    public Transform CurrentTarget { get; private set; }
    public Vector3 LastSeenPosition { get; private set; }
    public bool HasLastSeenPosition { get; private set; }

    public EnemyState CurrentState { get; private set; }

    public PatrolState PatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public GoToLastSeenPositionState GoToLastSeenPositionState { get; private set; }
    public SearchState SearchState { get; private set; }
    public AttackState AttackState { get; private set; }

    private void Awake()
    {
        FindMissingComponents();
        CreateStates();
    }

    private void Start()
    {
        ChangeState(PatrolState);
    }

    private void Update()
    {
        CurrentState?.Tick();
    }

    private void FindMissingComponents()
    {
        if (guardMovement == null)
            guardMovement = GetComponent<GuardMovement>();

        if (patrolRoute == null)
            patrolRoute = GetComponent<PatrolRoute>();

        if (detectPlayer == null)
            detectPlayer = GetComponent<DetectPlayer>();

        if (enemyCombat == null)
            enemyCombat = GetComponent<EnemyCombat>();

        if (guardMovement == null)
            Debug.LogError($"{name} mangler GuardMovement.", this);

        if (patrolRoute == null)
            Debug.LogError($"{name} mangler PatrolRoute.", this);

        if (detectPlayer == null)
            Debug.LogError($"{name} mangler DetectPlayer.", this);
    }

    private void CreateStates()
    {
        PatrolState = new PatrolState(this);
        ChaseState = new ChaseState(this);
        GoToLastSeenPositionState =
            new GoToLastSeenPositionState(this);
        SearchState = new SearchState(this);
        AttackState = new AttackState(this);
    }

    public void ChangeState(EnemyState newState)
    {
        if (newState == null)
        {
            Debug.LogError(
                $"{name} forsøgte at skifte til null state.",
                this);
            return;
        }

        if (newState == CurrentState)
            return;

        CurrentState?.Exit();

        CurrentState = newState;
        currentStateName = CurrentState.GetType().Name;
        CurrentState.Enter();
    }

    public void SetCurrentTarget(Transform target)
    {
        CurrentTarget = target;

        if (target == null)
            return;

        UpdateLastSeenPosition();
    }

    public void UpdateLastSeenPosition()
    {
        if (CurrentTarget == null)
            return;

        LastSeenPosition = CurrentTarget.position;
        HasLastSeenPosition = true;
    }

    public void ClearCurrentTarget()
    {
        CurrentTarget = null;
    }

    public void ClearLastSeenPosition()
    {
        HasLastSeenPosition = false;
    }
}