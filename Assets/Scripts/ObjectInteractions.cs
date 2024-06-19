using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ObjectInteractions : MonoBehaviour
{
    private readonly Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f);

    private const float RAY_LENGTH = 5.0f;
    private const string LAST_LAUNDRY_TEXT = "I think something fell on the ground.";
    private const string HOLDING_KEY_TEXT = "A key I wonder where this opens.";
    private const string COFFEE_TEXT = "I need a coffee...";
    private const string BATHROOM_TEXT = "Bathroom can wait.";
    private const string FINGER_HINT = "A guy's ring finger is missing, look like its cut off.";
    private const string TATTOO_HINT = "His arm looks so bad, it looks like a piece of skin has been torn off.";
    private const string MEAT_HINT = "An order tag, half of it torn off. It's her handwriting. Who is Ulrich?";
    private const string HANG_HINT = "The man hang himself, there is mentioned a missing persons report.";
    private const string TATTOO_NOTE_HINT = "Tatto reminder for a guy named Dean.";
    private const string UNLOCK_DOOR_TEXT = "This door goes down to the basement, Rose would never let me in here.";

    public GameObject lockerObject;

    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject levelUI;
    [SerializeField] private GameObject taskManager;
    [SerializeField] private GameObject holdObjectParent;
    [SerializeField] private GameObject zoomObjectParent;
    [SerializeField] private GameObject drunkBar;
    [SerializeField] private GameObject cupObject;
    [SerializeField] private GameObject cupPosObject;
    [SerializeField] private GameObject soundPlayer;
    [SerializeField] private GameObject keyObject;
    [SerializeField] private AudioClip[] soundEffects;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private TextMeshProUGUI captionTextObject;
    [SerializeField] private PlayableDirector removeUI;
    [SerializeField] private AudioSource houseBackgroundMusic;
    [SerializeField] private AudioSource basementBackgroundMusic;
    [SerializeField] private AudioSource clockTickingSound;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private InventorySlot handSlot;
    [SerializeField] private GameObject rotateText;
    [SerializeField] private GameObject chainObject;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private MotionCameraEffects motionCameraEffects;
    [SerializeField] private GameObject moveObjectParent;
    [SerializeField] private GameObject readLight;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private JournelControl journelControl;
    [SerializeField] private Volume postProcess;
    [SerializeField] private VolumeProfile basementProfile;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Transform wayPoint3;
    [SerializeField] private Transform wayPoint4;
    [SerializeField] private Transform newWayPoint3;
    [SerializeField] private Transform newWayPoint4;
    [SerializeField] private TextMeshProUGUI openInventoryText;
    [SerializeField] private AudioSource openInvSource;
    [SerializeField] private GameObject easterEgg;
    [SerializeField] private AudioSource partyHonk;

    private bool holdingObject = false;
    private bool dropLaundry = false;
    private bool openLaptop = false;
    private bool doorControl = false;
    private bool unlockDoor = false;
    private bool controlLight = false;
    private bool enterLocker = false;
    private bool openDrawr = false;
    private bool useLockPick = false;
    private bool openChainDoor = false;
    private bool openWardrobe = false;
    private bool enableDrag = false;
    private bool startInteraction = false;
    private bool stopRay = false;
    private bool enableClimb = false;
    private bool enableZoom = false;
    private bool isZoomed = false;
    private bool missingFingerBool = true;
    private bool ventHintBool = true;
    private bool isEnemyActivated = false;
    private GameObject holdObject;
    private GameObject rayObject;
    private GameObject newRayObject;
    private GameObject laptopObject;
    private GameObject zoomObject;
    private GameObject moveObject;
    private Vector3 cupPos;
    private AudioSource soundAudioSource;
    private CaptionTextTyper captionTextTyper;
    private Outline outlinedObject;
    private bool isRotating = false;
    private bool isFlashlightEnabled = false;
    private bool isFlashlightChanging = false;
    private bool enablePinPanel = false;
    private bool tattooHintBool = true;
    private bool meatHintBool = true;
    private bool hangHintBool = true;
    private bool tattooNoteHintBool = true;
    private bool enableCoffee = true;
    private bool isMoveActivated = false;
    private bool isInvTextActivated = false;
    private string currentHandItem;
    private string newHandItem;
    private FilmGrain filmGrain;
    private int lastLaundryCounter = 0;
    private bool sendRay = false;
    private Rigidbody playerRb;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        soundAudioSource = soundPlayer.GetComponent<AudioSource>();
        captionTextTyper = captionTextObject.GetComponent<CaptionTextTyper>();
        cupPos = cupPosObject.transform.position;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("isDeadByEnemy") == 1)
        {
            startInteraction = true;

            clockTickingSound.Stop();
            houseBackgroundMusic.Stop();
            basementBackgroundMusic.Play();
            drunkBar.GetComponent<DrunkBar>().StartFill = false;
            cameraObject.GetComponent<Drunk>().enabled = false;
        }
    }

    private void Update()
    {
        if (!startInteraction) return;

        if (enableDrag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isMoveActivated = true;
                interactionText.text = string.Empty;
                stopRay = true;
                moveObject.transform.SetParent(moveObjectParent.transform);
                if (!moveObject.GetComponent<AudioSource>().isPlaying)
                {
                    moveObject.GetComponent<AudioSource>().Play();
                }
                moveObject.tag = "stairsdragtag";
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isMoveActivated = false;
                if (moveObject != null)
                {
                    if (moveObject.GetComponent<AudioSource>().isPlaying)
                    {
                        moveObject.GetComponent<AudioSource>().Stop();
                    }
                    moveObject.tag = "dragtag";
                    moveObject.transform.SetParent(null);
                }
                stopRay = false;
            }
        }
        else if (holdObject is { tag: "flashlighttag" })
        {
            if (Input.GetMouseButtonDown(0) && !isFlashlightChanging)
            {
                isFlashlightChanging = true;
                StartCoroutine(FlashlightToggle());
            }
        }

        if (moveObject != null)
        {
            CheckMoveObjectParent();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isRotating)
        {
            if (zoomObject != null)
            {
                StartCoroutine(RotateOverTime(zoomObject, 180, 1.0f));
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && !isRotating)
        {
            if (openLaptop)
            {
                laptopObject.GetComponent<PinManager>().ShouldEnterPin = !laptopObject.GetComponent<PinManager>().ShouldEnterPin;
            }
            else if (controlLight)
            {
                rayObject.GetComponent<LightSwitchScript>().IsLightOn = !rayObject.GetComponent<LightSwitchScript>().IsLightOn;
            }
            else if (dropLaundry)
            {
                PlaySound(0);
                DestroyObject(holdObject);
            }
            else if (doorControl)
            {
                ControlDoor();
            }
            else if (enterLocker)
            {
                lockerObject = rayObject.gameObject;
                rayObject.GetComponent<LockerHide>().ControlPlayerHide();
            }
            else if (openDrawr)
            {
                rayObject.GetComponent<DrawerController>().ToggleDrawer();
            }
            else if (useLockPick)
            {
                interactionText.text = string.Empty;
                rayObject.GetComponent<LockpickControl>().enabled = true;
            }
            else if (openChainDoor)
            {
                chainObject.SetActive(false);
                wayPoint3.position = newWayPoint3.position;
                wayPoint4.position = newWayPoint4.position;
                rayObject.tag = "doortag";
                sendRay = true;
            }
            else if (openWardrobe)
            {
                rayObject.GetComponent<WardrobeControl>().ToggleDoors();
            }
            else if (enableClimb)
            {
                enableClimb = false;
                StartCoroutine(ClimbStairs(rayObject.transform.position + new Vector3(0, 1, 0)));
            }
            else if (enablePinPanel)
            {
                interactionText.text = string.Empty;
                rayObject.GetComponent<PinPanelHandler>().enabled = true;
            }
            else if (enableZoom)
            {
                ToggleZoom();
            }
            else if (holdingObject)
            {
                HoldingObjectActions();
            }
            else
            {
                RaycastObjectActions();
            }
        }

        if (Camera.main == null) return;
        if (stopRay) return;

        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, RAY_LENGTH, interactableLayers))
        {
            newRayObject = hit.transform.gameObject;
            newHandItem = handSlot.itemTag;
            if (rayObject != newRayObject || currentHandItem != newHandItem || sendRay)
            {
                ClearAllDisplays();
                currentHandItem = newHandItem;
                rayObject = newRayObject;
                HandleRaycastHit(rayObject);
            }
        }
        else
        {
            rayObject = null;
            ClearAllDisplays();
        }
    }

    private void CheckMoveObjectParent()
    {
        if (isMoveActivated && moveObject.transform.parent == null)
        {
            isMoveActivated = false;
            if (moveObject != null)
            {
                if (moveObject.GetComponent<AudioSource>().isPlaying)
                {
                    moveObject.GetComponent<AudioSource>().Stop();
                }
                moveObject.tag = "dragtag";
                moveObject.transform.SetParent(null);
            }
            stopRay = false;
        }
    }

    private void ToggleZoom()
    {
        isZoomed = !isZoomed;
        if (isZoomed)
        {
            playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            stopRay = true;
            interactionText.text = string.Empty;
            playerController.enabled = false;
            playerLook.enabled = false;
            motionCameraEffects.enabled = false;
            readLight.SetActive(true);
            zoomObject = Instantiate(rayObject, zoomObjectParent.transform.position, Quaternion.identity, zoomObjectParent.transform);
            zoomObject.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
            zoomObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            playerController.enabled = true;
            playerLook.enabled = true;
            motionCameraEffects.enabled = true;
            readLight.SetActive(false);
            Destroy(zoomObject);
            enableZoom = false;
            stopRay = false;
        }
    }

    private IEnumerator FlashlightToggle()
    {
        isFlashlightEnabled = !isFlashlightEnabled;
        holdObject.GetComponent<AudioSource>().Play();
        holdObject.transform.GetChild(0).GetComponent<Light>().enabled = isFlashlightEnabled;
        yield return new WaitForSeconds(0.2f);
        isFlashlightChanging = false;
    }

    private IEnumerator ClimbStairs(Vector3 endPosition)
    {
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        transform.position = endPosition;
    }

    private IEnumerator RotateOverTime(GameObject obj, float angle, float duration)
    {
        isRotating = true;
        Quaternion startRotation = obj.transform.localRotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, angle, 0);
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            obj.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.localRotation = endRotation;
        isRotating = false;
    }

    private void ControlDoor()
    {
        DoorController controller = rayObject.GetComponent<DoorController>();
        if (!controller.isDoorLocked)
        {
            controller.ToggleDoor();
        }
        else if (unlockDoor)
        {
            handSlot.ClearSlot();

            enableCoffee = false;
            unlockDoor = false;
            PlaySound(2);
            controller.isDoorLocked = false;

            removeUI.Play();
            StartCoroutine(DisableLevelUI());

            clockTickingSound.Stop();
            houseBackgroundMusic.Stop();
            basementBackgroundMusic.Play();

            drunkBar.GetComponent<DrunkBar>().StartFill = false;
            cameraObject.GetComponent<Drunk>().enabled = false;

            captionTextTyper.ResetTextIfEqual(COFFEE_TEXT);
            captionTextTyper.ResetTextIfEqual(HOLDING_KEY_TEXT);
            captionTextTyper.StartType(UNLOCK_DOOR_TEXT, false);

            DestroyObject(holdObject);
        }
    }

    private IEnumerator DisableLevelUI()
    {
        yield return new WaitForSeconds(1.0f);
        levelUI.SetActive(false);
    }

    private void HandleRaycastHit(GameObject rayObject)
    {
        if (rayObject.GetComponent<Outline>() != null)
        {
            if (outlinedObject != null)
            {
                outlinedObject.enabled = false;
                outlinedObject = null;
            }
            outlinedObject = rayObject.GetComponent<Outline>();
            outlinedObject.enabled = true;
        }
        switch (rayObject.tag)
        {
            case "bobbypintag":
            case "boltcuttertag":
            case "screwdrivertag":
            case "lastlaundrytag":
            case "sewergratetag":
            case "flashlighttag":
            case "clothestag":
            case "fruittag":
            case "winetag":
            case "keytag":
            case "cuptag":
                interactionText.text = "[F] to Hold Object";
                break;
            case "wardrobetag":
                interactionText.text = "[F] to Open";
                openWardrobe = true;
                break;
            case "laundryboxtag":
                HandleLaundryBoxInteraction();
                break;
            case "doortag":
                HandleDoorInteraction();
                break;
            case "zoomtag":
                if (hangHintBool && rayObject.name == "hanghint")
                {
                    hangHintBool = false;
                    journelControl.AddNewLine(HANG_HINT);
                }
                else if (tattooNoteHintBool && rayObject.name == "tattoohint")
                {
                    tattooNoteHintBool = false;
                    journelControl.AddNewLine(TATTOO_NOTE_HINT);
                }
                interactionText.text = "[F] to Zoom";
                enableZoom = true;
                break;
            case "lightswitchtag":
                controlLight = true;
                interactionText.text = rayObject.GetComponent<LightSwitchScript>().IsLightOn ? "[F] to Switch Off Lights" : "[F] to Switch On Lights";
                break;
            case "laptoptag":
                interactionText.text = "[F] to Enter";
                openLaptop = true;
                laptopObject = rayObject;
                laptopObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                break;
            case "lockertag":
                enterLocker = true;
                interactionText.text = "[F] to Hide";
                break;
            case "lockpickdoortag":
                if (handSlot.itemTag == "bobbypintag")
                {
                    useLockPick = true;
                    interactionText.text = "[F] to use Lock Pick";
                }
                else
                {
                    captionTextTyper.StartType("I need something to open this...", false);
                }
                break;
            case "chaindoortag":
                if (handSlot.itemTag == "boltcuttertag")
                {
                    openChainDoor = true;
                    interactionText.text = "[F] to Break Chain";
                }
                else
                {
                    captionTextTyper.StartType("I need something to open this...", false);
                }
                break;
            case "drawertag":
                openDrawr = true;
                interactionText.text = "[F] to Open";
                break;
            case "dragtag":
                interactionText.text = "[LMB] to Drag";
                enableDrag = true;
                moveObject = rayObject;
                break;
            case "stairstag":
                interactionText.text = "[F] to Climb the Stairs";
                enableClimb = true;
                break;
            case "codepaneltag":
                interactionText.text = "[F] to Enter";
                enablePinPanel = true;
                break;
            case "bathroomdoortag":
                captionTextTyper.StartType(BATHROOM_TEXT, false);
                break;
            case "missingfingertag":
                if (missingFingerBool)
                {
                    journelControl.AddNewLine(FINGER_HINT);
                    rayObject.GetComponent<BoxCollider>().enabled = false;
                    missingFingerBool = false;
                    captionTextTyper.StartType("Oh my god, his ring finger is gone! I'm going to throw up!", false);
                }
                break;
            case "venthinttag":
                if (ventHintBool)
                {
                    ventHintBool = false;
                    captionTextTyper.StartType("Did he draw this? Is it a clue? A vent?", false);
                }
                break;
            case "tattoohinttag":
                if (tattooHintBool)
                {
                    journelControl.AddNewLine(TATTOO_HINT);
                    tattooHintBool = false;
                    captionTextTyper.StartType("What happened to his arm?", false);
                }
                break;
            case "meattag":
                if (meatHintBool)
                {
                    journelControl.AddNewLine(MEAT_HINT);
                    meatHintBool = false;
                    captionTextTyper.StartType("This meat smells awful.", false);
                }
                break;
            case "lovetag":
                captionTextTyper.StartType("Gaspar Noé was her favorite director.", false);
                break;
            case "basementcaptiontag":
                rayObject.GetComponent<BoxCollider>().enabled = false;
                RenderSettings.fog = true;
                StartCoroutine(ChangeProfile());
                break;
            case "escapetriggertag":
                enemyController.isHeardSound = true;
                rayObject.SetActive(false);
                break;
            default:
                ClearAllDisplays();
                break;
        }
    }

    private IEnumerator ChangeProfile()
    {
        float elapsedTime = 0f;
        float totalTime = 0.6f;
        Color fadeColor = fadeImage.color;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalTime;
            fadeColor.a = t;
            fadeImage.color = fadeColor;
            yield return null;
        }

        if (basementProfile.TryGet<FilmGrain>(out filmGrain))
        {
            filmGrain.intensity.value = 0.7f;
            filmGrain.response.value = 0.4f;
        }

        postProcess.profile = basementProfile;

        captionTextTyper.StartType("What... What the fuck is this place?", false);
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            float t = elapsedTime / totalTime;
            fadeColor.a = t;
            fadeImage.color = fadeColor;
            yield return null;
        }
    }

    private void HandleLaundryBoxInteraction()
    {
        if (holdObject is { tag: "clothestag" or "lastlaundrytag" })
        {
            interactionText.text = "[F] to Put Clothes into Laundry Box";
            dropLaundry = true;
        }
    }

    private void HandleDoorInteraction()
    {
        doorControl = true;
        DoorController controller = rayObject.GetComponent<DoorController>();
        if (controller.isDoorLocked)
        {
            if (holdObject is { tag: "keytag" })
            {
                interactionText.text = "[F] to Unlock";
                unlockDoor = true;
            }
            else
            {
                interactionText.text = "Looks like it's locked";
            }
        }
        else
        {
            interactionText.text = controller.IsDoorOpen() ? "[F] to Close." : "[F] to Open";
        }
    }

    private void ClearAllDisplays()
    {
        interactionText.text = string.Empty;
        controlLight = false;
        doorControl = false;
        dropLaundry = false;
        openLaptop = false;
        unlockDoor = false;
        enterLocker = false;
        openDrawr = false;
        useLockPick = false;
        openChainDoor = false;
        openWardrobe = false;
        enableDrag = false;
        enableClimb = false;
        enablePinPanel = false;
        enableZoom = false;
        if (outlinedObject != null)
        {
            outlinedObject.enabled = false;
            outlinedObject = null;
        }
        moveObject = null;
    }

    private void HoldingObjectActions()
    {
        if (dropLaundry)
        {
            PlaySound(0);
            DestroyObject(holdObject);
        }
        else if (holdObject.tag == "cuptag")
        {
            PlaySound(1);
            Instantiate(cupObject, cupPos, Quaternion.identity);
            if (drunkBar.activeSelf)
            {
                drunkBar.GetComponent<DrunkBar>().DecreaseFill(100.0f);
            }
            DestroyObject(holdObject);
        }
        else if (rayObject is not { tag: "lockpickdoortag" or "venttag" or "doortag" or "chaindoortag" or "drawertag" or "codepaneltag" or "zoomtag" })
        {
            if (holdObject is { tag: "flashlighttag" or "keytag" or "screwdrivertag" or "boltcuttertag" or "bobbypintag" })
            {
                if (rayObject is { tag: "flashlighttag" or "keytag" or "screwdrivertag" or "boltcuttertag" or "bobbypintag" })
                {
                    RaycastObjectActions();
                    rayObject = null;
                }
                return;
            }
            if (holdObject.tag == "winetag")
            {
                rotateText.SetActive(false);
            }
            holdObject.GetComponent<Rigidbody>().isKinematic = false;
            holdObject.GetComponent<BoxCollider>().enabled = true;
            holdObject.transform.parent = null;
            ClearHold();
            handSlot.ClearSlot();
        }
    }

    public void ClearHold()
    {
        holdingObject = false;
        holdObject = null;
    }

    private void RaycastObjectActions()
    {
        if (rayObject != null)
        {
            switch (rayObject.tag)
            {
                case "sewergratetag":
                case "fruittag":
                case "clothestag":
                    HoldObject();
                    break;
                case "cuptag":
                    if (enableCoffee)
                    {
                        HoldObject();
                    }
                    break;
                case "winetag":
                    HoldObject();
                    rotateText.SetActive(true);
                    rayObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                    break;
                case "screwdrivertag":
                    HoldObject();
                    inventorySystem.AddItemToEmptySlot(rayObject.gameObject);
                    rayObject.transform.localEulerAngles = new Vector3(-15, 0, 0);
                    rayObject.transform.localPosition = new Vector3(0.7f, -0.75f, -0.75f);
                    break;
                case "flashlighttag":
                    if (!isInvTextActivated)
                    {
                        isInvTextActivated = true;
                        StartCoroutine(ActivateInventoryText());
                    }
                    HoldObject();
                    isFlashlightEnabled = false;
                    inventorySystem.AddItemToEmptySlot(rayObject.gameObject);
                    rayObject.transform.localEulerAngles = new Vector3(-6, -4, 0);
                    rayObject.transform.localPosition = new Vector3(0.7f, -0.75f, -0.75f);
                    break;
                case "boltcuttertag":
                    HoldObject();
                    inventorySystem.AddItemToEmptySlot(rayObject.gameObject);
                    rayObject.transform.localEulerAngles = new Vector3(0, -105, 15);
                    rayObject.transform.localPosition = new Vector3(0.7f, -0.75f, -0.75f);
                    break;
                case "bobbypintag":
                    HoldObject();
                    inventorySystem.AddItemToEmptySlot(rayObject.gameObject);
                    rayObject.transform.localEulerAngles = new Vector3(0, -105, 15);
                    rayObject.transform.localPosition = new Vector3(0.7f, -0.75f, -0.75f);
                    if (!isEnemyActivated)
                    {
                        isEnemyActivated = true;
                        enemyController.gameObject.SetActive(true);
                        enemyController.EnterHome();
                    }
                    break;
                case "lastlaundrytag":
                    lastLaundryCounter++;
                    if (lastLaundryCounter < 4)
                    {
                        HoldObject();
                        DropKey();
                        taskManager.GetComponent<TaskManager>().lastLaundryActive = true;
                        captionTextTyper.StartType(LAST_LAUNDRY_TEXT, true);
                    }
                    else if (lastLaundryCounter == 4)
                    {
                        easterEgg.transform.position = new Vector3(rayObject.transform.position.x, rayObject.transform.position.y + 1.0f, rayObject.transform.position.z);
                        easterEgg.SetActive(true);
                        partyHonk.Play();
                        captionTextTyper.StartType("CONGRATS! YOU FOUND A BUG!", false);
                    }
                    break;
                case "keytag":
                    HoldObject();
                    inventorySystem.AddItemToEmptySlot(rayObject.gameObject);
                    rayObject.transform.localEulerAngles = new Vector3(-10, -100, -80);
                    rayObject.transform.localPosition = new Vector3(0.7f, -0.5f, -0.5f);
                    break;
            }
        }
    }

    private IEnumerator ActivateInventoryText()
    {
        openInvSource.Play();
        openInventoryText.gameObject.SetActive(true);

        float elapsedTime = 0.0f;
        float totalTime = 1.0f;
        Color newColor = openInventoryText.color;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / totalTime;
            newColor.a = alpha;
            openInventoryText.color = newColor;
            yield return null;
        }

        elapsedTime = totalTime;
        yield return new WaitForSeconds(1.0f);

        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            float alpha = elapsedTime / totalTime;
            newColor.a = alpha;
            openInventoryText.color = newColor;
            yield return null;
        }
        openInventoryText.gameObject.SetActive(false);
    }

    private void HoldObject()
    {
        rayObject.transform.GetComponent<Rigidbody>().isKinematic = true;
        rayObject.transform.parent = holdObjectParent.transform;
        rayObject.GetComponent<BoxCollider>().enabled = false;
        rayObject.transform.localPosition = Vector3.zero;
        if (handSlot.itemTag == string.Empty)
        {
            SetHoldObject(rayObject);
        }
    }

    public void SetHoldObject(GameObject _holdObject)
    {
        PlaySound(0);
        holdObject = _holdObject;
        holdingObject = true;
    }

    private void DestroyObject(GameObject obj)
    {
        Destroy(obj);
        holdObject = null;
        holdingObject = false;
    }

    private void PlaySound(int index)
    {
        soundAudioSource.clip = soundEffects[index];
        soundAudioSource.Play();
    }

    private void DropKey()
    {
        Instantiate(keyObject, holdObjectParent.transform.position, Quaternion.Euler(90, 0, 0));
    }

    public void StartInteractions()
    {
        startInteraction = true;
    }
}