using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ReadPin : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = {
        KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
        KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9
    };

    public bool PinMode = false;

    private const int PIN_LENGTH = 8;
    private const int MAX_DAY = 28;
    private const int MAX_MONTH = 12;
    private const int MIN_YEAR = 2000;
    private const int MAX_YEAR = 2023;

    [SerializeField] private GameObject[] pinTiles;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject pinScriptHandler;
    [SerializeField] private GameObject backgroundAudioSource;
    [SerializeField] private TextMeshProUGUI pinDateText;

    private char[] pinCode = new char[PIN_LENGTH];
    private char[] enteredPin = new char[PIN_LENGTH];
    private int columnIndex = 0;
    private Tile[] pinTileScripts;
    private Image[] pinTileImages;
    private AudioSource backgroundMusic;
    private AudioSource audioSource;
    private TaskManager taskManager;

    private void Awake()
    {
        pinTileScripts = new Tile[pinTiles.Length];
        pinTileImages = new Image[pinTiles.Length];
        for (int i = 0; i < pinTiles.Length; i++)
        {
            pinTileScripts[i] = pinTiles[i].GetComponent<Tile>();
            pinTileImages[i] = pinTiles[i].GetComponent<Image>();
        }

        backgroundMusic = backgroundAudioSource.GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        taskManager = gameManager.GetComponent<TaskManager>();

        InitializePinArrays();
        GeneratePin();
        SetDateText();
    }

    private void Update()
    {
        if (!PinMode) return;

        HandleInput();
    }

    private void InitializePinArrays()
    {
        for (int i = 0; i < PIN_LENGTH; i++)
        {
            enteredPin[i] = '\0';
        }
    }

    private void GeneratePin()
    {
        System.Random random = new System.Random();
        int month = random.Next(1, MAX_MONTH + 1);
        int day = random.Next(1, MAX_DAY + 1);
        int year = random.Next(MIN_YEAR, MAX_YEAR + 1);

        pinCode = $"{month:D2}{day:D2}{year}".ToCharArray();
    }

    private void SetDateText()
    {
        string formattedDate = $"{pinCode[0]}{pinCode[1]}.{pinCode[2]}{pinCode[3]}.{pinCode[4]}{pinCode[5]}{pinCode[6]}{pinCode[7]}";
        pinDateText.text = formattedDate;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            HandleBackspace();
        }

        HandleNumericInput();
    }

    private void HandleBackspace()
    {
        if (columnIndex > 0)
        {
            columnIndex--;
            enteredPin[columnIndex] = '\0';
            UpdateTileDisplay(columnIndex, '\0');
        }
    }

    private void HandleNumericInput()
    {
        foreach (KeyCode key in SUPPORTED_KEYS)
        {
            if (Input.GetKeyDown(key) && columnIndex < PIN_LENGTH)
            {
                char enteredChar = KeyCodeToChar(key);
                enteredPin[columnIndex] = enteredChar;
                UpdateTileDisplay(columnIndex, enteredChar);
                columnIndex++;
                if (columnIndex == PIN_LENGTH)
                {
                    CheckPinCompletion();
                }
                break;
            }
        }
    }

    private char KeyCodeToChar(KeyCode keyCode)
    {
        return (char)('0' + keyCode - KeyCode.Alpha0);
    }

    private void UpdateTileDisplay(int index, char letter)
    {
        pinTileScripts[index].SetLetter(letter);
        UpdateColor(index, letter);
    }

    private void UpdateColor(int index, char enteredChar)
    {
        if (enteredChar == '\0')
        {
            pinTileImages[index].color = Color.black;
        }
        else if (pinCode[index] == enteredChar)
        {
            pinTileImages[index].color = Color.green;
        }
        else
        {
            pinTileImages[index].color = Color.red;
        }
    }

    private void CheckPinCompletion()
    {
        bool pinCorrect = true;
        for (int i = 0; i < PIN_LENGTH; i++)
        {
            if (enteredPin[i] != pinCode[i])
            {
                pinCorrect = false;
                break;
            }
        }

        if (pinCorrect)
        {
            taskManager.musicTaskDone = true;
            backgroundMusic.Stop();
            audioSource.Play();
            PinMode = false;
        }
    }
}
