using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAfterDelay : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PinManager pinManager;
    [SerializeField] private AudioSource backgroundAudio;
    [SerializeField] private HourTimer hourTimer;
    [SerializeField] private DrunkBar drunkBar;
    [SerializeField] private PauseManager pauseManager;

    private bool isTimelineEnded = false;

    private void Update()
    {
        if (!isTimelineEnded) return;

        StartGame();
    }

    private void StartGame()
    {
        playerController.CanMove = true;

        pinManager.IsPinEntryActive = true;
        StartCoroutine(IncreaseBackgroundMusicVolume());
        hourTimer.IsTimerActive = true;
        drunkBar.StartFill = true;
        pauseManager.canPause = true;
        this.enabled = false;
    }

    public void TimelineEnded()
    {
        isTimelineEnded = true;
    }

    private IEnumerator IncreaseBackgroundMusicVolume()
    {
        while (backgroundAudio.volume < 0.3f)
        {
            backgroundAudio.volume += 0.02f;
            yield return new WaitForSeconds(0.1f);
        }
    }

}
