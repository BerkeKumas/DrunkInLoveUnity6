using System.Collections;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI.Table;

public class TextTyper : MonoBehaviour
{
    private const float DELAY = 0.04f;
    private const float END_DELAY_FACTOR = 10.0f;

    [SerializeField] private AudioSource clearThroatSource;
    [SerializeField] private AudioSource phoneSource;
    [SerializeField] private AudioSource timelineSource;
    [SerializeField] private AudioClip[] timelineClips;

    private TextMeshProUGUI textDisplay;
    private int textIndex = 0;
    private string currentText = "";
    private string[] fullTexts = {
    "Ray: Ah, what a night...",
    "Ray: We really went all out with the drinks. My head's spinning...",
    "Ray: Oh no, my wife is calling. I completely forgot about our anniversary!",
    "Ray: Why did I drink so much?",
    "Ray: Now I'll tell her what! I'm officially fucked! SHIT!",
    "Rose: Hey darling, I hope you've set up our anniversary table.",
    "Rose: I'll be home in an hour, love ya...",
    "Ray: Oh no, I've made a huge mistake!"
    };

    private void Awake()
    {
        textDisplay = GetComponent<TextMeshProUGUI>();
    }

    public void StartText()
    {
        StartCoroutine(ShowText(fullTexts[textIndex]));
    }

    private IEnumerator ShowText(string fullText)
    {
        PlaySound();

        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            textDisplay.text = currentText;
            yield return new WaitForSeconds(DELAY);
        }

        PlayPhoneSound();
        yield return new WaitForSeconds(DELAY * END_DELAY_FACTOR);

        currentText = string.Empty;
        textDisplay.text = currentText;

        if (textIndex + 1 < fullTexts.Length)
        {
            textIndex++;
            StartCoroutine(ShowText(fullTexts[textIndex]));
        }
    }

    private void PlaySound()
    {
        if (textIndex == 2)
        {
            timelineSource.clip = timelineClips[0];
            timelineSource.Play();
        }
        else if (textIndex == 5)
        {
            timelineSource.clip = timelineClips[1];
            timelineSource.Play();
        }
        else if (textIndex == 6)
        {
            timelineSource.clip = timelineClips[2];
            timelineSource.Play();
        }
    }

    private void PlayPhoneSound()
    {
        if (textIndex == 4)
        {
            clearThroatSource.Play();
            phoneSource.Play();
        }
        else if (textIndex == 6)
        {
            phoneSource.Play();
        }
    }
}