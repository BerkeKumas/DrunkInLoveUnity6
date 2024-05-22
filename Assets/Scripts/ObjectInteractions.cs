using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class ObjectInteractions : MonoBehaviour
{
    private readonly Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f);

    private const float RAY_LENGTH = 5.0f;
    private const string LAST_LAUNDRY_TEXT = "I think something fell on the ground.";
    private const string HOLDING_KEY_TEXT = "A key I wonder where this opens.";
    private const string COFFEE_TEXT = "I need a coffee...";

    public GameObject lockerObject;

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
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource clockTickingSound;
    [SerializeField] private AudioClip darkAmbientMusic;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private InventorySlot handSlot;
    [SerializeField] private GameObject rotateText;
    [SerializeField] private GameObject chainObject;

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
    private bool startInteraction = true;
    private GameObject holdObject;
    private GameObject rayObject;
    private GameObject laptopObject;
    private GameObject zoomObject;
    private Vector3 cupPos;
    private AudioSource soundAudioSource;
    private CaptionTextTyper captionTextTyper;
    private Outline outlinedObject;
    private bool isRotating = false;

    private void Awake()
    {
        soundAudioSource = soundPlayer.GetComponent<AudioSource>();
        captionTextTyper = captionTextObject.GetComponent<CaptionTextTyper>();
        cupPos = cupPosObject.transform.position;
    }

    private void Update()
    {
        if (!startInteraction) return;

        if (Input.GetKeyDown(KeyCode.R) && !isRotating)
        {
            if (holdObject.tag == "zoomtag")
            {
                StartCoroutine(RotateOverTime(holdObject, 180, 1.0f));
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
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
                rayObject.tag = "doortag";

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

        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, RAY_LENGTH))
        {
            rayObject = hit.transform.gameObject;
            HandleRaycastHit(rayObject);
        }
        else
        {
            ClearAllDisplays();
        }
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
            unlockDoor = false;
            handSlot.ClearSlot();
            PlaySound(2);
            controller.isDoorLocked = false;

            removeUI.Play();
            StartCoroutine(DisableLevelUI());

            clockTickingSound.Stop();
            backgroundMusic.clip = darkAmbientMusic;
            backgroundMusic.Play();

            drunkBar.GetComponent<DrunkBar>().StartFill = false;
            cameraObject.GetComponent<Drunk>().enabled = false;

            captionTextTyper.ResetTextIfEqual(COFFEE_TEXT);
            captionTextTyper.ResetTextIfEqual(HOLDING_KEY_TEXT);

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
            case "laundryboxtag":
                HandleLaundryBoxInteraction();
                break;
            case "doortag":
                HandleDoorInteraction();
                break;
            case "zoomtag":
                interactionText.text = "[F] to Zoom";
                break;
            case "lightswitchtag":
                controlLight = true;
                interactionText.text = rayObject.GetComponent<LightSwitchScript>().IsLightOn ? "[F] to Switch Off Lights" : "[F] to Switch On Lights";
                break;
            case "laptoptag":
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
            default:
                ClearAllDisplays();
                break;
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
        if (laptopObject != null)
        {
            laptopObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }
        controlLight = false;
        doorControl = false;
        dropLaundry = false;
        openLaptop = false;
        unlockDoor = false;
        enterLocker = false;
        openDrawr = false;
        useLockPick = false;
        openChainDoor = false;
        if (outlinedObject != null)
        {
            outlinedObject.enabled = false;
            outlinedObject = null;
        }
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
            drunkBar.GetComponent<DrunkBar>().DecreaseFill(100.0f);
            DestroyObject(holdObject);
        }
        else if (holdObject.tag == "zoomtag")
        {
            DestroyObject(zoomObject);
        }
        else if (rayObject is not { tag: "lockpickdoortag" or "venttag" or "doortag"})
        {
            if (holdObject is { tag: "flashlighttag" or "keytag" or "screwdrivertag" or "boltcuttertag" or "bobbypintag"})
            {
                if (rayObject is { tag: "flashlighttag" or "keytag" or "screwdrivertag" or "boltcuttertag" or "bobbypintag"})
                {
                    RaycastObjectActions();
                    rayObject = null;
                    return;
                }
                else
                {
                    holdObject = handSlot.itemGameObject;
                    handSlot.ClearSlot();
                }
            }
            if (holdObject.tag == "flashlighttag")
            {
                holdObject.GetComponent<Light>().enabled = false;
            }
            else if (holdObject.tag == "winetag")
            {
                rotateText.SetActive(false);
            }
            holdObject.GetComponent<Rigidbody>().isKinematic = false;
            holdObject.GetComponent<BoxCollider>().enabled = true;
            holdObject.transform.parent = null;
            ClearHold();
        }
    }

    public void ClearHold()
    {
        holdingObject = false;
        holdObject = null;
    }

    private void RaycastObjectActions()
    {
        if (rayObject is { tag: "sewergratetag" or "fruittag" or "clothestag" or "cuptag" })
        {
            HoldObject();
        }
        else if (rayObject is { tag: "winetag"})
        {
            HoldObject();
            rotateText.SetActive(true);
            rayObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (rayObject is { tag: "screwdrivertag" })
        {
            HoldObject();
            inventorySystem.AddItemToEmptySlot(rayObject.gameObject);
            rayObject.transform.localEulerAngles = new Vector3(-15, 0, 0);
            rayObject.transform.localPosition = new Vector3(0.7f, -0.75f, -0.75f);
        }
        else if (rayObject is { tag: "flashlighttag" })
        {
            HoldObject();
            inventorySystem.AddItemToEmptySlot(rayObject.gameObject);
            rayObject.GetComponent<Light>().enabled = true;
            rayObject.transform.localEulerAngles = new Vector3(-15, 0, 0);
            rayObject.transform.localPosition = new Vector3(0.7f, -0.75f, -0.75f);
        }
        else if (rayObject is { tag: "boltcuttertag" })
        {
            HoldObject();
            inventorySystem.AddItemToEmptySlot(rayObject.gameObject);
            rayObject.transform.localEulerAngles = new Vector3(0, -105, 15);
            rayObject.transform.localPosition = new Vector3(0.7f, -0.75f, -0.75f);
        }
        else if (rayObject is { tag: "bobbypintag" })
        {
            HoldObject();
            inventorySystem.AddItemToEmptySlot(rayObject.gameObject);
            rayObject.transform.localEulerAngles = new Vector3(0, -105, 15);
            rayObject.transform.localPosition = new Vector3(0.7f, -0.75f, -0.75f);
        }
        else if (rayObject is { tag: "lastlaundrytag" })
        {
            HoldObject();
            DropKey();
            taskManager.GetComponent<TaskManager>().lastLaundryActive = true;
            captionTextTyper.StartType(LAST_LAUNDRY_TEXT, true);
        }
        else if (rayObject is { tag: "keytag" })
        {
            HoldObject();
            inventorySystem.AddItemToEmptySlot(rayObject.gameObject);
            captionTextTyper.StartType(HOLDING_KEY_TEXT, true);
            rayObject.transform.localEulerAngles = new Vector3(-10, -100, -80);
            rayObject.transform.localPosition = new Vector3(0.7f, -0.5f, -0.5f);

        }
        else if (rayObject is { tag: "zoomtag" })
        {
            zoomObject = Instantiate(rayObject, zoomObjectParent.transform.position, Quaternion.identity, zoomObjectParent.transform);
            zoomObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            holdObject = zoomObject.gameObject;
            holdingObject = true;
        }
    }

    private void HoldObject()
    {
        SetHoldObject(rayObject);
        holdObject.transform.GetComponent<Rigidbody>().isKinematic = true;
        holdObject.transform.parent = holdObjectParent.transform;
        holdObject.GetComponent<BoxCollider>().enabled = false;
        holdObject.transform.localPosition = Vector3.zero;
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
        Instantiate(keyObject, holdObjectParent.transform.position, Quaternion.identity);
    }

    public void StartInteractions()
    {
        startInteraction = true;
    }
}