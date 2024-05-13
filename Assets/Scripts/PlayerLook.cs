using System.Collections;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    private const float VERTICAL_ROTATION_LIMIT = 90.0f;

    public bool isMouseEnabled = false;
    public float mouseSensitivity = 2.5f;
    public float startingRotation = 180.0f;

    [SerializeField] private Transform player;
    [SerializeField] private float rotationSmoothing = 25.0f;

    private Vector2 rotationVelocity = Vector2.zero;
    private Vector2 frameVelocity = Vector2.zero;
    private float removeX;
    private float removeY;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!isMouseEnabled) return;

        ProcessMouseMovement();
        ApplyRotation();
    }

    private void ProcessMouseMovement()
    {
        Vector2 mouseInputs = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = mouseInputs * mouseSensitivity;
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, rotationSmoothing * Time.deltaTime);
        rotationVelocity += frameVelocity;
        rotationVelocity.y = Mathf.Clamp(rotationVelocity.y, -VERTICAL_ROTATION_LIMIT, VERTICAL_ROTATION_LIMIT);
    }

    private void ApplyRotation()
    {
        transform.localRotation = Quaternion.AngleAxis(-rotationVelocity.y, Vector3.right);
        player.localRotation = Quaternion.AngleAxis(startingRotation + rotationVelocity.x, Vector3.up);
    }

    public void StartMouseRotation()
    {
        isMouseEnabled = true;
    }
}
