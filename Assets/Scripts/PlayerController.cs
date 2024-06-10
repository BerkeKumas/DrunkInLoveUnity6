using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    private const float WALK_SPEED = 8.0f;
    private const float RUN_SPEED = 12.0f;
    private const float CROUCH_SPEED = 5.0f;
    private const float STAND_SCALE_Y = 1.0f;
    private const float CROUCH_SCALE_Y = 0.5f;
    private const float STAND_CROUCH_ANIMATION_DURATION = 0.25f;
    private const float STAND_MOVE_SPEED_CHANGE_DURATION = 1.0f;
    private const float CAPSULE_HEIGHT = 4.0f;

    public bool CanMove = false;
    public Vector3 rbMovement;

    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private GameObject playerCamera;

    private float targetStandSpeed;
    private float moveSpeed = WALK_SPEED;
    private bool isCrouching = false;
    private bool shouldChangeStandSpeed = false;
    private PlayerInputActions playerInputActions;
    private AudioSource walkingSound;
    private Coroutine currentCoroutine;
    private Coroutine currentStandCoroutine;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        walkingSound = GetComponent<AudioSource>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    private void Update()
    {
        if (!CanMove) return;

        bool shouldCrouch = Input.GetKey(KeyCode.LeftControl);
        if (shouldCrouch && !isCrouching)
        {
            if (currentStandCoroutine != null)
                StopCoroutine(currentStandCoroutine);
            SwitchCoroutine(Crouch());
        }
        else if (!shouldCrouch && isCrouching)
        {
            SwitchCoroutine(Stand());
        }

        shouldChangeStandSpeed = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftControl);
        targetStandSpeed = Input.GetKey(KeyCode.LeftShift) ? RUN_SPEED : WALK_SPEED;
        if (!isCrouching && shouldChangeStandSpeed)
        {
            shouldChangeStandSpeed = false;
            SwitchStandCoroutine(ManageStandSpeed());
        }
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;

        MovePlayer();
    }

    private void SwitchStandCoroutine(IEnumerator newCoroutine)
    {
        if (currentStandCoroutine != null)
            StopCoroutine(currentStandCoroutine);
        currentStandCoroutine = StartCoroutine(newCoroutine);
    }

    private void SwitchCoroutine(IEnumerator newCoroutine)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(newCoroutine);
    }

    private IEnumerator ManageStandSpeed()
    {
        float startSpeed = moveSpeed;

        float elapsedTime = 0.0f;

        while (elapsedTime < STAND_MOVE_SPEED_CHANGE_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / STAND_MOVE_SPEED_CHANGE_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            moveSpeed = Mathf.Lerp(startSpeed, targetStandSpeed, t);
            yield return null;
        }
        moveSpeed = targetStandSpeed;
    }

    private IEnumerator Crouch()
    {
        isCrouching = true;
        walkingSound.volume = 0.35f;

        float startHeight = capsuleCollider.height;
        float targetHeight = CAPSULE_HEIGHT * CROUCH_SCALE_Y;
        float cameraY = playerCamera.transform.localPosition.y;
        float targetCameraY = cameraY - (startHeight - targetHeight);

        float startSpeed = moveSpeed;
        float targetSpeed = CROUCH_SPEED;

        float elapsedTime = 0.0f;

        while (elapsedTime < STAND_CROUCH_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / STAND_CROUCH_ANIMATION_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            capsuleCollider.height = Mathf.Lerp(startHeight, targetHeight, t);

            float newPlayerCameraY = Mathf.Lerp(cameraY, targetCameraY, t);
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, newPlayerCameraY, playerCamera.transform.localPosition.z);

            float newCapsuleCenterY = Mathf.Lerp(startHeight / 2.0f, targetHeight / 2.0f, t);
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, newCapsuleCenterY, capsuleCollider.center.z);

            moveSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            yield return null;
        }
        capsuleCollider.height = targetHeight;
        playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, targetCameraY, playerCamera.transform.localPosition.z);
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, targetHeight / 2.0f, capsuleCollider.center.z);
        moveSpeed = CROUCH_SPEED;
    }

    private IEnumerator Stand()
    {
        isCrouching = false;
        walkingSound.volume = 1.0f;

        float startHeight = capsuleCollider.height;
        float targetHeight = CAPSULE_HEIGHT * STAND_SCALE_Y;
        float cameraY = playerCamera.transform.localPosition.y;
        float targetCameraY = cameraY + (targetHeight - startHeight);

        float elapsedTime = 0.0f;

        while (elapsedTime < STAND_CROUCH_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / STAND_CROUCH_ANIMATION_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            capsuleCollider.height = Mathf.Lerp(startHeight, targetHeight, t);

            float newPlayerCameraY = Mathf.Lerp(cameraY, targetCameraY, t);
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, newPlayerCameraY, playerCamera.transform.localPosition.z);

            float newCapsuleCenterY = Mathf.Lerp(startHeight / 2.0f, targetHeight / 2.0f, t);
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, newCapsuleCenterY, capsuleCollider.center.z);

            yield return null;
        }
        capsuleCollider.height = targetHeight;
        playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, targetCameraY, playerCamera.transform.localPosition.z);
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, targetHeight / 2.0f, capsuleCollider.center.z);
    }

    private void MovePlayer()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
        rbMovement = moveSpeed * Time.deltaTime * (transform.forward * inputVector.y + transform.right * inputVector.x);

        Vector3 rayOrigin = transform.position + Vector3.up * 1.0f;
        Vector3 movementDirection = rbMovement.normalized;

        float sphereRadius = 0.75f;
        int rayCount = 8;
        bool isCollisionDetected = false;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (360f / rayCount) * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, sphereRadius, interactableLayers))
            {
                isCollisionDetected = true;
                Vector3 hitNormal = hit.normal;

                float angleToNormal = Vector3.Angle(movementDirection, hitNormal);

                if (angleToNormal < 90.0f || angleToNormal > 270.0f)
                {
                    rb.MovePosition(rb.position + rbMovement);
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        if (!isCollisionDetected)
        {
            rb.MovePosition(rb.position + rbMovement);
        }

        bool isWalking = inputVector != Vector2.zero;
        UpdateWalkingSound(isWalking);
    }

    public void UpdateWalkingSound(bool isWalking)
    {
        if (isWalking)
        {
            if (!walkingSound.isPlaying)
            {
                walkingSound.Play();
            }
        }
        else if (walkingSound.isPlaying)
        {
            walkingSound.Stop();
        }
    }

    public void EnableMovement()
    {
        CanMove = true;
    }
}
