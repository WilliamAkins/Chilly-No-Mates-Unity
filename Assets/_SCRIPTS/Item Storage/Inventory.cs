using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    private ItemDatabase database;
    private Vitals vitals;

    public GameObject playerPhone;

    public AudioClip burpSound;

    public List<Item> foodList = new List<Item>();
    public List<Item> drinkList = new List<Item>();
    public List<Item> clothesList = new List<Item>();
    public List<Item> questList = new List<Item>();
    public List<Item> miscList = new List<Item>();

    public bool canStore = true;

    //IDs start at 0. -1 indicated no item present.
    private int itemHolding = -1;      

    //Identifiers to know if player has certain items
    private bool hasPhone;

    public bool phoneOut;

    private bool storeInput = false;

    private AudioSource audioSource;

    private void Start()
    {
        database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>();

        //gets a reference to the vitals class
        vitals = GameObject.Find("Character").GetComponent<Vitals>();

        audioSource = GetComponent<AudioSource>();
    }
    bool StoreEum()
    {
        if (Input.GetButtonDown("Store") && !storeInput)
        {
            StartCoroutine(resetBool(0.5f)); // change this float here to alter the time
            storeInput = true;
            return true;
        }
        return false;
    }
    IEnumerator resetBool(float seconds)
    {
        float ResumeTime = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < ResumeTime)
        {
            yield return null;
        }
        storeInput = false;
    }

    private void Update()
    {
        //Updates the current status of the phone if the player has found the phone
        if (Input.GetButtonDown("Phone") && hasPhone)
        {
            updatePhone();
        }
        //If the player is holding an item...
        if (itemHolding != -1 && canStore)
        {
            //If the player wishes to store the item...
            if (StoreEum())
            {
                Debug.Log("Storing Item");
                updateItems(itemHolding, true);
                itemHolding = -1;
                GameObject.Find("FPPCamera").GetComponent<PickupDrop>().holdingItem = false;
                Destroy(GameObject.Find("FPPCamera").GetComponent<PickupDrop>().itemInHand.gameObject);
                Debug.Log("Putting away object");
            }

            //If the player wishes to use the item...
            if (Input.GetButtonDown("Use"))
            {
                Debug.Log("Trying to use Item");
                useItem(GameObject.Find("ItemDatabase").GetComponent<ItemDatabase>().items[itemHolding],true);
                GameObject.Find("FPPCamera").GetComponent<PickupDrop>().holdingItem = false;
            }

        }
    }

    public bool ItemSearch(int ID)
    {
        //Pass in an item and the function will return true if the player possesses it or false if not.

        string type = database.items[ID].itemType.ToString();
        switch (type)
        {
            case "Food":
                {
                    for(int i = 0; i < foodList.Count; i++)
                    {
                        if (foodList[i].itemID == ID)
                            return true;
                    }
                    return false;
                }
            case "Drink":
                {
                    for (int i = 0; i < drinkList.Count; i++)
                    {
                        if (drinkList[i].itemID == ID)
                            return true;
                    }
                    return false;
                }
            case "Clothes":
                {
                    for (int i = 0; i < clothesList.Count; i++)
                    {
                        if (clothesList[i].itemID == ID)
                            return true;
                    }
                    return false;
                }
            case "Quest":
                {
                    for (int i = 0; i < questList.Count; i++)
                    {
                        if (questList[i].itemID == ID)
                            return true;
                    }
                    return false;
                }
            case "Misc":
                {
                    for (int i = 0; i < miscList.Count; i++)
                    {
                        if (miscList[i].itemID == ID)
                            return true;
                    }
                    return false;
                }
            default:
                {
                    print("Error: Unknown item type.");
                    return false;
                }
        }
    }

    //Shows/hides the phone
    public void updatePhone()
    {
        phoneOut = !phoneOut;
        GameObject.FindWithTag("Phone").transform.GetChild(0).gameObject.SetActive(phoneOut);
    }

    //Adds or removes items into the system based on bool (true = add)
    public void updateItems(int id, bool addOrRemove)
    { 
        Item storedItem = database.items[id];
        List<Item> typeList = new List<Item>();
        string type = storedItem.itemType.ToString();

        switch (type)
        {
            case "Food":
                {
                    updateList(ref foodList, storedItem, addOrRemove);
                    break;
                }
            case "Drink":
                {
                    updateList(ref drinkList, storedItem, addOrRemove);
                    break;
                }
            case "Clothes":
                {
                    updateList(ref clothesList, storedItem, addOrRemove);
                    break;
                }
            case "Quest":
                {
                    updateList(ref questList, storedItem, addOrRemove);
                    break;
                }
            case "Misc":
                {
                    updateList(ref miscList, storedItem, addOrRemove);
                    break;
                }
        }
    }

    public void updateList (ref List<Item> list, Item current, bool aor)
    {
        bool add = true;
        bool remove = true;
        if (aor == true)
        {
            current.itemQuantity++;
            for (int i = 0; i < list.Count; i++)
            {
                //If the player already has the item, increases the quantity
                if (list[i] == current)
                {
                    list[i].itemQuantity++;
                    add = false;
                }
            }

            //Adds the item to the appropriate list
            if (add == true)
            {
                list.Add(current);
                list = list.OrderBy(g => g.itemName).ToList();
            }
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                //If the player already has the item, decreases the quantity
                if (list[i] == current)
                {
                    list[i].itemQuantity--;
                    remove = false;

                    if (list[i].itemQuantity < 1)
                        remove = true;
                }
            }

            //Adds the item to the appropriate list
            if (remove == true)
            {
                list.Remove(current);
            }
        }

    }

    //Setters and Getters for pickupDrop
    public void setItemHolding(int i)
    {
        itemHolding = i;
    }
    public int getItemHolding()
    {
        return itemHolding;
    }

    public void CollectedCollectable(int itemCollected)
    {
        switch (itemCollected)
        {
            //Add ID's and bools for collectable items eg. wallet, shirt etc.
            case 3:
                {
                    Instantiate(playerPhone, Vector3.zero, Quaternion.identity);

                    hasPhone = true;
                    phoneOut = true;

                    break;
                    //Display message to user saying they found phone here

                }
        }
    }

    public void useItem(Item item, bool fromHand)
    {
        Debug.Log("Using Item: " + item + " - " + fromHand);
        switch (item.itemID)
        {
            //Add cases for each item ID that has an effect when consumed.
            case 0:
                {
                    print("Drunk dat drink");
                    //Add stat increases here
                    vitals.setEnergy(30); //increases energy levels by 30

                    audioSource.PlayOneShot(burpSound, 1.0f);

                    break;
                }
            case 1:
                {
                    print("Drunk dat alc");
                    //Add stat increases here
                    vitals.setSoberness(-30); //lowers your soberness levels by 30

                    audioSource.PlayOneShot(burpSound, 1.0f);

                    break;
                }
            case 2:
                {
                    print("Where did you get this?");
                    //Add stat increases here
                    break;
                }
        }

        //When the item is used, and is 'Destroyed when used', destroys the item
        if (item.destroyWhenUsed == true)
        {
            if (fromHand == true)
            {
                Destroy(GameObject.Find("FPPCamera").GetComponent<PickupDrop>().itemInHand.gameObject);
                itemHolding = -1;
            }
            else
            {
                updateItems(item.itemID, false);
            }
        }
    }

    public void resetInventoryTimerInSeconds(float time)
    {
        StartCoroutine(resetInventoryTimer(time));
    }

    IEnumerator resetInventoryTimer(float time)
    {
        Debug.Log("Resetting Bool");
        canStore = false;
        float ResumeTime = Time.realtimeSinceStartup + time;
        while (Time.realtimeSinceStartup < ResumeTime)
        {
            yield return null;
        }
        canStore = true;
        Debug.Log("Bool reset");
    }
}
