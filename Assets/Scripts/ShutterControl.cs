using System.Collections;
using UnityEngine;

public class ShutterControl : MonoBehaviour
{
    private const float OPENING_DISTANCE = -7.5f;

    [SerializeField] private AudioClip[] audioClips;

    private bool isShutterOpen = false;
    private AudioSource audioSource;
    private Vector3 closePos;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        closePos = transform.position;
    }

    public void ToggleShutter(float duration, int audioIndex)
    {
        Vector3 endPos = isShutterOpen ? closePos : closePos + new Vector3(0, OPENING_DISTANCE, 0);
        audioSource.clip = audioClips[audioIndex];
        audioSource.Play();
        StartCoroutine(MoveShutterDoor(endPos, duration));

        isShutterOpen = !isShutterOpen;
    }

    private IEnumerator MoveShutterDoor(Vector3 targetPos, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        transform.position = targetPos;
        audioSource.Stop();
    }
}
