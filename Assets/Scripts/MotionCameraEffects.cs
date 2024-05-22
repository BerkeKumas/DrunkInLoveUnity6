using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class MotionCameraEffects : MonoBehaviour
{
    private const float FOV_CHANGE_DURATION = 1.0f;
    private const float BOBBING_AMOUNT = 0.15f;
    private const float RUNNING_FOV_INCREASE = 10.0f;

    public bool isEffectActive = false;

    private float targetFOV;
    private float currentFOV;
    private float defaultFOV;
    private float defaultPosY;
    private float waveTime = 0;
    private float bobbingSpeed = 10.0f;
    private CinemachineCamera _cam;
    private Coroutine currentCoroutine;

    private void Start()
    {
        _cam = GetComponent<CinemachineCamera>();
        defaultPosY = transform.localPosition.y;
        defaultFOV = _cam.Lens.FieldOfView;
    }

    private void Update()
    {
        if (!isEffectActive) return;

        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 cameraTransform = transform.localPosition;

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            waveTime = 0;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                bobbingSpeed = 15.0f;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                bobbingSpeed = 5.0f;
            }
            else
            {
                bobbingSpeed = 10.0f;
            }
            targetFOV = Input.GetKey(KeyCode.LeftShift) ? defaultFOV + RUNNING_FOV_INCREASE : defaultFOV;
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift))
            {
                currentFOV = _cam.Lens.FieldOfView;
                SwitchCoroutine(UpdateFOV());
            }

            waveslice = Mathf.Sin(waveTime);
            waveTime += bobbingSpeed * Time.deltaTime;
            if (waveTime > Mathf.PI * 2)
            {
                waveTime -= (Mathf.PI * 2);
            }
        }

        if (waveslice != 0)
        {
            float translateChange = waveslice * BOBBING_AMOUNT;
            float totalAxes = new Vector2(horizontal, vertical).magnitude;
            totalAxes = Mathf.Clamp01(totalAxes);
            translateChange = totalAxes * translateChange;
            cameraTransform.y = defaultPosY + translateChange;
        }
        else
        {
            cameraTransform.y = defaultPosY;
        }

        transform.localPosition = cameraTransform;
    }

    private void SwitchCoroutine(IEnumerator newCoroutine)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(newCoroutine);
    }

    private IEnumerator UpdateFOV()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < FOV_CHANGE_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / FOV_CHANGE_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            _cam.Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, t);
            yield return null;
        }
        _cam.Lens.FieldOfView = targetFOV;
    }

    public void StartMotionEffect()
    {
        isEffectActive = true;
    }
}
