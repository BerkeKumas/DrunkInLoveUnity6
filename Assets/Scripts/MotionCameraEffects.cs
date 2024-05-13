using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class MotionCameraEffects : MonoBehaviour
{
    private const float FOV_CHANGE_DURATION = 1.0f;
    private float BOBBING_SPEED = 10.0f;
    private const float BOBBING_AMOUNT = 0.15f;
    private const float RUNNING_FOV_INCREASE = 15.0f;

    public bool isEffectActive = false;

    private float defaultFOV;
    private float defaultPosY;
    private float waveTime = 0;
    private CinemachineCamera _cam;

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
            waveslice = Mathf.Sin(waveTime);
            waveTime += BOBBING_SPEED * Time.deltaTime;
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

        BOBBING_SPEED = Input.GetKey(KeyCode.LeftShift) ? 15 : 10;
        float targetFOV = Input.GetKey(KeyCode.LeftShift) ? defaultFOV + RUNNING_FOV_INCREASE : defaultFOV;
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            float currentFOV = _cam.Lens.FieldOfView;
            StartCoroutine(UpdateFOV(currentFOV, targetFOV));
        }
    }

    private IEnumerator UpdateFOV(float _currentFOV, float _targetFOV)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < FOV_CHANGE_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / FOV_CHANGE_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            _cam.Lens.FieldOfView = Mathf.Lerp(_currentFOV, _targetFOV, t);
            yield return null;
        }
        _cam.Lens.FieldOfView = _targetFOV;
    }

    public void StartMotionEffect()
    {
        isEffectActive = true;
    }
}
