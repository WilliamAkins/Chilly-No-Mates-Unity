using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    void Awake()        //Awake used to fix execution order issue.
    {
        //Add all items in the game here
        items.Add(new Item(name: "Energy Drink", id: 0, desc: "Increases\nyour speed\nfor a short\n amount of\ntime.", type: Item.ItemType.Drink, destroy: true, quantity: 0, cost: 100));
        items.Add(new Item(name: "Premium Larger", id: 1, desc: "This will\n f%$k you up.", type: Item.ItemType.Drink, destroy: true, quantity: 0, cost: 150));
        items.Add(new Item(name: "Police Hat", id: 2, desc: "Definitely should not have this.", type: Item.ItemType.Drink, destroy: false, quantity: 0, cost: 100));
        items.Add(new Item(name: "Mobile Phone", id: 3, desc: "Mobile Phone", type: Item.ItemType.Drink, destroy: false, quantity: 0, cost: 100));
        items.Add(new Item(name: "Red T-Shirt", id: 4, desc: "Classic\nRed T-Shirt", type: Item.ItemType.Clothes, destroy: false, quantity: 0, cost: 100));
        items.Add(new Item(name: "Money Stack", id: 5, desc: "Small Stack of Money", type: Item.ItemType.Money, destroy: false, quantity: 0, cost: 0));
        items.Add(new Item(name: "Jeans", id: 6, desc: "Jeans", type: Item.ItemType.Clothes, destroy: false, quantity: 0, cost: 100));
        items.Add(new Item(name: "Shoes", id: 7, desc: "Shoes", type: Item.ItemType.Clothes, destroy: false, quantity: 0, cost: 100));
        items.Add(new Item(name: "Socks", id: 8, desc: "Socks", type: Item.ItemType.Clothes, destroy: false, quantity: 0, cost: 100));
        items.Add(new Item(name: "Hat", id: 9, desc: "Hat", type: Item.ItemType.Clothes, destroy: false, quantity: 0, cost: 100));
    }
    //ITEM DESCRIPTIONS: Need to be implemented with \n to make new lines. No text wrap so must be done manually. Place a \n every 12 characters.
}
