using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private ObjectInteractions objectInteractions;
    [SerializeField] private InventorySlot handSlot;

    public Image itemIcon;
    public string itemTag = string.Empty;
    public GameObject itemGameObject;
    private int slotIndex = -1;

    private void Awake()
    {
        itemIcon = GetComponent<Image>();
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
            if (this.name == "0" && handSlot.itemGameObject == null)
            {
                SendToHand();
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
        if (this.tag == "handtag" && slotIndex != -1)
        {
            inventorySystem.ClearSlot(slotIndex);
            slotIndex = -1;
        }
        itemTag = string.Empty;
        itemIcon.sprite = null;
        itemGameObject = null;
        itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 0);
    }

    public void SendToHand()
    {
        if (handSlot.itemGameObject != null)
        {
            handSlot.itemGameObject.SetActive(false);
        }
        handSlot.AddItem(itemTag, itemIcon.sprite, itemGameObject);
        handSlot.slotIndex = Convert.ToInt32(this.name);
        if (handSlot.itemIcon.sprite == null)
        {
            handSlot.itemIcon.color = new Color(itemIcon.color.r, itemIcon.color.g, itemIcon.color.b, 0);
        }
    }
}
