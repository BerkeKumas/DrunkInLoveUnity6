using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitCollector : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject watermelonObject;
    [SerializeField] private GameObject cherryObject;
    [SerializeField] private GameObject redAppleObject;
    [SerializeField] private GameObject greenAppleObject;

    private bool watermelonBool = false;
    private bool cherryBool = false;
    private bool redAppleBool = false;
    private bool greenAppleBool = false;
    private TaskManager taskManager;

    private void Awake()
    {
        taskManager = gameManager.GetComponent<TaskManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.name)
        {
            case "Watermelon":
                watermelonBool = true;
                watermelonObject.SetActive(true);
                Destroy(other.gameObject);
                break;
            case "Cherry":
                cherryBool = true;
                cherryObject.SetActive(true);
                Destroy(other.gameObject);
                break;
            case "RedApple":
                redAppleBool = true;
                redAppleObject.SetActive(true);
                Destroy(other.gameObject);
                break;
            case "GreenApple":
                greenAppleBool = true;
                greenAppleObject.SetActive(true);
                Destroy(other.gameObject);
                break;
            default:
                break;
        }
        CheckBools();
    }

    private void CheckBools()
    {
        if (watermelonBool && cherryBool && redAppleBool && greenAppleBool)
        {
            taskManager.fruitTaskDone = true;
        }
    }
}
