using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private const float WANDER_RADIUS = 15.0f;
    private const float WANDER_COOLDOWN = 2.0f;

    public bool startEnemy = false;
    public bool isPlayerHiding = false;
    public bool waitOnPlayer = false;
    public bool canSearchPlayer = true;

    [SerializeField] private GameObject player;
    [SerializeField] private BreathControl breathControl;

    private float wanderTimer = 0.0f;
    private bool checkPlayerDistance = false;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!startEnemy) return;

        if (checkPlayerDistance && Vector3.Distance(player.transform.position, transform.position) <= 2.5f)
        {
            checkPlayerDistance = false;

            breathControl.ToggleBreathControl();
        }
        if (canSearchPlayer && Vector3.Distance(player.transform.position, transform.position) <= 20.0f)
        {
            if (!isPlayerHiding)
            {
                agent.SetDestination(player.transform.position);
            }
            else if (waitOnPlayer)
            {
                waitOnPlayer = false;

                Transform lockerPos = player.GetComponent<ObjectInteractions>().lockerObject.transform;
                Vector3 targetPosition = lockerPos.position + lockerPos.forward * 2.0f;

                agent.SetDestination(targetPosition);

                checkPlayerDistance = true;
            }
        }
        else
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= WANDER_COOLDOWN)
            {
                Vector3 randomDestination = GetRandomPoint(transform.position, WANDER_RADIUS);
                agent.SetDestination(randomDestination);
                wanderTimer = 0.0f;
            }
        }
    }

    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
    }
}
