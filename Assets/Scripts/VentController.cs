using UnityEngine;
using Unity.Cinemachine;
using TMPro;

public class VentController : MonoBehaviour
{
    private readonly Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f);
    private const float RAY_DISTANCE = 5.0f;
    private const string ITEM_NEED_TEXT = "I need something to open this...";

    [SerializeField] private CinemachineCamera ventCam;
    [SerializeField] private TextMeshProUGUI ventText;
    [SerializeField] private InventorySlot handSlot;
    [SerializeField] private CaptionTextTyper captionTextTyper;

    private bool activateVent = false;

    private void Update()
    {
        if (!activateVent)
        {
            ScreenVentRay();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ExitVentMode();
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj.tag == "screwtag" && Input.GetMouseButtonDown(0))
                {
                    hitObj.GetComponent<ScrewRotationHandler>().StartScrewRotation();
                }
            }
        }
    }

    public void ExitVentMode()
    {
        if (handSlot.itemTag != string.Empty)
        {
            handSlot.itemGameObject.SetActive(true);
        }
        GetComponent<BoxCollider>().enabled = true;
        ventCam.Priority = 0;
        Cursor.lockState = CursorLockMode.Locked;
        activateVent = false;
        this.enabled = false;
    }

    private void ScreenVentRay()
    {
        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, RAY_DISTANCE))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.tag == "venttag")
            {
                ventText.text = "[F] to Zoom";
                if (handSlot.itemTag == "screwdrivertag")
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        handSlot.itemGameObject.SetActive(false);
                        ventText.text = string.Empty;
                        GetComponent<BoxCollider>().enabled = false;
                        ventCam.Priority = 2;
                        Cursor.lockState = CursorLockMode.None;
                        activateVent = true;
                    }
                }
                else
                {
                    captionTextTyper.StartType(ITEM_NEED_TEXT, false);
                }
            }
            else
            {
                ventText.text = string.Empty;
            }
        }
        else
        {
            ventText.text = string.Empty;
        }
    }
}
