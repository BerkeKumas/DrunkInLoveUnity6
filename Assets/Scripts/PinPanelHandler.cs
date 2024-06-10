using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.Cinemachine;

public class PinPanelHandler : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H,
        KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P,
        KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z
    };

    private const int PIN_LENGTH = 6;

    [SerializeField] private GameObject[] pinTiles;
    [SerializeField] private ShutterControl ShutterControl;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerLook PlayerLook;
    [SerializeField] private CinemachineCamera pinCam;
    [SerializeField] private ObjectInteractions objectInteractions;
    [SerializeField] private GameObject exitDoor;
    [SerializeField] private GameObject escapeTrigger;

    private bool PinMode = false;
    private char[] pinCode = new char[] { 'N', 'E', 'D', 'L', 'U', 'V' };
    private char[] enteredPin = new char[PIN_LENGTH];
    private int columnIndex = 0;
    private Tile[] pinTileScripts;
    private Image[] pinTileImages;
    private bool isPinEntered = false;

    private void Awake()
    {
        pinTileScripts = new Tile[pinTiles.Length];
        pinTileImages = new Image[pinTiles.Length];
        for (int i = 0; i < pinTiles.Length; i++)
        {
            pinTileScripts[i] = pinTiles[i].GetComponent<Tile>();
            pinTileImages[i] = pinTiles[i].GetComponent<Image>();
        }

        InitializePinArrays();
    }

    private void Update()
    {
        if (!PinMode) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            DisablePinMode();
            return;
        }

        HandleInput();
    }

    private void InitializePinArrays()
    {
        for (int i = 0; i < PIN_LENGTH; i++)
        {
            enteredPin[i] = '\0';
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            HandleBackspace();
        }

        HandleLetterInput();
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

    private void HandleLetterInput()
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
        return (char)('A' + (keyCode - KeyCode.A));
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
            Color customColor;
            if (ColorUtility.TryParseHtmlString("#005CFF", out customColor))
            {
                pinTileImages[index].color = customColor;
            }
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

        if (pinCorrect && !isPinEntered)
        {
            isPinEntered = true;
            escapeTrigger.SetActive(true);
            ShutterControl.ToggleShutter(9.5f, 0);
            transform.tag = "Untagged";
            exitDoor.tag = "doortag";
            DisablePinMode();
        }
    }

    private void OnEnable()
    {
        playerController.enabled = false;
        PlayerLook.enabled = false;
        pinCam.Priority = 2;
        PinMode = true;
    }

    private void DisablePinMode()
    {
        playerController.enabled = true;
        PlayerLook.enabled = true;
        pinCam.Priority = 0;
        PinMode = false;
        this.enabled = false;
    }
}
