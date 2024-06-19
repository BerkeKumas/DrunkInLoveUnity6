using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    private readonly string[] hideTexts =
    {
        "She's coming, I must hide now",
        "Footsteps approaching, I need to run",
        "She's near, I must hide quickly",
        "Footsteps close, I must hide fast",
        "She's close, I need to find a spot",
        "She heard me, I must hide",
    };
    private const float RAYCAST_HEIGHT_OFFSET = 1.0f;

    public bool isHeardSound = false;
    public bool startEnemy = false;
    public bool isPlayerHiding = false;
    public bool waitOnLocker = false;
    public bool canSearchPlayer = true;
    public bool isPaused = false;
    public NavMeshAgent agent;

    public float visionRadius = 15f;
    public float visionAngle = 30f;

    [SerializeField] private AudioClip[] walkSounds;
    [SerializeField] private GameObject player;
    [SerializeField] private BreathControl breathControl;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform[] houseWaypoints;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private AudioSource breathSound;
    [SerializeField] private AudioSource slowBreathSound;
    [SerializeField] private CaptionTextTyper captionTextTyper;
    [SerializeField] private AudioSource otherAudioSource;
    [SerializeField] private AudioSource voiceAudioSource;
    [SerializeField] private AudioClip[] otherAudioClips;
    [SerializeField] private AudioClip[] voiceAudioClips;
    [SerializeField] private Transform shutterDoorDestination;
    [SerializeField] private ShutterControl shutterControl;
    [SerializeField] private VolumeProfile basementProfile;
    [SerializeField] private AudioSource screamSource;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Image fadeImage;

    private int currentWaypointIndex = 0;
    private int houseCurrentWaypointIndex = 0;
    private Animator animator;
    private bool returningToWaypoints = false;
    private bool checkLockerDistance = false;
    private bool enableHouseWaypoints = true;
    private bool isSeen = false;
    private AudioSource audioSource;
    private Coroutine houseActionOrganizerCoroutine;
    private Coroutine houseMoveCoroutine;
    private Coroutine slowBreathSoundCoroutine;
    private Coroutine basementActionOrganizerCoroutine;
    private Coroutine randomSounds;
    private float hideDistance;
    private FilmGrain filmGrain;
    private bool wasOnVision = false;
    private bool enableCatchAnim = true;
    private int newRandomVoice = 0;
    private int randomVoice = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
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
        if (enableCatchAnim)
        {
            bool isNear;
            if (isPlayerHiding || Vector3.Distance(player.transform.position, transform.position) >= 25.0f)
            {
                isNear = false;
            }
            else
            {
                isNear = true;
            }
            animator.SetBool("IsPlayerNear", isNear);
        }

        if (!startEnemy) return;
        if (isPaused) return;

        if (checkLockerDistance && Vector3.Distance(player.transform.position, transform.position) <= 3.0f && agent.velocity == Vector3.zero)
        {
            checkLockerDistance = false;
            canSearchPlayer = false;
            breathControl.enabled = true;
            isSeen = false;
            isHeardSound = false;
            StartCoroutine(RotateTowards(transform, player.transform.position));
        }

        if (!isPlayerHiding)
        {
            hideDistance = Vector3.Distance(player.transform.position, transform.position);
        }

        if (!canSearchPlayer) return;

        if (isSeen || isHeardSound)
        {
            if (randomSounds != null)
            {
                StopCoroutine(randomSounds);
                randomSounds = null;
            }
            ChasePlayer();
        }
        else if (CheckVisionCone() && !isPlayerHiding)
        {
            isSeen = true;
        }
        else if (playerController.rbMovement.magnitude > 0.15f && Vector3.Distance(player.transform.position, transform.position) < 35.0f && !isPlayerHiding)
        {
            isHeardSound = true;
        }
        else
        {
            if (randomSounds == null)
            {
                randomSounds = StartCoroutine(PlayRandomSound());
            }
            if (breathSound.isPlaying)
            {
                breathSound.Stop();
                breathSound.loop = false;
                if (slowBreathSound.volume != 0)
                {
                    slowBreathSound.volume = 0.9f;
                    slowBreathSoundCoroutine = StartCoroutine(PlaySlowBreath());
                }
            }
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
    }

    private void ChasePlayer()
    {
        PlayBreathSound();

        if (!isPlayerHiding)
        {
            agent.speed = 10;
            agent.SetDestination(player.transform.position + player.transform.forward);
            returningToWaypoints = false;
            if (Vector3.Distance(player.transform.position, transform.position) < 5.0f)
            {
                KillPlayer();
            }
            else if (CheckVisionCone())
            {
                wasOnVision = true;
            }
            else
            {
                wasOnVision = false;
            }
        }
        else
        {
            if (hideDistance <= 15.0f)
            {
                OpenAndKillPlayer();
            }
            else if (wasOnVision)
            {
                if (Vector3.Distance(player.transform.position, transform.position) < 4.0f)
                {
                    OpenAndKillPlayer();
                }
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
    }

    private void PlayBreathSound()
    {
        if (!breathSound.isPlaying)
        {
            if (slowBreathSound.isPlaying)
            {
                StopCoroutine(slowBreathSoundCoroutine);
                slowBreathSound.Stop();
            }
            string hideText = hideTexts[Random.Range(0, hideTexts.Length)];
            captionTextTyper.StartType(hideText, false);
            breathSound.pitch = 0.9f;
            breathSound.loop = true;
            breathSound.Play();
        }
    }

    public void OpenAndKillPlayer()
    {
        enableCatchAnim = false;
        StartCoroutine(OpenAndKill());
    }

    private IEnumerator OpenAndKill()
    {
        StartCoroutine(PlayerToEnemy());
        player.GetComponent<ObjectInteractions>().lockerObject.GetComponent<LockerHide>().OpenDoor();
        yield return new WaitForSeconds(0.2f);
        KillPlayer();
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsPlayerNear", true);
    }

    public void KillPlayer()
    {
        playerLook.enabled = false;
        playerController.enabled = false;

        StartCoroutine(DeathActions());
    }

    private IEnumerator DeathActions()
    {
        isPaused = true;
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.GetComponent<CapsuleCollider>().enabled = false;
        agent.updateRotation = false;
        agent.isStopped = true;
        impulseSource.GenerateImpulse();
        StartCoroutine(RotateTowards(player.transform, transform.position));
        if (basementProfile.TryGet<FilmGrain>(out filmGrain))
        {
            StartCoroutine(ChangeFilmGrainValues(0.5f));
        }
        StartCoroutine(FadeRed());
        yield return new WaitForSeconds(0.2f);
        screamSource.Play();
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(FadeBlack());
        yield return new WaitForSeconds(1.0f);
        PlayerPrefs.SetInt("isDeadByEnemy", 1);
        SceneManager.LoadScene(2);

        yield return null;
    }

    private IEnumerator PlayerToEnemy()
    {
        player.GetComponent<CapsuleCollider>().enabled = false;

        float elapsedTime = 0f;
        float totalTime = 1.0f;

        Vector3 initialPos = player.transform.position;
        Vector3 targetPos = transform.position + transform.forward * 0.5f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalTime;
            player.transform.position = Vector3.Lerp(initialPos, targetPos, t);
            yield return null;
        }
        player.transform.position = targetPos;
    }

    private IEnumerator FadeBlack()
    {
        float elapsedTime = 0f;
        float totalTime = 1.0f;
        Color initialColor = fadeImage.color;
        Color targetColor = Color.black;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalTime;
            fadeImage.color = Color.Lerp(initialColor, targetColor, t);
            yield return null;
        }
    }

    private IEnumerator FadeRed()
    {
        float elapsedTime = 0f;
        float totalTime = 0.6f;
        Color initialColor = fadeImage.color;
        Color targetColor = new Color(0.5f, 0f, 0f, 0.7f);

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalTime;
            fadeImage.color = Color.Lerp(initialColor, targetColor, t);
            yield return null;
        }
    }

    private IEnumerator ChangeFilmGrainValues(float duration)
    {
        float elapsedTime = 0f;

        float initialIntensity = filmGrain.intensity.value;
        float initialResponse = filmGrain.response.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            filmGrain.intensity.value = Mathf.Lerp(initialIntensity, 1f, t);
            filmGrain.response.value = Mathf.Lerp(initialResponse, 0f, t);

            yield return null;
        }

        filmGrain.intensity.value = 1f;
        filmGrain.response.value = 0f;
    }

    private IEnumerator RotateTowards(Transform from, Vector3 toPosition)
    {
        Quaternion initialRotation = from.rotation;
        Vector3 directionToWoman = toPosition - from.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToWoman);
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            from.rotation = Quaternion.Lerp(initialRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        from.rotation = targetRotation;
    }

    private bool CheckVisionCone()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, visionRadius, targetMask);
        Vector3 rayOrigin = transform.position + Vector3.up * RAYCAST_HEIGHT_OFFSET;

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

        Vector3 rayOrigin = transform.position + Vector3.up * RAYCAST_HEIGHT_OFFSET;
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

    private IEnumerator PlayRandomSound()
    {
        while (true)
        {
            float randomWaitTime = Random.Range(10f, 20f);
            yield return new WaitForSeconds(randomWaitTime);

            do
            {
                randomVoice = Random.Range(4, 9);
            } while (randomVoice == newRandomVoice);

            newRandomVoice = randomVoice;
            PlayVoiceClip(newRandomVoice);
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

    private IEnumerator PlaySlowBreath()
    {
        for (int i = 0; i < 3; i++)
        {
            slowBreathSound.Play();
            slowBreathSound.volume -= 0.2f;
            yield return new WaitForSeconds(slowBreathSound.clip.length);
        }
    }

    private void MoveToNextWaypoint()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    public void EnterHome()
    {
        houseActionOrganizerCoroutine = StartCoroutine(HouseActionOrganizer());
    }

    private IEnumerator HouseActionOrganizer()
    {
        yield return StartCoroutine(WaitForSecondsWithPause(3.0f));
        PlayClip(0);
        yield return StartCoroutine(WaitForSecondsWithPause(2.0f));
        PlayVoiceClip(0);
        captionTextTyper.StartType("Crap, she's back!", false);
        yield return StartCoroutine(WaitForSecondsWithPause(2.0f));
        houseMoveCoroutine = StartCoroutine(HouseMoveToNextWaypoint());
        yield return StartCoroutine(WaitForSecondsWithPause(6.0f));
        PlayVoiceClip(1);
        yield return StartCoroutine(WaitForSecondsWithPause(4.0f));
        captionTextTyper.StartType("I need to find a way out before she finds me.", false);
    }

    private void PlayClip(int index)
    {
        otherAudioSource.clip = otherAudioClips[index];
        otherAudioSource.Play();
    }

    private void PlayVoiceClip(int index)
    {
        voiceAudioSource.clip = voiceAudioClips[index];
        voiceAudioSource.Play();
    }

    private IEnumerator HouseMoveToNextWaypoint()
    {
        while (enableHouseWaypoints)
        {
            agent.SetDestination(houseWaypoints[houseCurrentWaypointIndex].position);

            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            yield return new WaitForSeconds(5.0f);

            houseCurrentWaypointIndex = (houseCurrentWaypointIndex + 1) % houseWaypoints.Length;
        }
    }

    public void EnterBasement()
    {
        enableHouseWaypoints = false;
        if (houseActionOrganizerCoroutine != null)
        {
            StopCoroutine(houseActionOrganizerCoroutine);
        }
        if (houseMoveCoroutine != null)
        {
            StopCoroutine(houseMoveCoroutine);
        }
        basementActionOrganizerCoroutine = StartCoroutine(BasementActionOrganizer());
    }

    private IEnumerator BasementActionOrganizer()
    {
        agent.SetDestination(shutterDoorDestination.position);
        yield return StartCoroutine(WaitForSecondsWithPause(5.0f));
        PlayVoiceClip(2);
        yield return StartCoroutine(WaitForSecondsWithPause(5.0f));
        PlayClip(1);
        yield return StartCoroutine(WaitForSecondsWithPause(3.0f));
        PlayClip(2);
        captionTextTyper.StartType("Did she just knock? She can't get in, right?", false);
        yield return StartCoroutine(WaitForSecondsWithPause(6.0f));
        PlayClip(3);
        yield return StartCoroutine(WaitForSecondsWithPause(3.0f));
        PlayClip(4);
        yield return StartCoroutine(WaitForSecondsWithPause(2.0f));
        PlayVoiceClip(3);
        yield return StartCoroutine(WaitForSecondsWithPause(4.0f));
        PlayClip(5);
        captionTextTyper.StartType("Oh no, she's trying to break in!", false);
        yield return StartCoroutine(WaitForSecondsWithPause(6.0f));
        PlayClip(6);
        yield return StartCoroutine(WaitForSecondsWithPause(3.0f));
        shutterControl.ToggleShutter(2.0f, 1);
        yield return StartCoroutine(WaitForSecondsWithPause(0.5f));
        captionTextTyper.StartType("She's inside!", false);
        startEnemy = true;
        yield return StartCoroutine(WaitForSecondsWithPause(2.0f));
        PlayVoiceClip(4);
    }

    private IEnumerator WaitForSecondsWithPause(float seconds)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < seconds)
        {
            if (!isPaused)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
    }

    public void ResetControls()
    {
        canSearchPlayer = true;
        isSeen = false;
        isHeardSound = false;
    }
}
