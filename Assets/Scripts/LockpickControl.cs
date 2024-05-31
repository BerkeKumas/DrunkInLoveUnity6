using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using Unity.Cinemachine;

public class LockpickControl : MonoBehaviour
{
    [SerializeField] private GameObject lockpickObjects;
    [SerializeField] private GameObject doorObject;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private CinemachineCamera lockpickCam;
    [SerializeField] private LockpickPinControl LockpickPinControl;
    [SerializeField] private InventorySlot handSlot;
    [SerializeField] private GameObject womanObject;
    [SerializeField] private DoorController doorController;
    [SerializeField] private ObjectInteractions objectInteractions;
    [SerializeField] private AudioSource laptopMusic;

    private void OnEnable()
    {
        if (handSlot.itemTag != string.Empty)
        {
            objectInteractions.enabled = false;
            handSlot.itemGameObject.SetActive(false);
        }
        lockpickObjects.SetActive(true);
        LockpickPinControl.enabled = true;
        playerController.UpdateWalkingSound(false);
        playerController.enabled = false;
        playerLook.enabled = false;
        lockpickCam.Priority = 2;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (handSlot.itemTag != string.Empty)
            {
                handSlot.itemGameObject.SetActive(true);
                objectInteractions.enabled = true;
            }
            lockpickObjects.SetActive(false);
            playerController.enabled = true;
            playerLook.enabled = true;
            lockpickCam.Priority = 0;
            this.enabled = false;
        }
    }

    public void Unlocked()
    {
        if (handSlot.itemTag != string.Empty)
        {
            handSlot.itemGameObject.SetActive(true);
            objectInteractions.enabled = true;
        }
        laptopMusic.Stop();
        lockpickObjects.SetActive(false);
        doorObject.tag = "doortag";
        playerController.enabled = true;
        playerLook.enabled = true;
        lockpickCam.Priority = 0;
        if (doorController.IsDoorOpen())
        {
            doorController.ToggleDoor();
            doorController.isDoorLocked = true;
        }
        womanObject.SetActive(true);
        this.enabled = false;
    }
}
