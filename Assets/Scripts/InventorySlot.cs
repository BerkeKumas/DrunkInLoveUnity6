using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private ObjectInteractions objectInteractions;

    public Image itemIcon;
    public string itemTag = string.Empty;
    public GameObject itemGameObject;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private void Awake()
    {
        itemIcon = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        if (itemTag == string.Empty)
        {
            ClearSlot();
        }
    }

    public void AddItem(string _itemTag, Sprite _itemIcon, GameObject _itemGameObject)
    {
        itemTag = _itemTag;
        itemIcon.sprite = _itemIcon;
        itemGameObject = _itemGameObject;
        itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 1);
        if (this.tag != "handtag")
        {
            if (itemGameObject != null)
            {
                itemGameObject.SetActive(false);
            }
        }
        else
        {
            if (itemGameObject != null)
            {
                itemGameObject.SetActive(true);
                objectInteractions.SetHoldObject(itemGameObject);
            }
            else
            {
                objectInteractions.ClearHold();
            }
        }
    }

    public void ClearSlot()
    {
        itemTag = string.Empty;
        itemIcon.sprite = null;
        itemGameObject = null;
        itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = originalPosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (draggedSlot != null && draggedSlot != this)
        {
            SwapSlots(draggedSlot);
        }
    }

    private void SwapSlots(InventorySlot otherSlot)
    {
        string tempItemTag = otherSlot.itemTag;
        Sprite tempItemIcon = otherSlot.itemIcon.sprite;
        GameObject tempItemGameObject = otherSlot.itemGameObject;
        otherSlot.AddItem(itemTag, itemIcon.sprite, itemGameObject);
        this.AddItem(tempItemTag, tempItemIcon, tempItemGameObject);
        if (otherSlot.itemIcon.sprite == null)
        {
            otherSlot.itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 0);
        }
    }
}
