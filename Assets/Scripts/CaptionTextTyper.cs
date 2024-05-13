using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CaptionTextTyper : MonoBehaviour
{
    private const float COMPLETE_TEXT_PAUSE_FACTOR = 10.0f;

    private TextMeshProUGUI textDisplay;
    private float delay = 0.075f;
    private string currentText = "";
    private string fullText;
    private string currentFullText;
    private bool isStable = false;
    private bool currentIsStable = false;
    private bool canType = true;


    private void Awake()
    {
        textDisplay = GetComponent<TextMeshProUGUI>();
    }

    public void StartType(string _fullText, bool isTextStable)
    {
        fullText = _fullText;
        isStable = isTextStable;
        if (canType && currentFullText != fullText)
        {
            canType = false;
            currentFullText = fullText;
            currentIsStable = isStable;
            textDisplay.text = currentText;
            StartCoroutine(ShowText());
        }
    }

    public void ResetTextIfEqual(string text)
    {
        if (text == currentFullText)
        {
            StartType("", true);
            canType = true;
        }
    }

    private IEnumerator ShowText()
    {
        for (int i = 0; i <= currentFullText.Length; i++)
        {
            currentText = currentFullText.Substring(0, i);
            textDisplay.text = currentText;
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(delay * COMPLETE_TEXT_PAUSE_FACTOR);

        if (currentFullText != fullText)
        {
            currentText = "";
            textDisplay.text = currentText;
            currentFullText = fullText;
            currentIsStable = isStable;
            StartCoroutine(ShowText());
        }
        else
        {
            if (!currentIsStable)
            {
                yield return new WaitForSeconds(delay * COMPLETE_TEXT_PAUSE_FACTOR);
                currentText = "";
                textDisplay.text = currentText;
            }
            canType = true;
        }
    }
}
