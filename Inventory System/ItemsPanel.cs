using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsPanel : MonoBehaviour
{
    public List<ScriptObj_InvItem> allItems;
    public ItemSlot[] allItemSlots; //all of the inventory slots. All of these must be manually assigned
    protected int numItemSlots;
    public int pageNum = 0;

    ScriptObj_InvItem.Tag newInvType;

    //here, the inventory will be redisplayed using items of type "Tag" (if you want to view all weapons or potions, i.e.)
    public void ChangeItems(ScriptObj_InvItem.Tag itemType)
    {
        List<ScriptObj_InvItem> itemList = GetAllItems(itemType);

        //this "for" loop designates the numbers that we will take from the allItemSlots list
        //example: page 0 will display allItemSlots items 0 - 7, page 1 will display 8 - 15, etc
        for (int i = numItemSlots * pageNum; i < numItemSlots * (pageNum + 1); i++)
        {
            // do this if the itemList has at least i items
            if (itemList.Count > i)
            {
                //assign items to an inventory slot
                allItemSlots[i % numItemSlots].inventoryItem = itemList[i]; //(10 % 8 = 2 = 2nd slot)
                allItemSlots[i % numItemSlots].NewItemAssigned();
            }
            // do this if itemList doesn't have at least i items
            else
            {
                //remove the current item in the slot (if any) and make it null
                allItemSlots[i % numItemSlots].inventoryItem = null;
                allItemSlots[i % numItemSlots].ItemRemoved();
            }
        }
    }

    public void ChangeItems()
    {
        //this "for" loop designates the numbers that we will take from the allItemSlots list
        //example: page 0 will display allItemSlots items 0 - 7, page 1 will display 8 - 15, etc
        for (int i = numItemSlots * pageNum; i < numItemSlots * (pageNum + 1); i++)
        {
            // do this if the itemList has at least i items
            if (allItems.Count > i)
            {
                //assign items to an inventory slot
                allItemSlots[i % numItemSlots].inventoryItem = allItems[i]; //(10 % 8 = 2 = 2nd slot)
                allItemSlots[i % numItemSlots].NewItemAssigned();
            }
            // do this if itemList doesn't have at least i items
            else
            {
                //remove the current item in the slot (if any) and make it null
                allItemSlots[i % numItemSlots].inventoryItem = null;
                allItemSlots[i % numItemSlots].ItemRemoved();
            }
        }
    }

    //takes a parameter of ScriptObj_InvItem.InventoryType and returns a list of all InventoryItems of that Tag
    List<ScriptObj_InvItem> GetAllItems(ScriptObj_InvItem.Tag searchForInvType)
    {
        List<ScriptObj_InvItem> newInvItemList = new List<ScriptObj_InvItem>();

        foreach (ScriptObj_InvItem item in allItems)
        {
            if (item.itemTag == searchForInvType)
            {
                newInvItemList.Add(item);
            }
        }

        return newInvItemList;
    }

    //function for the Next Page button
    public void NextPage()
    {
        //if the current inventory boxes * the next page number would be
        //greater than all of the items on the page, then skip this
        //if ((allItemSlots.Length * (pageNum + 1)) < GetAllItems(newInvType).Count)
        if ((allItemSlots.Length * (pageNum + 1)) < allItems.Count)
        {
            pageNum++;
            ChangeItems();
        }
    }

    //function for the Previous Page button
    public void PreviousPage()
    {
        if (pageNum > 0) //if it is the last page (page 0), skip this
        {
            pageNum--;
            ChangeItems();
        }
    }

    public void UpdateItems()
    {
        ChangeItems();
    }

}
