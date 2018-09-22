using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupDrop : MonoBehaviour
{
    //setting max distance player can pick up items from
    public float itemRange;
    public Transform player;
    //used to reference which item if any should be activated on the camera
    public bool holdingItem = false;
    //distance dropped objects drop from player
    public float spawnDistance;
    public GameObject daInventoryMan;
    // Use this for initialization
    public Rigidbody itemInHand;
    //Font to be used
    public Font font;
    private GUIStyle style = new GUIStyle();
    bool showMessage = false;
    private GameObject playerCamera;
    private GameManager gm;
    private GameObject character;

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerCamera = GameObject.Find("Character/FPPCamera").gameObject;
        int h = Screen.height;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;
        style.font = font;
        character = GameObject.Find("Character").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //player input to try and pick up an item
        if (Input.GetButtonDown("Interact"))
        {

            if (holdingItem)
            {
                dropItem();

            }
            else if (!holdingItem)
            {
                pickupItem();
            }
        }

        if(playerCamera.transform.childCount == 0)
        {
            holdingItem = false;
            itemInHand = null;
            daInventoryMan.GetComponent<Inventory>().setItemHolding(-1);
        }
    }

    private void OnGUI()
    {
        if (holdingItem && !daInventoryMan.GetComponent<Inventory>().phoneOut)
        {
            if (gm.ControllerCheck())
            {
                GUI.Label(new Rect((Screen.width - 100), Screen.height - 20, 1, 20), "Press X to Use", style);
                Rect dropPromt = new Rect((Screen.width - 120), Screen.height - 70, 40, 40);
                GUI.DrawTexture(dropPromt, Resources.Load<Texture2D>("KeyPrompts/" + "X"));

                GUI.Label(new Rect((Screen.width - 250), Screen.height - 20, 1, 20), "Press Y to Store", style);
                Rect storePromt = new Rect((Screen.width - 270), Screen.height - 70, 40, 40);
                GUI.DrawTexture(storePromt, Resources.Load<Texture2D>("KeyPrompts/" + "Y"));

                GUI.Label(new Rect((Screen.width - 440), Screen.height - 20, 1, 20), "Press Right Stick to Drop", style);
                Rect usePromt = new Rect((Screen.width - 460), Screen.height - 70, 40, 40);
                GUI.DrawTexture(usePromt, Resources.Load<Texture2D>("KeyPrompts/" + "R_C"));
            }
            else
            {
                GUI.Label(new Rect((Screen.width - 100), Screen.height - 20, 1, 20), "Press R to Use", style);
                Rect dropPromt = new Rect((Screen.width - 120), Screen.height - 70, 40, 40);
                GUI.DrawTexture(dropPromt, Resources.Load<Texture2D>("KeyPrompts/" + "R"));

                GUI.Label(new Rect((Screen.width - 250), Screen.height - 20, 1, 20), "Press Q to Store", style);
                Rect storePromt = new Rect((Screen.width - 270), Screen.height - 70, 40, 40);
                GUI.DrawTexture(storePromt, Resources.Load<Texture2D>("KeyPrompts/" + "Q"));

                GUI.Label(new Rect((Screen.width - 400), Screen.height - 20, 1, 20), "Press E to Drop", style);
                Rect usePromt = new Rect((Screen.width - 420), Screen.height - 70, 40, 40);
                GUI.DrawTexture(usePromt, Resources.Load<Texture2D>("KeyPrompts/" + "E"));
            }
            
        }

    }
    void dropItem()
    {
        itemInHand.isKinematic = false;
        itemInHand.detectCollisions = true;
        itemInHand.useGravity = true;
        itemInHand.constraints = RigidbodyConstraints.None;
        itemInHand.transform.parent = null;
        itemInHand = null;
        holdingItem = false;
        daInventoryMan.GetComponent<Inventory>().setItemHolding(-1);
    }
    void pickupItem()
    {
        RaycastHit hit;

        Ray ray = playerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, itemRange) && hit.transform.tag == "item")    //checking that item trying to be picked up is tagged to be held
        {
            //checking if phone was selected
            if (hit.transform.GetComponent<ItemID>().itemID == 3)
            {
                daInventoryMan.GetComponent<Inventory>().CollectedCollectable(hit.transform.GetComponent<ItemID>().itemID);
                hit.rigidbody.GetComponent<ItemHeldBool>().beingHeld = true;
                StartCoroutine(turnOffPhone(hit));
            }
            //checking if money has been selected
            else if(hit.transform.GetComponent<ItemID>().itemID == 5)
            {
                character.GetComponent<funds>().addingFunds(hit.transform.GetComponent<value>().worth);
                Destroy(hit.transform.gameObject);
            }
            else
            {
                //setting object as a child and giving new position
                hit.transform.SetParent(player);
                //changing the items position so that it is in a set position when picked up
                hit.transform.position = hit.transform.parent.position + Camera.main.transform.right * 0.8f + Camera.main.transform.forward - Camera.main.transform.up * 0.08f;
                //getting the rotation of the player to base item rotation off of
                Quaternion playerRotation = player.transform.rotation;
                //adjusting the rotation of the item to a prefered alignment
                hit.transform.rotation = playerRotation;
                hit.transform.Rotate(Vector3.right, -90);
                hit.transform.Rotate(Vector3.forward, 180);
                daInventoryMan.GetComponent<Inventory>().setItemHolding(hit.transform.GetComponent<ItemID>().itemID);
                holdingItem = true;
                //setting the objects rigid body and turning off collisions
                itemInHand = hit.transform.GetComponent<Rigidbody>();
                itemInHand.isKinematic = true;
                itemInHand.detectCollisions = false;
                itemInHand.useGravity = false;
                itemInHand.constraints = RigidbodyConstraints.FreezeAll;

                if (itemInHand.GetComponent<ItemHeldBool>() != null)
                    itemInHand.GetComponent<ItemHeldBool>().beingHeld = true;
            }

        }
    }

    IEnumerator turnOffPhone(RaycastHit hit)
    {
        Renderer[] children = hit.rigidbody.gameObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in children)
        {
            r.enabled = false;
        }
        yield return new WaitForSeconds(2f);
        Destroy(hit.rigidbody.gameObject);
    }
}