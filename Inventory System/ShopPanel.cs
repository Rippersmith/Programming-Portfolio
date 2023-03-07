using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : ItemsPanel
{
    public static ShopPanel instance;
    public List<ScriptObj_InvItem> shopItems;

    private void Awake() { instance = this; }

    void Start()
    {
        shopItems.AddRange(allItems);
        numItemSlots = allItemSlots.Length;
        ChangeItems();
    }

    public void SwitchToBuying()
    {
        allItems.Clear();
        allItems.AddRange(shopItems);
        UpdateItems();
        foreach (ItemSlot invSlot in allItemSlots)
        {
            invSlot.ShopPanel_Buying();
        }
    }

    public void SwitchToSelling()
    {
        allItems.Clear();
        allItems.AddRange(InventoryPanel.instance.allItems);
        UpdateItems();
        foreach (ItemSlot invSlot in allItemSlots)
        {
            invSlot.ShopPanel_Selling();
        }
    }
}
