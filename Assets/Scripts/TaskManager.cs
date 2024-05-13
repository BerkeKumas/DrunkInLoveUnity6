using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class TaskManager : MonoBehaviour
{
    public class Task
    {
        public string Description { get; private set; }
        public bool IsCompleted { get; private set; }
        public TextMeshProUGUI TaskText { get; private set; }
        public RectTransform RectTransform => TaskText.rectTransform;

        public Task(string description, TextMeshProUGUI taskText)
        {
            Description = description;
            TaskText = taskText;
            Reset();
        }

        public void Complete()
        {
            IsCompleted = true;
        }

        public void Reset()
        {
            IsCompleted = false;
            TaskText.color = Color.white;
            TaskText.text = Description;
        }

        public void UpdateDescription(string newDescription)
        {
            Description = newDescription;
            TaskText.text = Description;
        }
    }

    private const float CLOSE_UI_END_DELAY = 2.0f;
    private const float WAIT_FOR_TEXT_COMPLETE = 2.5f;
    private const float UI_OFFSET = 15.0f;
    private const float OBJECTIVE_STRIKE_LETTER_INTERVAL = 0.05f;
    private const float ANIMATION_DURATION = 1.0f;
    private const float CLOTHES_TASK_CHECK_INTERVAL = 0.5f;
    private const string FINAL_REMINDER_MESSAGE = "Looks like I missed a dirty laundry.";

    public bool isClothesTaskActive = true;
    public bool wineTaskDone = false;
    public bool fruitTaskDone = false;
    public bool musicTaskDone = false;
    public bool lastLaundryActive = false;
    public bool isTaskManagerStarted = false;

    [SerializeField] private TextMeshProUGUI clothesTaskText;
    [SerializeField] private TextMeshProUGUI wineTaskText;
    [SerializeField] private TextMeshProUGUI fruitTaskText;
    [SerializeField] private TextMeshProUGUI musicTaskText;
    [SerializeField] private GameObject lastLaundry;
    [SerializeField] private GameObject taskList;
    [SerializeField] private GameObject tabText;
    [SerializeField] private List<GameObject> clothesObjects;
    [SerializeField] private CaptionTextTyper captionTextTyper;
    [SerializeField] private RectTransform panelRectTransform;
    [SerializeField] private PlayableDirector enableTaskList;
    [SerializeField] private PlayableDirector disableTaskList;

    private List<Task> tasks = new List<Task>();

    void Start()
    {
        InitializeTasks();
        StartCoroutine(CheckClothesTaskPeriodically());
    }

    private void InitializeTasks()
    {
        tasks = new List<Task>
        {
            new Task("• Put clothes into laundry basket.", clothesTaskText),
            new Task("• Pour wine into a glass.", wineTaskText),
            new Task("• Prepare a fruit plate.", fruitTaskText),
            new Task("• Play music on the laptop.", musicTaskText)
        };
    }

    void Update()
    {
        if (!isTaskManagerStarted) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (tabText.activeSelf)
            {
                tabText.SetActive(false);
                enableTaskList.Play();
            }

            if (taskList.activeSelf)
            {
                disableTaskList.Play();
            }
            else
            {
                taskList.SetActive(true);
                enableTaskList.Play();
            }
        }

        if (wineTaskDone && !tasks[1].IsCompleted)
            CompleteTask(1);
        if (fruitTaskDone && !tasks[2].IsCompleted)
            CompleteTask(2);
        if (musicTaskDone && !tasks[3].IsCompleted)
            CompleteTask(3);

        CheckAllTasksCompleted();
    }

    private void CompleteTask(int taskIndex)
    {
            taskList.SetActive(true);
            enableTaskList.Play();
            tasks[taskIndex].Complete();
            StartCoroutine(TextCompleteEffect(tasks[taskIndex].TaskText, tasks[taskIndex].Description));

            if (taskIndex < tasks.Count - 1)
            {
                StartCoroutine(ShiftTextsUp(taskIndex + 1));
            }

            StartCoroutine(ShiftPanelUp());
    }


    private IEnumerator TextCompleteEffect(TextMeshProUGUI taskText, string description)
    {
        string completedText = string.Empty;
        for (int i = 0; i < description.Length; i++)
        {
            completedText += $"<color=#00ff00><s>{description[i]}</s></color>";
            taskText.text = completedText + description.Substring(i + 1);
            yield return new WaitForSeconds(OBJECTIVE_STRIKE_LETTER_INTERVAL);
        }

        float alpha = 1.0f;
        while (alpha > 0.0f)
        {
            alpha -= Time.deltaTime;
            taskText.color = new Color(taskText.color.r, taskText.color.g, taskText.color.b, alpha);
            yield return null;
        }

        taskText.gameObject.SetActive(false);
    }

    private IEnumerator ShiftTextsUp(int startIndex)
    {
        yield return new WaitForSeconds(WAIT_FOR_TEXT_COMPLETE);

        Vector3[] originalPositions = new Vector3[tasks.Count];
        for (int i = startIndex; i < tasks.Count; i++)
        {
            originalPositions[i] = tasks[i].RectTransform.anchoredPosition;
        }

        float elapsedTime = 0f;

        while (elapsedTime < ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / ANIMATION_DURATION;
            t = Mathf.SmoothStep(0, 1, t);

            for (int i = startIndex; i < tasks.Count; i++)
            {
                Vector3 newPos = originalPositions[i] + new Vector3(0, t * UI_OFFSET, 0);
                tasks[i].RectTransform.anchoredPosition = Vector3.Lerp(originalPositions[i], newPos, t);
            }
            yield return null;
        }
    }

    private IEnumerator ShiftPanelUp()
    {
        yield return new WaitForSeconds(WAIT_FOR_TEXT_COMPLETE);

        Vector2 originalBottom = new Vector2(panelRectTransform.offsetMin.x, panelRectTransform.offsetMin.y);
        Vector2 newBottom = new Vector2(panelRectTransform.offsetMin.x, panelRectTransform.offsetMin.y + UI_OFFSET);
        float elapsedTime = 0f;

        while (elapsedTime < ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / ANIMATION_DURATION;
            t = Mathf.SmoothStep(0, 1, t);
            panelRectTransform.offsetMin = Vector2.Lerp(originalBottom, newBottom, t);
            yield return null;
        }

        panelRectTransform.offsetMin = newBottom;

        yield return new WaitForSeconds(CLOSE_UI_END_DELAY);
        disableTaskList.Play();
    }

    private void CheckAllTasksCompleted()
    {
        if (tasks.All(t => t.IsCompleted) && !lastLaundryActive)
            AllTasksEnded();
    }

    private IEnumerator CheckClothesTaskPeriodically()
    {
        while (isClothesTaskActive)
        {
            UpdateClothesTask();
            yield return new WaitForSeconds(CLOTHES_TASK_CHECK_INTERVAL);
        }
    }

    private void UpdateClothesTask()
    {
        clothesObjects = clothesObjects.Where(cloth => cloth != null).ToList();
        if (clothesObjects.Count == 0)
        {
            isClothesTaskActive = false;
            tasks[0].UpdateDescription($"• Put clothes into laundry basket.");
            CompleteTask(0);
        }
        else
        {
            tasks[0].UpdateDescription($"• Put {clothesObjects.Count} clothes into laundry basket.");
        }
    }

    private void AllTasksEnded()
    {
        lastLaundryActive = true;
        captionTextTyper.StartType(FINAL_REMINDER_MESSAGE, true);
        lastLaundry.SetActive(true);
    }

    public void DisableTaskList()
    {
        taskList.SetActive(false);
        if (tabText.activeSelf)
        {
            isTaskManagerStarted = true;
        }
    }
}
