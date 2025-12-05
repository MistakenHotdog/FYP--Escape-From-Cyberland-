using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public Transform[] patrolPoints;
    public float chaseDistance = 6f;
    public float attackDistance = 1.6f;
    public float idleTime = 2f;

    [Header("References")]
    public Animator animator;

    [HideInInspector] public StateMachine StateMachine;
    [HideInInspector] public NavMeshAgent Agent;

    private Transform player;

    // States
    [HideInInspector] public IdleState idleState;
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public AttackState attackState;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        StateMachine = new StateMachine();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Create states
        idleState = new IdleState(this, idleTime);
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
    }

    private void Start()
    {
        StateMachine.Initialize(idleState);
    }

    private void Update()
    {
        StateMachine.Tick();
    }

    public Transform GetPlayer() => player;
}
