using UnityEngine;

public class StairsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("stairsdragtag"))
        {
            Destroy(other.gameObject);
            this.GetComponent<MeshRenderer>().enabled = true;
            this.GetComponent<BoxCollider>().isTrigger = false;
            this.tag = "stairstag";
            this.enabled = false;
        }
    }
}
