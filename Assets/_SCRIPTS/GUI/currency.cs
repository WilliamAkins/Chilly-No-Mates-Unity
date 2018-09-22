using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class currency : MonoBehaviour
{

    public static bool transactionScreen = false;
    public static bool transaction = false;
    public static bool buying = false;
    public static bool selling = false;
    public static bool confirm = false;
    public static bool congrats = false;
    public static bool refuse = false;
    //bool to prevent multiply vendor screens from being opened
    public static bool clicked = false;
    public bool close = false;

    public GameObject confirmationScreen;
    public GameObject sellingScreen;
    public GameObject transactionUI;
    public GameObject buyingScreen;
    public GameObject congratsScreen;
    public GameObject refusedScreen;
    public GameObject[] buyingButtons;
    public Text[] itemButtons;
    public Text[] itemCosts;
    public Text funds;
    public Text congratsText;
    //index 0=item name and price, index 1=item description, index 2=players funds
    //SET AS ARRAY INSTEAD OF SEPERATE TO TRY HELP REMOVE ANY CONFUSION WITH PREVIOUS PUBLIC TEXT VALUES
    public Text[] confirmationScreenTexts;
    

    //references to other game objects required
    private GameObject player;
    private funds playerMoney;
    private GameObject inventorySystem;
    private Inventory inventory;
    private VendorSupplies vendorSupplies;
    private ItemDatabase database;
    private Item itemRef;

    private RaycastHit vendorRef;

    // Use this for initialization
    void Start()
    {
        inventorySystem = GameObject.Find("InventorySystem");
        player = GameObject.Find("Character");
        playerMoney = player.GetComponent<funds>();
        inventory = inventorySystem.GetComponentInChildren<Inventory>();
        database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!clicked)
        {
            transactionScreen = true;
            transaction = true;
            clicked = true;
            vendorSupplies = vendorRef.transform.GetComponent<VendorSupplies>();
        }
        if (transaction)
        {
            if (transactionScreen)
            {
                VendorScreen();
            }
            else if (buying)
            {
                BuyingUI();
            }
            else if (selling)
            {
                SellingUI();
            }
            else if (confirm)
            {
                ConfirmPurchaseUI();
            }
            else if(congrats)
            {
                accepted();
            }
            else if(refuse)
            {
                refused();
            }
        }
    }

    void VendorScreen()
    {
        //bringing up the ui and pausing the game
        transactionUI.SetActive(true);
        Time.timeScale = 0f;
        //reallowing player to see and use their mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //function to be used with on click for buying button
    public void BuyingUI()
    {
        buying = true;
        transactionScreen = false;
        transactionUI.SetActive(false);
        buyingScreen.SetActive(true);
        //setting the correct text for each of the items in the buttons
        for (int i = 0; i < 4; i++)
        {
            //checking if an item has been set in the vendor supplies
            if (vendorSupplies.supplies[i] > -1)
            {
                itemRef = database.items[vendorSupplies.supplies[i]];
                itemButtons[i].text = itemRef.itemName;
                itemCosts[i].text = "Cost: " + itemRef.price.ToString();
            }
            else if (vendorSupplies.supplies[i] < 0)
            {
                buyingButtons[i].SetActive(false);
            }
        }
        funds.text = "Funds: " + playerMoney.money.ToString();
    }

    public void ReturnToGame()
    {
        transactionUI.SetActive(false);
        transaction = false;
        clicked = false;
        inventorySystem = null;
        close = true;
    }

    public void exitBuyingPanel()
    {
        //turning on the initial transaction screen back on
        //also turning off the buying screen
        buyingScreen.SetActive(false);
        transactionUI.SetActive(true);
        //setting transaction back to true so that only the transaction screen is showing
        buying = false;
        transactionScreen = true;
    }

    //SELLING SCREEN FUNCTIONS
    public void SellingUI()
    {
        selling = true;
        transactionScreen = false;
        transactionUI.SetActive(false);
        sellingScreen.SetActive(true);
    }

    public void exitSellingPanel()
    {
        sellingScreen.SetActive(false);
        transactionUI.SetActive(true);
        selling = false;
        transactionScreen = true;
    }

    //FUNCTIONS RELATED TO THE CONFIRMATION OF PURCHASE SCREEN
    public void ConfirmPurchaseUI()
    {
        //finding which item was selected to buy based off of which button was pressed
        for (int i = 0; i < 4; i++)
        {
            if (EventSystem.current.currentSelectedGameObject.name == "Item" + (i + 1))
            {
                itemRef = database.items[vendorSupplies.supplies[i]];
            }
        }
        
        buying = false;
        confirm = true;
        buyingScreen.SetActive(false);
        confirmationScreen.SetActive(true);
        confirmationScreenTexts[0].text = itemRef.itemName + "  Cost: " + itemRef.price;
        confirmationScreenTexts[1].text = itemRef.itemDesc;
        confirmationScreenTexts[2].text = playerMoney.money.ToString();
    }

    public void DeclinePurchase()
    {
        buyingScreen.SetActive(true);
        confirmationScreen.SetActive(false);
        confirm = false;
        transactionScreen = false;
        buying = true;     
    }

    //FUNCTIONS RELATED TO WHEN A PURCHASE IS ATTEMPTED
    public void AttemptPurchase()
    {
        confirmationScreen.SetActive(false);
        confirm = false;
        if (itemRef.price <= playerMoney.money)
        {
            //adding item to inventory
            inventory.updateItems(itemRef.itemID, true);
            playerMoney.removingFunds(itemRef.price);
            accepted();
            congratsScreen.SetActive(true);
            congrats = true;
        }
        else if(itemRef.price > playerMoney.money)
        {
            refusedScreen.SetActive(true);
            refuse = true;
            refused();
        }
    }

    public void refused()
    {
        refusedScreen.SetActive(true);
    }

    public void accepted()
    {
        congratsScreen.SetActive(true);
        congratsText.text = "Congrats! You bought the " + itemRef.itemName;
    }

    public void returnFromAccept()
    {
        buyingScreen.SetActive(true);
        congratsScreen.SetActive(false);
        buying = true;
        congrats = false;
    }

    public void returnFromRefuse()
    {
        buyingScreen.SetActive(true);
        refusedScreen.SetActive(false);
        buying = true;
        refuse = false;
    }

    public void readVendor(RaycastHit hit)
    {
        vendorRef = hit;
    }

    public void resetAllBools()
    {
        transaction = false;
        transactionScreen = false;
        buying = false;
        selling = false;
        confirm = false;
        congrats = false;
        refuse = false;
        clicked = false;
        close = false;
    }
}