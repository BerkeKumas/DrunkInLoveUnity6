using System.Collections;
using UnityEngine;

public class ShutterDoorTrigger : MonoBehaviour
{
    [SerializeField] private ShutterControl shutterControl;
    [SerializeField] private string triggerTag;
    [SerializeField] private CaptionTextTyper captionTextTyper;
    [SerializeField] private AudioSource slowBreathSound;

    private bool enableTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag) && enableTrigger)
        {
            if (triggerTag == "Player")
            {
                enableTrigger = false;
                shutterControl.ToggleShutter(2.0f, 1);
                StartCoroutine(TypeCaption());
                if (slowBreathSound.volume != 0)
                {
                    slowBreathSound.volume = 0.9f;
                    StartCoroutine(PlaySlowBreath());
                }
            }
            else if (triggerTag == "enemytag")
            {
                shutterControl.ToggleShutter(2.0f, 1);
                Destroy(this.gameObject);
            }
        }
    }

    private IEnumerator TypeCaption()
    {
        yield return new WaitForSeconds(0.5f);
        captionTextTyper.StartType("What? No way, am I trapped here?", false);
    }

    private IEnumerator PlaySlowBreath()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1.0f);
            slowBreathSound.Play();
            slowBreathSound.volume -= 0.1f;
            yield return new WaitForSeconds(slowBreathSound.clip.length);
        }
        Destroy(this.gameObject);
    }
}
