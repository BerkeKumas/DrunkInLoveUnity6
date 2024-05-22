using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public bool startEnemy = false;
    public bool isPlayerHiding = false;
    public bool waitOnPlayer = false;
    public bool canSearchPlayer = true;
    public bool checkPlayerDistance = false;

    [SerializeField] private GameObject player;
    [SerializeField] private BreathControl breathControl;
    [SerializeField] private Transform[] waypoints;

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private bool returningToWaypoints = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!startEnemy) return;

        if (checkPlayerDistance && Vector3.Distance(player.transform.position, transform.position) <= 3f)
        {
            checkPlayerDistance = false;
            breathControl.enabled = true;
        }

        if (canSearchPlayer && Vector3.Distance(player.transform.position, transform.position) <= 30.0f)
        {
            if (!isPlayerHiding)
            {
                agent.speed = 7;
                agent.SetDestination(player.transform.position);
                returningToWaypoints = false;
            }
            else if (waitOnPlayer)
            {
                waitOnPlayer = false;

                Transform lockerPos = player.GetComponent<ObjectInteractions>().lockerObject.transform;
                Vector3 targetPosition = lockerPos.position + lockerPos.forward * 2.0f;
                agent.SetDestination(targetPosition);

                checkPlayerDistance = true;
                returningToWaypoints = false;
            }
        }
        else
        {
            agent.speed = 4;
            if (!returningToWaypoints)
            {
                FindNearestWaypoint();
                returningToWaypoints = true;
            }
            else
            {
                MoveToNextWaypoint();
            }
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
        animator.SetBool("IsMoving", agent.velocity.magnitude > 0);
    }

    private void FindNearestWaypoint()
    {
        float shortestDistance = Mathf.Infinity;
        int nearestWaypointIndex = 0;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[i].position);
            if (distanceToWaypoint < shortestDistance)
            {
                shortestDistance = distanceToWaypoint;
                nearestWaypointIndex = i;
            }
        }

        currentWaypointIndex = nearestWaypointIndex;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private void MoveToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}
