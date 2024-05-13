using UnityEngine;
using TMPro;
using Unity.Cinemachine;

public class LockPick : MonoBehaviour
{
    private const float ROTATION_SPEED = 0.75f;

    [SerializeField] private RectTransform rotatingNeedle;
    [SerializeField] private RectTransform targetArea;
    [SerializeField] private TextMeshProUGUI countSuccessText;
    [SerializeField] private GameObject lockPickUI;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private CinemachineCamera lockPickCam;

    private int countSuccess = 0;
    private float targetAngle;
    private bool isRotating = false;
    private bool enabledControl = false;
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        SetRandomTargetAngle();
    }

    private void Update()
    {
        if (!enabledControl) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            lockPickUI.SetActive(false);
            enabledControl = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRotating = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isRotating = false;
            CheckSuccess();
        }

        if (isRotating)
        {
            RotateObject();
        }
    }

    public void ToggleLockPick()
    {
        boxCollider.enabled = false;
        lockPickCam.Priority = 2;
        playerController.enabled = false;
        playerLook.enabled = false;
        lockPickUI.SetActive(true);
        enabledControl = true;
    }

    private void RotateObject()
    {
        rotatingNeedle.Rotate(0, 0, -ROTATION_SPEED);
    }

    private void SetRandomTargetAngle()
    {
        targetAngle = Random.Range(0, 360);
        targetArea.localEulerAngles = new Vector3(0, 0, targetAngle);
    }

    private void CheckSuccess()
    {
        float currentAngle = rotatingNeedle.localEulerAngles.z;

        if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) <= 18)
        {
            countSuccess++;
            countSuccessText.text = $"{countSuccess}/5";
            if (countSuccess == 5)
            {
                boxCollider.enabled = true;
                lockPickUI.SetActive(false);
                enabledControl = false;
                gameObject.tag = "doortag";
                playerController.enabled = true;
                playerLook.enabled = true;
                lockPickCam.Priority = 0;
            }
        }
        SetRandomTargetAngle();
        rotatingNeedle.localEulerAngles = Vector3.zero;
    }
}
