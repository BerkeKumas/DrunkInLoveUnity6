using System.Collections;
using UnityEngine;

public class DrawerController : MonoBehaviour
{
    private const float DRAWER_OPENING_DISTANCE = 0.5f;
    private const float ANIMATION_TIME = 0.7f;

    private bool isDrawerOpen = false;
    private bool enableControl = true;
    private Vector3 closePos;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        closePos = transform.localPosition;
    }

    public void ToggleDrawer()
    {
        if (!enableControl) return;

        enableControl = false;
        StartCoroutine(ControlDrawer());
        isDrawerOpen = !isDrawerOpen;
    }

    private IEnumerator ControlDrawer()
    {
        audioSource.Play();
        float elapsedTime = 0.0f;
        Vector3 startPos = transform.localPosition;

        Vector3 newDrawrPos = isDrawerOpen ? new Vector3(startPos.x, startPos.y, closePos.z) : new Vector3(startPos.x, startPos.y, DRAWER_OPENING_DISTANCE);

        while (elapsedTime < ANIMATION_TIME)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / ANIMATION_TIME;
            t = Mathf.SmoothStep(0, 1, t);
            transform.localPosition = Vector3.Lerp(startPos, newDrawrPos, t);
            yield return null;
        }

        transform.localPosition = newDrawrPos;
        enableControl = true;
    }
}
