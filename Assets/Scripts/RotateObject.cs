using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private const float MAX_ROTATION = 60.0f;
    private const float ROTATION_SPEED = 90.0f;

    [SerializeField] private GameObject parentHoldObject;

    private bool canRotate = false;
    private bool needToCheckChild = false;
    private float rotationAmount = 0;
    private GameObject holdObject;

    private void Update()
    {
        if (parentHoldObject.transform.childCount > 0)
        {
            if (needToCheckChild)
            {
                needToCheckChild = false;
                holdObject = parentHoldObject.transform.GetChild(0).gameObject;

                if (holdObject.tag == "winetag")
                {
                    rotationAmount = holdObject.transform.localEulerAngles.z;
                    canRotate = true;
                }
            }

            if (canRotate)
            {
                Rotate();
            }
        }
        else
        {
            needToCheckChild = true;
            canRotate = false;
        }
    }

    private void Rotate()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (rotationAmount < MAX_ROTATION)
            {
                float rotateStep = ROTATION_SPEED * Time.deltaTime;
                rotationAmount += rotateStep;
                rotationAmount = Mathf.Min(rotationAmount, MAX_ROTATION);
                holdObject.transform.Rotate(0, 0, rotateStep, Space.Self);
                holdObject.transform.GetChild(0).rotation = Quaternion.identity;
            }
        }
        if (Input.GetKey(KeyCode.E))
        {
            if (rotationAmount > -MAX_ROTATION)
            {
                float rotateStep = ROTATION_SPEED * Time.deltaTime;
                rotationAmount -= rotateStep;
                rotationAmount = Mathf.Max(rotationAmount, -MAX_ROTATION);
                holdObject.transform.Rotate(0, 0, -rotateStep, Space.Self);
                holdObject.transform.GetChild(0).rotation = Quaternion.identity;
            }
        }
    }
}
