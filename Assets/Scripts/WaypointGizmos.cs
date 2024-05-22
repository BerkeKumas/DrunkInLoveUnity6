using UnityEngine;

public class WaypointGizmos : MonoBehaviour
{
    public Color gizmoColor = Color.red;
    public float gizmoSize = 0.5f;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoSize);
    }
}