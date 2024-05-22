using UnityEngine;

public class VentCoverControl : MonoBehaviour
{
    [SerializeField] private GameObject[] screws;
    [SerializeField] private VentController ventController;

    private bool releaseVent = false;

    public void CheckScrews()
    {
        releaseVent = true;
        foreach (GameObject screw in screws)
        {
            if (screw.GetComponent<Rigidbody>().isKinematic)
            {
                releaseVent = false;
            }
        }

        if (releaseVent)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            ventController.ExitVentMode();
            this.enabled = false;
        }
    }
}
