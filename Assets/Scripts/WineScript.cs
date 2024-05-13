using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WineScript : MonoBehaviour
{
    private const float MAX_DISTANCE_TO_POUR = 0.4f;
    private const float GLASS_FILL_LIMIT = 0.92f;
    private const float POUR_ANGLE_LIMIT = 50.0f;
    private const float POUR_RISE_SPEED = 0.1f;

    [SerializeField] private GameObject glass;
    [SerializeField] private GameObject gameManager;

    private GameObject wineBottleTip;
    private GameObject wineLevel;
    private TaskManager taskManager;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        wineBottleTip = gameObject.transform.GetChild(0).gameObject;
        wineLevel = glass.transform.GetChild(0).gameObject;
        taskManager = gameManager.GetComponent<TaskManager>();
    }

    private void Update()
    {
        float rotationAmount = transform.localEulerAngles.z;
        rotationAmount = rotationAmount > 180 ? rotationAmount - 360 : rotationAmount;

        if (rotationAmount >= POUR_ANGLE_LIMIT || rotationAmount <= -POUR_ANGLE_LIMIT)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            PourWine();
        }
        else
        {
            audioSource.Stop();
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void PourWine()
    {
        Vector2 pourVec = new Vector2(wineBottleTip.transform.position.x, wineBottleTip.transform.position.z);
        Vector2 glassVec = new Vector2(glass.transform.position.x, glass.transform.position.z);

        if (Vector2.Distance(pourVec, glassVec) <= MAX_DISTANCE_TO_POUR)
        {
            if (wineLevel.transform.localPosition.y >= GLASS_FILL_LIMIT)
            {
                audioSource.Stop();
                taskManager.wineTaskDone = true;
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                wineLevel.transform.localPosition += new Vector3(0, POUR_RISE_SPEED * Time.deltaTime, 0);
            }
        }
        else
        {
            audioSource.Stop();
        }
    }
}
