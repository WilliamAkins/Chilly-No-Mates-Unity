using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Item
{
    public string itemName;
    public int itemID;
    public string itemDesc;
    public Texture2D itemIcon;
    public ItemType itemType;
    public bool destroyWhenUsed;
    public int price;
    public int itemQuantity;

    public enum ItemType
    {
        Food,
        Drink,
        Clothes,
        Quest,
        Misc,
        Money
    }

    public Item()
    {
        //Used to create empty inventory slots.
    }

    public Item(string name, int id, string desc, ItemType type, bool destroy, int quantity, int cost) //If equipable or consumable effects are added, create new constructor.
    {
        itemName = name;
        itemID = id;
        itemDesc = desc;
        itemType = type;
        itemIcon = Resources.Load<Texture2D>("ItemIcons/" + itemName);
        destroyWhenUsed = destroy;
        itemQuantity = quantity;
        price = cost;
    }

    public bool getDestroy()
    {
        return destroyWhenUsed;
    }
}
