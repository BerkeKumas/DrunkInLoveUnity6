using UnityEngine;
using UnityEngine.UI;

public class BreathControl : MonoBehaviour
{
    private const float INITIAL_TARGET_WIDTH = 45.0f;
    private const float MIN_TARGET_WIDTH = 20.0f;
    private const float DECREASE_AMOUNT = 5.0f;
    private const float BAR_LIMIT = 170.0f;
    private const float NEEDLE_MOVE_SPEED = 400.0f;

    [SerializeField] private GameObject BreathControlUI;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private RectTransform movingNeedle;
    [SerializeField] private RectTransform targetArea;

    private float targetWidth = INITIAL_TARGET_WIDTH;
    private float targetLoc;
    private float targetAreaLimit;
    private bool isMovingRight = true;
    private bool isHoldingBreath = false;
    private bool enableBreathControl = false;

    private void Start()
    {
        targetAreaLimit = BAR_LIMIT - targetWidth / 2.0f;
        SetNewTargetArea();
        movingNeedle.localPosition = new Vector3(0, movingNeedle.localPosition.y, movingNeedle.localPosition.z);
    }

    private void Update()
    {
        if (!enableBreathControl) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isHoldingBreath = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isHoldingBreath = false;
            if (CheckSuccess())
            {
                ReduceTargetArea();
            }
            SetNewTargetArea();
        }

        if (isHoldingBreath)
        {
            MoveNeedle();
        }
    }

    private void SetNewTargetArea()
    {
        targetLoc = Random.Range(-targetAreaLimit, targetAreaLimit);
        targetArea.sizeDelta = new Vector2(targetWidth, targetArea.sizeDelta.y);
        targetArea.localPosition = new Vector3(targetLoc, targetArea.localPosition.y, targetArea.localPosition.z);
    }

    private void MoveNeedle()
    {
        float newX = movingNeedle.localPosition.x + Time.deltaTime * NEEDLE_MOVE_SPEED * (isMovingRight ? 1 : -1);
        if (newX > BAR_LIMIT)
        {
            newX = BAR_LIMIT;
            isMovingRight = false;
        }
        else if (newX < -BAR_LIMIT)
        {
            newX = -BAR_LIMIT;
            isMovingRight = true;
        }
        movingNeedle.localPosition = new Vector3(newX, movingNeedle.localPosition.y, movingNeedle.localPosition.z);
    }

    private bool CheckSuccess()
    {
        if (movingNeedle.localPosition.x >= targetLoc - targetWidth / 2.0f && movingNeedle.localPosition.x <= targetLoc + targetWidth / 2.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ReduceTargetArea()
    {
        if (targetWidth > MIN_TARGET_WIDTH)
        {
            targetWidth -= DECREASE_AMOUNT;
            if (targetWidth <= MIN_TARGET_WIDTH)
            {
                targetWidth = INITIAL_TARGET_WIDTH;
                enemyController.canSearchPlayer = false;
                enableBreathControl = false;
                BreathControlUI.SetActive(false);
            }
        }
    }

    public void ToggleBreathControl()
    {
        if (!enableBreathControl)
        {
            enableBreathControl = true;
            BreathControlUI.SetActive(true);
        }
    }
}
