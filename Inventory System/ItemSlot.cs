using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[System.Serializable]
public class ItemSlot : MonoBehaviour
{
    //public Sprite itemImage;
    public bool usedSpace = false;
    public ScriptObj_InvItem inventoryItem = null;
    public Image itemDisplay;
    public delegate void BuyOrSellItem();
    BuyOrSellItem bos;

    public void Awake()
    {
        bos = BuyItem;
        itemDisplay = this.GetComponent<Image>();
        if (inventoryItem != null)
            itemDisplay.sprite = inventoryItem.itemImage;
        if (itemDisplay.sprite != null)
            itemDisplay.color = Color.white;
    }

    //function to assign a new image to the inventory slot
    public void NewItemAssigned()
    {
        itemDisplay.sprite = inventoryItem.itemImage;
        if (itemDisplay.sprite != null)
            itemDisplay.color = Color.white;
    }

    //function to make an empty inventory slot's image change to no image
    public void ItemRemoved()
    {
        itemDisplay.sprite = null;
        itemDisplay.color = Color.gray;
    }

    //function that denotes what happens when an inventory slot is clicked on. Calls a function specific to the type of item selected
    public void InventorySlotClicked()
    {        //if no item is assigned to the slot, return an error message
        if (inventoryItem == null)
        {
            Debug.Log("No item listed there, nothing happens");
            InventoryPanel.instance.description.text = "";
            return;
        }

       

        InventoryPanel.instance.description.text = inventoryItem.description;
        //if the item is a weapon, equip a weapon
        if (inventoryItem.itemTag == ScriptObj_InvItem.Tag.Weapon)
        {
            //equip weapon
        }

        //need different functions for each tag, each one will have a different effect
        //depending on the tag
    }

    public void ShopPanel_Selling()
    {
        bos = SellItem;
    }

    public void ShopPanel_Buying()
    {
        bos = BuyItem;
    }

    public void ShopButton()
    {
        bos();
    }

    private void BuyItem()
    {
        if (inventoryItem != null)
        {
            print("buy");
            GameManager._instance.currency -= inventoryItem.itemPrice;
            InventoryPanel.instance.allItems.Add(inventoryItem);
            InventoryPanel.instance.UpdateItems();
            ShopPanel.instance.UpdateItems();
        }
    }

    private void SellItem()
    {
        if (inventoryItem != null)
        {
            print("sell");
            GameManager._instance.currency += inventoryItem.itemSell;
            InventoryPanel.instance.allItems.Remove(inventoryItem);
            inventoryItem = null;
            InventoryPanel.instance.UpdateItems();
            ShopPanel.instance.SwitchToSelling();
        }
    }
}
