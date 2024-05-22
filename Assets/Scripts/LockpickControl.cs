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

    private void OnEnable()
    {
        if (handSlot.itemTag != string.Empty)
        {
            handSlot.itemGameObject.SetActive(false);
        }
        lockpickObjects.SetActive(true);
        LockpickPinControl.enabled = true;
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
        }
        lockpickObjects.SetActive(false);
        doorObject.tag = "doortag";
        playerController.enabled = true;
        playerLook.enabled = true;
        lockpickCam.Priority = 0;
        this.enabled = false;
    }
}
