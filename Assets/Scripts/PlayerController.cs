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

    public bool CanMove = true;

    private float moveSpeed = WALK_SPEED;
    private bool isCrouching = false;
    private bool shouldChangeStandSpeed = false;
    private PlayerInputActions playerInputActions;
    private AudioSource walkingSound;
    private Coroutine currentCoroutine;

    private void Awake()
    {
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
            SwitchCoroutine(Crouch());
        }
        else if (!shouldCrouch && isCrouching)
        {
            SwitchCoroutine(Stand());
        }

        shouldChangeStandSpeed = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftControl);
        if (!isCrouching && shouldChangeStandSpeed)
        {
            shouldChangeStandSpeed = false;
            StartCoroutine(ManageStandSpeed());
        }

        MovePlayer();
    }

    private void SwitchCoroutine(IEnumerator newCoroutine)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(newCoroutine);
    }

    private IEnumerator Crouch()
    {
        isCrouching = true;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(transform.localScale.x, CROUCH_SCALE_Y, transform.localScale.z);
        float startSpeed = moveSpeed;
        float targetSpeed = CROUCH_SPEED;

        float elapsedTime = 0.0f;

        while (elapsedTime < STAND_CROUCH_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / STAND_CROUCH_ANIMATION_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            moveSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            yield return null;
        }
        transform.localScale = targetScale;
        moveSpeed = CROUCH_SPEED;

        isCrouching = false;
    }

    private IEnumerator Stand()
    {
        isCrouching = false;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(transform.localScale.x, STAND_SCALE_Y, transform.localScale.z);

        float elapsedTime = 0.0f;

        while (elapsedTime < STAND_CROUCH_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / STAND_CROUCH_ANIMATION_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        transform.localScale = targetScale;
    }

    private IEnumerator ManageStandSpeed()
    {
        float startSpeed = moveSpeed;
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? RUN_SPEED : WALK_SPEED;

        float elapsedTime = 0.0f;

        while (elapsedTime < STAND_MOVE_SPEED_CHANGE_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / STAND_MOVE_SPEED_CHANGE_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            moveSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            yield return null;
        }
        moveSpeed = targetSpeed;
    }

    private void MovePlayer()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
        Vector3 movement = moveSpeed * Time.deltaTime * (transform.forward * inputVector.y + transform.right * inputVector.x);
        transform.position += movement;

        bool isWalking = inputVector != Vector2.zero;
        UpdateWalkingSound(isWalking);
    }

    private void UpdateWalkingSound(bool isWalking)
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
}
