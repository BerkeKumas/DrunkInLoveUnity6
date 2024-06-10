using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;

public class GameEndScript : MonoBehaviour
{
    private readonly string newsText =
        "Breaking News\r\n\r\nWe are here with very striking news today. A victim, R.A., came running to the police station last night and reported a terrifying situation. Initially, the police officers did not believe him.\r\n\r\nThe psychopathic killer named Rose has been getting married at regular intervals for years and has been torturing her husbands by taking them to her basement on their first anniversary.\r\n\r\nUnfortunately, six men have fallen victim to these regular serial murders. The images are too horrifying for us to show you.\r\n\r\nR.A., who escaped from being the seventh victim at the last moment and caused this incredible situation to come to light, is currently in the hospital.\r\n\r\nWe will continue to share the details of this terrible case with you. Stay tuned.";

    private const float DELAY = 0.01f;
    private const float END_DELAY_FACTOR = 10.0f;


    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject lastDoor;
    [SerializeField] private Image fadeImage;
    [SerializeField] private RectTransform creditsText;
    [SerializeField] private GameObject enemy;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private AudioSource endSource;
    [SerializeField] private TextMeshProUGUI newsTextDisplay;
    [SerializeField] private AudioSource textSound;

    private float scrollSpeed = 30f;
    private string currentText = string.Empty;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.GetComponent<BoxCollider>().enabled = false;
            endText.SetActive(true);
            lastDoor.GetComponent<DoorController>().ToggleDoor();
            lastDoor.tag = "Untagged";

            enemy.GetComponent<EnemyController>().isPaused = true;
            pauseManager.MusicBar(0);
            pauseManager.SoundBar(0);
            pauseManager.enabled = false;
            playerController.enabled = false;
            playerLook.enabled = false;

            StartCoroutine(ShowText());
        }
    }

    private IEnumerator ShowText()
    {
        endSource.Play();
        StartCoroutine(FadeBlack());
        yield return new WaitForSeconds(2.0f);
        endText.SetActive(false);
        newsTextDisplay.gameObject.SetActive(true);

        textSound.Play();

        for (int i = 0; i <= newsText.Length; i++)
        {
            currentText = newsText.Substring(0, i);
            newsTextDisplay.text = currentText;
            yield return new WaitForSeconds(DELAY);
        }

        yield return new WaitForSeconds(DELAY * END_DELAY_FACTOR);

        currentText = string.Empty;
        newsTextDisplay.text = currentText;

        textSound.Stop();
        StartCoroutine (ActionControl());
    }

    private IEnumerator ActionControl()
    {
        yield return new WaitForSeconds(2.0f);
        creditsText.gameObject.SetActive(true);
        StartCoroutine(ScrollCredits());
    }

    private IEnumerator FadeBlack()
    {
        float elapsedTime = 0f;
        float totalTime = 2.0f;
        Color initialColor = fadeImage.color;
        Color targetColor = Color.black;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalTime;
            fadeImage.color = Color.Lerp(initialColor, targetColor, t);
            yield return null;
        }
    }

    private IEnumerator ScrollCredits()
    {
        float elapsedTime = 0f;
        float totalTime = (creditsText.rect.height + 950) / scrollSpeed;
        Vector2 initialPosition = new Vector2(creditsText.anchoredPosition.x, -750);
        Vector2 targetPosition = new Vector2(creditsText.anchoredPosition.x, 350);

        creditsText.anchoredPosition = initialPosition;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalTime;
            creditsText.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(0);
    }
}
