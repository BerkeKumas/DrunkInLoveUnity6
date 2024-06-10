using UnityEngine;

public class ScrewRotationHandler : MonoBehaviour
{
    [SerializeField] private VentCoverControl ventCoverControl;

    private float movementSpeedFactor = 0.0003f;
    private float minZ = -0.1f;
    private float maxZ = 1.0f;

    private Vector3 screwCenter;
    private bool isMouseClicked = false;
    private bool startRotation = false;
    private float previousAngle = 0f;
    private Rigidbody rb;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!startRotation) return;

        if (Input.GetMouseButtonDown(0))
        {
            isMouseClicked = true;
        }

        if (isMouseClicked)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 direction = currentMousePosition - screwCenter;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion currentRotation = transform.rotation;
            currentRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, -angle);
            transform.rotation = currentRotation;

            float angleDelta = -Mathf.DeltaAngle(previousAngle, angle);

            float movementAmount = angleDelta * movementSpeedFactor;

            if (angleDelta > 0)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Clamp(transform.localPosition.z - movementAmount, minZ, maxZ));
            }
            else if (angleDelta < 0)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Clamp(transform.localPosition.z - movementAmount, minZ, maxZ));
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }

            if (transform.localPosition.z >= 0.68f)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                startRotation = false;
                transform.GetChild(0).gameObject.SetActive(false);
                rb.isKinematic = false;
                ventCoverControl.CheckScrews();
                this.enabled = false;
            }

            previousAngle = angle;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            isMouseClicked = false;
        }
    }

    public void StartScrewRotation()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        screwCenter = Camera.main.WorldToScreenPoint(transform.position);
        startRotation = true;
    }
}
