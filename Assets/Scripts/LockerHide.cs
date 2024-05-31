using System.Collections;
using UnityEngine;

public class LockerHide : MonoBehaviour
{
    private const float DOOR_ANIMATION_DURATION = 0.5f;
    private const float MOVE_PLAYER_DURATION = 1.0f;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform doorTransform;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private MotionCameraEffects motionCameraEffects;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private ObjectInteractions objectInteractions;
    [SerializeField] private InventorySlot handSlot;
    [SerializeField] private AudioClip[] audioClips;

    private bool isPlayerGoingInside = false;
    private BoxCollider boxCollider;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (isPlayerGoingInside && Input.GetKeyDown(KeyCode.F))
        {
            ControlPlayerHide();
        }
    }

    public void ControlPlayerHide()
    {
        isPlayerGoingInside = !isPlayerGoingInside;

        if (isPlayerGoingInside)
        {
            objectInteractions.enabled = false;
            if (handSlot.itemGameObject != null)
            {
                handSlot.itemGameObject.SetActive(false);
            }
        }

        StartCoroutine(OpenCloseDoorRoutine());
        if (isPlayerGoingInside)
        {
            enemyController.isPlayerHiding = true;

            boxCollider.enabled = false;
            playerController.UpdateWalkingSound(false);
            playerController.enabled = false;
            playerLook.startingRotation = playerLook.startingRotation + 180.0f;
            motionCameraEffects.isEffectActive = false;
            StartCoroutine(TogglePlayerInside(transform.position, transform.rotation));
        }
        else
        {
            enemyController.isPlayerHiding = false;

            playerController.enabled = true;
            motionCameraEffects.isEffectActive = true;
            StartCoroutine(TogglePlayerInside(transform.position + transform.forward, transform.rotation));
        }

        if (!isPlayerGoingInside)
        {
            if (handSlot.itemGameObject != null)
            {
                handSlot.itemGameObject.SetActive(true);
            }
            objectInteractions.enabled = true;
        }
    }

    private IEnumerator OpenCloseDoorRoutine()
    {
        audioSource.clip = audioClips[0];
        audioSource.Play();
        yield return StartCoroutine(RotateDoor(90));
        yield return new WaitForSeconds(0.5f);
        audioSource.clip = audioClips[1];
        audioSource.Play();
        yield return StartCoroutine(RotateDoor(-90));
    }

    private IEnumerator RotateDoor(float targetAngle)
    {
        float elapsedTime = 0;
        Quaternion startRotation = doorTransform.localRotation;
        Quaternion endRotation = Quaternion.Euler(doorTransform.localEulerAngles.x, doorTransform.localEulerAngles.y, targetAngle);

        while (elapsedTime < DOOR_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / DOOR_ANIMATION_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            doorTransform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }
    }

    private IEnumerator TogglePlayerInside(Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 startPosition = playerTransform.position;
        Quaternion startRotation = playerTransform.rotation;

        float elapsedTime = 0;

        while (elapsedTime < MOVE_PLAYER_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / MOVE_PLAYER_DURATION;
            t = Mathf.SmoothStep(0, 1, t);

            playerTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            playerTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        playerTransform.position = targetPosition;
        playerTransform.rotation = targetRotation;
        enemyController.waitOnLocker = true;

        if (!isPlayerGoingInside)
        {
            boxCollider.enabled = true;
            yield return new WaitForSeconds(0.5f);
            enemyController.canSearchPlayer = true;
        }
    }
}
