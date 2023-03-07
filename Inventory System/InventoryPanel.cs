using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : ItemsPanel
{
    public static InventoryPanel instance;

    public Text description;

    private void Awake() { instance = this; print(instance.description); }

    void Start()
    {
        allItems = GameManager._instance.allSavedItems;
        numItemSlots = allItemSlots.Length;
        ChangeItems();
    }

}
