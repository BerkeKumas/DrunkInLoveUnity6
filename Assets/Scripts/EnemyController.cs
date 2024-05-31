using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public bool startEnemy = false;
    public bool isPlayerHiding = false;
    public bool waitOnLocker = false;
    public bool canSearchPlayer = true;

    public float visionRadius = 15f; // Görüþ yarýçapý
    public float visionAngle = 30f; // Görüþ açýsý

    [SerializeField] private AudioClip[] walkSounds;
    [SerializeField] private GameObject player;
    [SerializeField] private BreathControl breathControl;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LayerMask targetMask;
    public float raycastHeightOffset = 2.0f;

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private bool returningToWaypoints = false;
    private bool checkLockerDistance = false;
    private bool isHeardSound = false;
    private bool canHide = false;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!startEnemy) return;

        if (checkLockerDistance && Vector3.Distance(player.transform.position, transform.position) <= 3f)
        {
            checkLockerDistance = false;
            breathControl.enabled = true;
        }

        if (!isHeardSound && playerController.rbMovement.magnitude > 0.15f)
        {
            isHeardSound = true;
        }

        if (Vector3.Distance(player.transform.position, transform.position) >= 15.0f)
        {
            canHide = true;
        }

        if (CheckVisionCone())
        {
            Debug.Log("on vision");
            if (!isPlayerHiding)
            {
                canHide = false;
            }
            else if (!canHide)
            {
                ChasePlayer();
            }
        }
        else if (isHeardSound && canSearchPlayer && Vector3.Distance(player.transform.position, transform.position) <= 30.0f)
        {
            Debug.Log("heard");
            ChasePlayer();
        }
        else
        {
            isHeardSound = false;
            agent.speed = 5;
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

        if (agent.velocity.magnitude > 0.1)
        {
            if (agent.velocity.magnitude > 5.2)
            {
                audioSource.clip = walkSounds[1];
            }
            else
            {
                audioSource.clip = walkSounds[0];
            }
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
        animator.SetBool("IsMoving", agent.velocity.magnitude > 0.1);
    }

    private void ChasePlayer()
    {
        if (!isPlayerHiding)
        {
            agent.speed = 8;
            agent.SetDestination(player.transform.position);
            returningToWaypoints = false;
        }
        else if (waitOnLocker)
        {
            waitOnLocker = false;
            Transform lockerPos = player.GetComponent<ObjectInteractions>().lockerObject.transform;
            Vector3 LockerFrontPos = lockerPos.position + lockerPos.forward * 2.0f;
            agent.SetDestination(LockerFrontPos);

            checkLockerDistance = true;
            returningToWaypoints = false;
        }
    }

    private bool CheckVisionCone()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, visionRadius, targetMask);
        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeightOffset;

        foreach (Collider target in targetsInViewRadius)
        {
            if (target.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector3 targetPosition = new Vector3(target.transform.position.x, rayOrigin.y, target.transform.position.z);
                Vector3 directionToTarget = (targetPosition - rayOrigin).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget < visionAngle / 2)
                {
                    if (Physics.Raycast(rayOrigin, directionToTarget, out RaycastHit hit, visionRadius))
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Vector3 forward = transform.forward * visionRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2, 0) * forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2, 0) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);

        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeightOffset;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, visionRadius, targetMask);

        foreach (Collider target in targetsInViewRadius)
        {
            if (target.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector3 targetPosition = new Vector3(target.transform.position.x, rayOrigin.y, target.transform.position.z);
                Vector3 directionToTarget = (targetPosition - rayOrigin).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget < visionAngle / 2)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(rayOrigin, targetPosition);

                    if (Physics.Raycast(rayOrigin, directionToTarget, out RaycastHit hit, visionRadius))
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(rayOrigin, hit.point);
                        }
                        else
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawLine(rayOrigin, hit.point);
                        }
                    }
                }
            }
        }
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
