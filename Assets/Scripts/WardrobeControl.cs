using System.Collections;
using UnityEngine;

public class WardrobeControl : MonoBehaviour
{
    [SerializeField] private GameObject leftSide;
    [SerializeField] private GameObject rightSide;

    private bool isOpen = false;
    private bool isRotating = false;
    private Quaternion leftStartRotation;
    private Quaternion rightStartRotation;
    private Quaternion leftEndRotation;
    private Quaternion rightEndRotation;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ToggleDoors()
    {
        if (isRotating) return;
        isRotating = true;
        isOpen = !isOpen;
        audioSource.Play();
        leftStartRotation = leftSide.transform.rotation;
        rightStartRotation = rightSide.transform.rotation;
        leftEndRotation = isOpen ? leftStartRotation * Quaternion.Euler(0, 90, 0) : leftStartRotation * Quaternion.Euler(0, -90, 0);
        rightEndRotation = isOpen ? rightStartRotation * Quaternion.Euler(0, -90, 0) : rightStartRotation * Quaternion.Euler(0, 90, 0);
        StartCoroutine(RotateDoor(leftEndRotation, rightEndRotation));
    }

    private IEnumerator RotateDoor(Quaternion leftAngle, Quaternion rightAngle)
    {
        float elapsedTime = 0;

        while (elapsedTime < 1.0f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 1.0f;
            t = Mathf.SmoothStep(0, 1, t);
            leftSide.transform.rotation = Quaternion.Lerp(leftStartRotation, leftAngle, t);
            rightSide.transform.rotation = Quaternion.Lerp(rightStartRotation, rightAngle, t);
            yield return null;
        }
        leftSide.transform.rotation = leftEndRotation;
        rightSide.transform.rotation = rightEndRotation;
        isRotating = false;
    }
}
