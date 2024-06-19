using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BreathControl : MonoBehaviour
{
    private const float INITIAL_HEART_BEAT_SPEED = 0.5f;
    private const float SPEED_REDUCTION_FACTOR = 1.05f;

    [SerializeField] private GameObject BreathControlUI;
    [SerializeField] private RectTransform heartUI;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Image heartImage;
    [SerializeField] private AudioSource heartBeat;
    [SerializeField] private ObjectInteractions objectInteractions;

    private float currentHeartbeatSpeed;
    private Color originalColor;
    private int countErrors;

    private void OnEnable()
    {
        countErrors = 0;
        originalColor = heartImage.color;
        BreathControlUI.SetActive(true);
        currentHeartbeatSpeed = INITIAL_HEART_BEAT_SPEED;
        StartCoroutine(Heartbeat());
    }

    private IEnumerator Heartbeat()
    {
        while (true)
        {
            yield return ScaleUI(0.3f, 1f, currentHeartbeatSpeed / 2);
            heartBeat.pitch = Mathf.Lerp(1.4f, 0.9f, Mathf.InverseLerp(0.5f, 1.0f, currentHeartbeatSpeed));
            heartBeat.Play();
            yield return ScaleUI(1f, 0.3f, currentHeartbeatSpeed / 2);
        }
    }

    private IEnumerator ScaleUI(float from, float to, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.SmoothStep(from, to, elapsed / duration);
            heartUI.localScale = new Vector3(scale, scale, scale);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (scale >= 0.85f && scale <= 1f)
                {
                    currentHeartbeatSpeed *= SPEED_REDUCTION_FACTOR;
                    if (currentHeartbeatSpeed >= 1.0f)
                    {
                        enemyController.ResetControls();
                        StopAllCoroutines();
                        BreathControlUI.SetActive(false);
                        this.enabled = false;
                    }
                    StartCoroutine(ShowFeedback(Color.green));
                }
                else
                {
                    countErrors++;
                    if (countErrors >= 8)
                    {
                        enemyController.OpenAndKillPlayer();
                        StopAllCoroutines();
                        BreathControlUI.SetActive(false);
                        this.enabled = false;
                    }
                    currentHeartbeatSpeed = Mathf.Max(currentHeartbeatSpeed / SPEED_REDUCTION_FACTOR, INITIAL_HEART_BEAT_SPEED);
                    StartCoroutine(ShowFeedback(Color.red));
                }
            }

            yield return null;
        }
        heartUI.localScale = new Vector3(to, to, to);
    }

    private IEnumerator ShowFeedback(Color feedbackColor)
    {
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            heartImage.color = Color.Lerp(originalColor, feedbackColor, elapsed / duration);
            yield return null;
        }

        heartImage.color = feedbackColor;

        yield return new WaitForSeconds(0.3f);

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            heartImage.color = Color.Lerp(feedbackColor, originalColor, elapsed / duration);
            yield return null;
        }

        heartImage.color = originalColor;
    }
}
