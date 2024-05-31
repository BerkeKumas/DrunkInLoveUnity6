using UnityEngine;

public class EnemyDoorControl : MonoBehaviour
{
    private DoorController doorController;

    private void Awake()
    {
        doorController = GetComponentInChildren<DoorController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemytag"))
        {
            if (!doorController.IsDoorOpen())
            {
                doorController.StartDoorRotation(true);
                transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("enemytag"))
        {
            if (doorController.IsDoorOpen())
            {
                transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
                doorController.StartDoorRotation(false);
            }
        }
    }
}
