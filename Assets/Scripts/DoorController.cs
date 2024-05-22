using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private const float DOOR_SWING_TIME = 0.7f;

    public bool isDoorLocked = false;

    [SerializeField] private AudioClip[] doorSounds;

    private bool isDoorOpen = false;
    private bool canRotateDoor = true;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.Euler(0, 90f, 0);
    }

    public void ToggleDoor()
    {
        if (!canRotateDoor) return;

        isDoorOpen = !isDoorOpen;
        StartCoroutine(RotateDoor(isDoorOpen));
    }

    private IEnumerator RotateDoor(bool openDoor)
    {
        canRotateDoor = false;
        Quaternion startRotation = gameObject.transform.rotation;
        Quaternion endRotation = openDoor ? openRotation : closedRotation;
        audioSource.clip = openDoor ? doorSounds[0] : doorSounds[1];
        audioSource.Play();

        float time = 0.0f;
        while (time < DOOR_SWING_TIME)
        {
            time += Time.deltaTime;
            float t = time / DOOR_SWING_TIME;
            t = Mathf.SmoothStep(0, 1, t);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
        canRotateDoor = true;
    }

    public bool IsDoorOpen()
    {
        return isDoorOpen;
    }

    public void StartDoorRotation(bool openDoor)
    {
        isDoorOpen = openDoor;
        StartCoroutine(RotateDoor(isDoorOpen));
    }
}
