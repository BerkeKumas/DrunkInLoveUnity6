using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectDragHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject is { tag: "stairsdragtag" or "doortag" })
        {
            transform.SetParent(null);
        }
    }
}
