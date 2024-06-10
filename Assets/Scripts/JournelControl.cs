using System.Collections;
using TMPro;
using UnityEngine;

public class JournelControl : MonoBehaviour
{
    [SerializeField] private GameObject journelObject;
    [SerializeField] private TextMeshProUGUI[] lines;
    [SerializeField] private AudioSource journelAddSource;
    [SerializeField] private AudioSource journelOpenSource;
    [SerializeField] private TextMeshProUGUI newHintText;

    private bool isJournelActive = false;
    private int currentLine = 0;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            isJournelActive = !isJournelActive;
            if (isJournelActive) journelOpenSource.Play();
            journelObject.SetActive(isJournelActive);
        }
    }

    public void AddNewLine(string newLine)
    {
        journelAddSource.Play();
        if (!newHintText.gameObject.activeSelf)
        {
            newHintText.gameObject.SetActive(true);
        }
        StartCoroutine(activateHintText());
        lines[currentLine].text = "-" + newLine;
        if (journelObject.activeSelf)
        {
            lines[currentLine].ForceMeshUpdate();
            int lineCount = lines[currentLine].textInfo.lineCount;
            currentLine += lineCount;
        }
        else
        {
            journelObject.SetActive(true);
            lines[currentLine].ForceMeshUpdate();
            int lineCount = lines[currentLine].textInfo.lineCount;
            currentLine += lineCount;
            journelObject.SetActive(false);
        }
    }

    private IEnumerator activateHintText()
    {
        float elapsedTime = 0.0f;
        float totalTime = 1.0f;
        Color newColor = newHintText.color;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / totalTime;
            newColor.a = alpha;
            newHintText.color = newColor;
            yield return null;
        }

        elapsedTime = totalTime;
        yield return new WaitForSeconds(1.0f);

        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            float alpha = elapsedTime / totalTime;
            newColor.a = alpha;
            newHintText.color = newColor;
            yield return null;
        }
    }
}
