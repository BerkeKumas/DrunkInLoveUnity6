using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Volume volume;

    private bool isGamePaused = false;
    private bool isLevelUIEnabled = true;
    private PlayerLook playerLook;
    private Rigidbody playerRb;

    private void Awake()
    {
        playerRb = playerController.gameObject.GetComponent<Rigidbody>();
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
            playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            playerLook.enabled = false;
            playerController.enabled = false;
            enemyController.isPaused = true;
            if (enemyController.agent != null)
            {
                enemyController.agent.isStopped = true;
            }

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
            playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            playerLook.enabled = true;
            playerController.enabled = true;
            enemyController.isPaused = false;
            if (enemyController.agent != null)
            {
                enemyController.agent.isStopped = false;
            }

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
            if (musicObject != null)
            {
                musicObject.GetComponent<AudioSource>().volume = barInput;
            }
        }
    }

    public void SoundBar(float barInput)
    {
        foreach (GameObject soundObject in soundObjects)
        {
            if (soundObject != null)
            {
                AudioSource audioSource = soundObject.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.volume = barInput;
                }
            }
        }
    }

    public void BrightnessBar(float barInput)
    {
        if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.postExposure.value = barInput;
        }
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
