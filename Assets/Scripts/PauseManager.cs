using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public bool canPause = false;

    private const float SENSITIVITY_FACTOR = 5.0f;
    private const int MENU_INDEX = 0;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject levelUI;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject[] musicObjects;
    [SerializeField] private GameObject[] soundObjects;

    private bool isGamePaused = false;
    private bool isLevelUIEnabled = true;
    private PlayerLook playerLook;

    private void Awake()
    {
        playerLook = cameraObject.GetComponent<PlayerLook>();
    }

    private void Update()
    {
        if (canPause)
        {
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Cursor.lockState = CursorLockMode.None;
            playerLook.isMouseEnabled = false;

            if (!levelUI.activeSelf)
            {
                isLevelUIEnabled = false;
            }
            else
            {
                levelUI.SetActive(false);
            }
            pauseMenu.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerLook.isMouseEnabled = true;
            pauseMenu.SetActive(false);
            if (isLevelUIEnabled)
            {
                levelUI.SetActive(true);
            }
        }
    }

    public void Sensitivity(float sensInput)
    {
        playerLook.mouseSensitivity = sensInput * SENSITIVITY_FACTOR;
    }

    public void MusicBar(float barInput)
    {
        foreach (GameObject musicObject in musicObjects)
        {
            musicObject.GetComponent<AudioSource>().volume = barInput;
        }
    }

    public void SoundBar(float barInput)
    {
        foreach (GameObject soundObject in soundObjects)
        {
            soundObject.GetComponent<AudioSource>().volume = barInput;
        }
    }

    public void BrightnessBar(float barInput)
    {
        RenderSettings.ambientIntensity = barInput;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(MENU_INDEX);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        PauseGame();
    }
}
