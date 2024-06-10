using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CaptionTextTyper : MonoBehaviour
{
    private const float UNSTABLE_COMPLETATION_FACTOR = 60.0f;
    private const float COMPLETE_TEXT_PAUSE_FACTOR = 20.0f;
    private const float LETTER_DELAY = 0.04f;

    [SerializeField] private AudioSource typeSource; 

    private TextMeshProUGUI textDisplay;
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
            if (!typeSource.isPlaying)
            {
                typeSource.Play();
            }
            currentText = currentFullText.Substring(0, i);
            textDisplay.text = currentText;
            yield return new WaitForSeconds(LETTER_DELAY);
        }
        if (typeSource.isPlaying)
        {
            typeSource.Stop();
        }
        yield return new WaitForSeconds(LETTER_DELAY * COMPLETE_TEXT_PAUSE_FACTOR);


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
            canType = true;
            if (!currentIsStable)
            {
                yield return new WaitForSeconds(LETTER_DELAY * UNSTABLE_COMPLETATION_FACTOR);
                if (canType)
                {
                    currentText = "";
                    textDisplay.text = currentText;
                }
            }
        }
    }
}
