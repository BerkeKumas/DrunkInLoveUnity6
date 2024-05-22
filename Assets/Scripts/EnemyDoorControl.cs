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
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("enemytag"))
        {
            if (doorController.IsDoorOpen())
            {
                doorController.StartDoorRotation(false);
            }
        }
    }
}
