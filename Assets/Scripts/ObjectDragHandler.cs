using UnityEngine;

public class ObjectDragHandler : MonoBehaviour
{
    private readonly Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f);
    private const float RAY_DISTANCE = 5.0f;

    [SerializeField] private GameObject moveObjectParent;

    private GameObject hitObject;
    private string[] tagsToCheck = { "wallstag" };
    private bool isDragActive = true;

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, RAY_DISTANCE))
        {
            hitObject = hit.collider.gameObject;
            if (hitObject.tag == "dragtag")
            {
                if (!isDragActive) return;

                if (Input.GetMouseButtonDown(0))
                {
                    hitObject.transform.SetParent(moveObjectParent.transform);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    isDragActive = true;
                    hitObject.transform.SetParent(null);
                }
            }
            else
            {
                isDragActive = true;
                hitObject.transform.SetParent(null);
            }
        }
        else
        {
            isDragActive = true;
            if (hitObject != null)
            {
                hitObject.transform.SetParent(null);
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        foreach (string tag in tagsToCheck)
        {
            if (col.gameObject.CompareTag(tag))
            {
                isDragActive = false;
                hitObject.transform.SetParent(null);
            }
        }
    }
}
