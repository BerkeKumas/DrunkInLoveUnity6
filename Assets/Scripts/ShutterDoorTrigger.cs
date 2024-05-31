using UnityEngine;

public class ShutterDoorTrigger : MonoBehaviour
{
    [SerializeField] private ShutterControl shutterControl;
    [SerializeField] private string triggerTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            if (triggerTag == "Player")
            {
                shutterControl.ToggleShutter(2.0f, 1);
            }
            else if (triggerTag == "enemytag")
            {
                shutterControl.ToggleShutter(2.0f, 1);
            }
            Destroy(this.gameObject);
        }
    }
}
