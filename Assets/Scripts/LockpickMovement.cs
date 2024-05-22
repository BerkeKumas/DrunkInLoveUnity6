using UnityEngine;

public class LockpickMovement : MonoBehaviour
{
    private const float MOVE_SPEED = 0.6f;
    private const float LERP_SPEED = 2.0f;

    private Vector3 startPosition;
    private bool isReturning = true;

    private Vector3 movement;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        movement.z = -Input.GetAxis("Horizontal");

        float targetZ = Mathf.Clamp(transform.localPosition.z + (movement.z * MOVE_SPEED * Time.deltaTime), startPosition.z - 1.50f, startPosition.z);

        if (Input.GetKey(KeyCode.W))
        {
            isReturning = false;
            float targetY = Mathf.Clamp(transform.localPosition.y + (MOVE_SPEED * Time.deltaTime), startPosition.y, startPosition.y + 0.35f);
            transform.localPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);
        }
        else
        {
            isReturning = true;
        }

        if (isReturning)
        {
            float targetY = Mathf.Lerp(transform.localPosition.y, startPosition.y, LERP_SPEED * Time.deltaTime);
            transform.localPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);
        }

        if (!Input.GetKey(KeyCode.W) && Mathf.Abs(transform.localPosition.y - startPosition.y) < 0.03f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, targetZ);
        }
    }
}
