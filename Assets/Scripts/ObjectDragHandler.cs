using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectDragHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject is { tag: "stairsdragtag" or "doortag"  or "wallstag"})
        {
            transform.SetParent(null);
        }
    }
}
