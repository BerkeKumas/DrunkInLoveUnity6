using System.Collections;
using UnityEngine;
using TMPro;

public class TextTyper : MonoBehaviour
{
    private const float DELAY = 0.07f;
    private const float END_DELAY_FACTOR = 10.0f;

    private TextMeshProUGUI textDisplay;
    private int textIndex = 0;
    private string currentText = "";
    private string[] fullTexts = {
    "Ray: Ah, what a night...",
    "Ray: We really went all out with the drinks. My head's spinning...",
    "Ray: Wait a second, who's calling now?",
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
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            textDisplay.text = currentText;
            yield return new WaitForSeconds(DELAY);
        }
        yield return new WaitForSeconds(DELAY * END_DELAY_FACTOR);

        currentText = string.Empty;
        textDisplay.text = currentText;

        if (textIndex + 1 < fullTexts.Length)
        {
            textIndex++;
            StartCoroutine(ShowText(fullTexts[textIndex]));
        }
    }
}