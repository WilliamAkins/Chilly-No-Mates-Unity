using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryApp : MonoBehaviour {

    public Material background;
    public Material homescreen;
    public Inventory inventory;

    private GameObject phone;
    private bool backgroundUpdate = true;
    private int categorySelection = 0;
    private int oldCategorySelection = 1;
    private int itemSelection = 0;
    private int oldItemSelection = 1;


    private bool left = false;
    private bool right = false;
    private bool up = false;
    private bool down = false;
    private bool storeInput = false;

    // Use this for initialization
    void Start () {
        phone = GameObject.FindGameObjectWithTag("Phone").transform.GetChild(0).gameObject;
        inventory = GameObject.FindGameObjectWithTag("inventory").transform.Find("Inventory").GetComponent<Inventory>();
    }
	
	// Update is called once per frame
	void Update () {

        //Updates the background if it is still the home screen.
        if (backgroundUpdate == true)
        {
            phone.transform.GetChild(0).GetComponent<MeshRenderer>().material = background;
            backgroundUpdate = false;
        }

        //Closes the app
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            close();
        }

        //Gets user navigation
        navigateCategory();
    }

    void navigateCategory()
    {

        //Updates selection based on user input
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.LeftArrow) || DpadLeft())
            categorySelection--;
        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.RightArrow) || DpadRight())
            categorySelection++;

        //Ensures the user doesn't exceed number of categories
        if (categorySelection < 0)
            categorySelection = 4;
        if (categorySelection > 4)
            categorySelection = 0;

        //Updates colour of icon and title to represent selection
        if (oldCategorySelection != categorySelection)
        {
            transform.GetChild(oldCategorySelection).gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            transform.GetChild(categorySelection).gameObject.GetComponent<SpriteRenderer>().color = Color.gray;

            transform.Find("Category").GetComponent<TextMesh>().text = transform.GetChild(categorySelection).name;
        }

        //Loads the current category into the list
        load(categorySelection);
        
        //Resets for colour change to function correctly
        oldCategorySelection = categorySelection;
    }
    bool StoreEum()
    {
        if (Input.GetButtonDown("Store") && !storeInput)
        {
            StartCoroutine(resetBool(0.5f));
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
    void load (int categoryLoaded)
    {
        List<Item> itemList = new List<Item>();
        int counter = 0;

        

        //Determines which list to load from function parameter
        switch (categoryLoaded)
        {
            case 0:
                {
                    itemList = inventory.foodList;
                    break;
                }
            case 1:
                {
                    itemList = inventory.drinkList;
                    break;
                }
            case 2:
                {
                    itemList = inventory.clothesList;
                    break;
                }
            case 3:
                {
                    itemList = inventory.questList;
                    break;
                }
            case 4:
                {
                    itemList = inventory.miscList;
                    break;
                }
        }

        //If the current list isn't empty...
        if (itemList.Count != 0)
        {
            //Moves the list up and down
            if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.UpArrow) || DpadUp())
                itemSelection--;
            if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.DownArrow) || DpadDown())
                itemSelection++;
        
            //Prevents user from going out of the end of the list
            if (itemSelection < 0)
                itemSelection = itemList.Count - 1;
            if (itemSelection == itemList.Count)
                itemSelection = 0;

            //Updates the item selected with bold text and makes the previous one normal
            if (oldItemSelection != itemSelection)
            {
                transform.Find("Lists").Find("Items").GetChild(oldItemSelection).gameObject.GetComponent<TextMesh>().fontStyle = FontStyle.Normal;
                transform.Find("Lists").Find("Items").GetChild(oldItemSelection).gameObject.GetComponent<TextMesh>().fontSize = 50;
                transform.Find("Lists").Find("Items").GetChild(itemSelection).gameObject.GetComponent<TextMesh>().fontStyle = FontStyle.BoldAndItalic;
                transform.Find("Lists").Find("Items").GetChild(itemSelection).gameObject.GetComponent<TextMesh>().fontSize = 70;
            }

            //Updates the item description
            transform.Find("Description").GetComponent<TextMesh>().text = itemList[itemSelection].itemDesc;

            transform.Find("Item Icon").gameObject.SetActive(true);
            transform.Find("Item Icon").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ItemIcons/" + itemList[itemSelection].itemName);

            oldItemSelection = itemSelection;

            while (counter < itemList.Count)
            {
                //Displays items and quantites in current list
                transform.Find("Lists").Find("Items").GetChild(counter).GetComponent<TextMesh>().text = itemList[counter].itemName;
                transform.Find("Lists").Find("Quantities").GetChild(counter).GetComponent<TextMesh>().text = itemList[counter].itemQuantity.ToString();
                counter++;

                //Stops overflow
                if (counter == 12)
                    counter = itemList.Count;
            }

            //Fills unfilled slots with blanks
            while(counter < 12)
            {
                transform.Find("Lists").Find("Items").GetChild(counter).GetComponent<TextMesh>().text = "";
                transform.Find("Lists").Find("Quantities").GetChild(counter).GetComponent<TextMesh>().text = "";
                counter++;
            }

        }

        //Clears the list when it is empty and displays messages to the player
        else
        {
            for (int i = 0; i < 12; i++)
            {
                transform.Find("Lists").Find("Items").GetChild(i).GetComponent<TextMesh>().text = "";
                transform.Find("Lists").Find("Quantities").GetChild(i).GetComponent<TextMesh>().text = "";
            }

            transform.Find("Lists").Find("Items").GetChild(0).GetComponent<TextMesh>().text = "Empty";
            transform.Find("Description").GetComponent<TextMesh>().text = "No item \nto show. \nGo find \nsome!";
            transform.Find("Description").GetComponent<TextMesh>().text.Replace("\\n", "\n");
            transform.Find("Item Icon").gameObject.SetActive(false);
        }
        
        if (Input.GetButtonDown("Use"))
        {
            inventory.GetComponent<Inventory>().useItem(itemList[itemSelection], false);
        }

        if (StoreEum())
        {
            inventory.resetInventoryTimerInSeconds(0.1f);

            GameObject item = Instantiate(Resources.Load("ItemPrefabs/" + itemList[itemSelection].itemName)) as GameObject;

            Debug.Log("Spawned Item From Inventory");
            Transform player = GameObject.Find("Character/FPPCamera").transform;
            item.transform.SetParent(player);
            //changing the items position so that it is in a set position when picked up
            item.transform.position = item.transform.parent.position + Camera.main.transform.right * 0.8f + Camera.main.transform.forward - Camera.main.transform.up * 0.08f;
            //getting the rotation of the player to base item rotation off of
            Quaternion playerRotation = player.transform.rotation;
            //adjusting the rotation of the item to a prefered alignment
            item.transform.rotation = playerRotation;
            item.transform.Rotate(Vector3.right, -90);
            item.transform.Rotate(Vector3.forward, 180);
            inventory.setItemHolding(item.GetComponent<ItemID>().itemID);
            player.GetComponentInChildren<PickupDrop>().holdingItem = true;

            
            player.GetComponentInChildren<PickupDrop>().itemInHand = item.GetComponent<Rigidbody>();

            //setting the objects rigid body and turning off collisions
            player.GetComponentInChildren<PickupDrop>().itemInHand.isKinematic = true;
            player.GetComponentInChildren<PickupDrop>().itemInHand.detectCollisions = false;
            player.GetComponentInChildren<PickupDrop>().itemInHand.useGravity = false;
            player.GetComponentInChildren<PickupDrop>().itemInHand.constraints = RigidbodyConstraints.FreezeAll;

            inventory.GetComponent<Inventory>().updateItems(itemList[itemSelection].itemID, false);
            inventory.GetComponent<Inventory>().updatePhone();
        }

    }

    void close()
    {
        //Resets phone to home screen to close the app

        phone.transform.GetChild(0).GetComponent<MeshRenderer>().material = homescreen;
        phone.GetComponent<MobilePhone>().appClosed = true;
        backgroundUpdate = true;
        gameObject.SetActive(false);
    }
    
    //DPadController start
    bool DpadUp()
    {
        if (Input.GetAxisRaw("DPadVertical") == 1 && !up)
        {
            StartCoroutine(resetUpBool(0.5f));
            up = true;
            return true;
        }
        return false;
    }
    bool DpadDown()
    {
        if (Input.GetAxisRaw("DPadVertical") == -1 && !down)
        {
            StartCoroutine(resetDownBool(0.5f));
            down = true;
            return true;
        }
        return false;
    }
    bool DpadLeft()
    {
        if (Input.GetAxisRaw("DPadHorizontal") == -1 && !left)
        {
            StartCoroutine(resetLeftBool(0.5f));
            left = true;
            return true;
        }
        return false;
    }
    bool DpadRight()
    {
        if (Input.GetAxisRaw("DPadHorizontal") == 1 && !right)
        {
            StartCoroutine(resetRightBool(0.5f));
            right = true;
            return true;
        }
        return false;
    }

    IEnumerator resetLeftBool(float seconds)
    {
        float ResumeTime = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < ResumeTime)
        {
            yield return null;
        }
        left = false;
    }
    IEnumerator resetRightBool(float seconds)
    {
        float ResumeTime = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < ResumeTime)
        {
            yield return null;
        }
        right = false;
    }
    IEnumerator resetUpBool(float seconds)
    {
        float ResumeTime = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < ResumeTime)
        {
            yield return null;
        }
        up = false;
    }
    IEnumerator resetDownBool(float seconds)
    {
        float ResumeTime = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < ResumeTime)
        {
            yield return null;
        }
        down = false;
    }

    //DPad Input end



}
