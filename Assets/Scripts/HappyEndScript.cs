using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HappyEndScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayLoadScene());
    }

    private IEnumerator DelayLoadScene()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene(3);
    }
}
