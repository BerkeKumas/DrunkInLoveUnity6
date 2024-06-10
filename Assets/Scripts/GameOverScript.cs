using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void TryAgain()
    {
        if (PlayerPrefs.GetInt("isDeadByEnemy") == 1)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
