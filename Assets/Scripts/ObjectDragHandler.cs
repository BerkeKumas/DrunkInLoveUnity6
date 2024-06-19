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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject is { tag: "stairsdragtag" or "doortag" or "wallstag" })
        {
            Vector3 collisionNormal = collision.contacts[0].normal;
            transform.position += collisionNormal * 0.1f;
        }
    }
}
