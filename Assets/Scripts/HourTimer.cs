using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class HourTimer : MonoBehaviour
{
    private const int GAME_OVER_SCENE_INDEX = 2;
    private const float SECONDS_TO_WAIT = 3.0f;
    private const float TOTAL_MINUTES = 60.0f;
    private const string TIME_ON_COMPLETE = "08:00";

    public bool IsTimerActive = false;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private AudioSource clockTickingSound;

    private float elapsedMinutes = 0;

    private void OnEnable()
    {
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {

        while (elapsedMinutes < TOTAL_MINUTES)
        {
            if (IsTimerActive)
            {
                UpdateCountdownDisplay(elapsedMinutes);
                elapsedMinutes++;
                clockTickingSound.volume = 0.2f + (elapsedMinutes / TOTAL_MINUTES) * 0.8f;
                if (elapsedMinutes >= 30)
                {
                    clockTickingSound.pitch = 1.0f + 2.0f * (elapsedMinutes - 30) / TOTAL_MINUTES;
                }
            }
            yield return new WaitForSeconds(SECONDS_TO_WAIT);
        }

        OnTimerComplete();
    }

    private void UpdateCountdownDisplay(float elapsedMinutes)
    {
        timerText.text = string.Format("07:{0:00}", elapsedMinutes);
    }

    private void OnTimerComplete()
    {
        timerText.text = TIME_ON_COMPLETE;
        SceneManager.LoadScene(GAME_OVER_SCENE_INDEX);
    }
}
