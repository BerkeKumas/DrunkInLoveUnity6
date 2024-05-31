using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    private readonly KeyCode[] InventoryKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7 };
    public List<InventoryObjects> inventoryObjects = new List<InventoryObjects>();
    
    [SerializeField] private GameObject InventoryUI;
    [SerializeField] private InventorySlot[] InventorySlots;
    [SerializeField] private Sprite[] iconList;

    private bool isInventoryOpen = false;

    public class InventoryObjects
    {
        public Sprite itemIcon { get; private set; }
        public string itemTag { get; private set; }
        public GameObject itemGameObject { get; set; }

        public InventoryObjects(string objectTag, Sprite icon)
        {
            itemIcon = icon;
            itemTag = objectTag;
        }
    }

    private void Awake()
    {
        inventoryObjects = new List<InventoryObjects>
        {
            new InventoryObjects("flashlighttag", iconList[0]),
            new InventoryObjects("keytag", iconList[1]),
            new InventoryObjects("screwdrivertag", iconList[2]),
            new InventoryObjects("boltcuttertag", iconList[3]),
            new InventoryObjects("bobbypintag", iconList[4])
        };
    }

    private void Update()
    {
        HandleInventoryToggle();
    }

    private void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            InventoryUI.SetActive(isInventoryOpen);
        }
        foreach (KeyCode keys in InventoryKeys)
        {
            if (Input.GetKeyDown(keys))
            {
                int keyNum = (Array.IndexOf(InventoryKeys, keys));
                InventorySlots[keyNum].SendToHand();
            }
        }
    }

    public void AddItemToEmptySlot(GameObject addItem)
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots[i].itemTag == string.Empty)
            {
                foreach (var item in inventoryObjects)
                {
                    if (item.itemTag == addItem.tag)
                    {
                        InventorySlots[i].AddItem(item.itemTag, item.itemIcon, addItem);
                    }
                }
                break;
            }
        }
    }

    public void ClearSlot(int slot)
    {
        InventorySlots[slot].ClearSlot();
    }
}
