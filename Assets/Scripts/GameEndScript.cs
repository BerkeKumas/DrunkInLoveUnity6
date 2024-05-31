using TMPro;
using UnityEngine;

public class GameEndScript : MonoBehaviour
{
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject lastDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            endText.SetActive(true);
            lastDoor.GetComponent<DoorController>().ToggleDoor();
            lastDoor.tag = "Untagged";
            this.enabled = false;
        }
    }

}
