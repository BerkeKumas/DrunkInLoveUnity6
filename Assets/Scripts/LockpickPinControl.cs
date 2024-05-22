using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.LowLevel;

public class LockpickPinControl : MonoBehaviour
{
    [SerializeField] private List<GameObject> lockPins;
    [SerializeField] private LockpickControl lockpickControl;
    [SerializeField] private Material defaultMat;

    private int lockIndex;
    private GameObject targetPin;

    private void Start()
    {
        SetTargetPin();
    }

    private void SetTargetPin()
    {
        lockIndex = Random.Range(0, lockPins.Count);
        targetPin = lockPins[lockIndex];
        targetPin.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetPin)
        {
            targetPin.GetComponent<MeshRenderer>().material = defaultMat;
            other.GetComponent<BoxCollider>().enabled = false;
            other.GetComponent<Rigidbody>().isKinematic = true;
            lockPins.RemoveAt(lockIndex);
            if (lockPins.Count == 0)
            {
                lockpickControl.Unlocked();
                this.enabled = false;
                return;
            }
            SetTargetPin();
        }
    }
}
