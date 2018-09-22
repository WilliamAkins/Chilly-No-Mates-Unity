using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerGUI : MonoBehaviour
{
    public byte crosshairSizeInPx;
    public Color crosshairColour;

    //I want to load this privately, probably should move the font folder to resources so we can load them on start
    public Font font;

    private GameObject playerCamera;
    private float deltaTime = 0.0f;
    private Texture2D crosshair;

    private GameManager gm;
    private DialogueSystem dialogueSystem;

    // Use this for initialization
    void Start()
    {
        //Finds the FPP camera in a fairly efficient way, privately that is...
        playerCamera = GameObject.Find("Character/FPPCamera").gameObject;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        //Load the crosshair texture
        crosshair = Resources.Load("Textures/crosshair", typeof(Texture2D)) as Texture2D;
        GetComponent<Renderer>().material.mainTexture = crosshair;

        //Loop through each pixel in the crosshair and set its color
        for (int y = 0; y < crosshair.height; y++)
            for (int x = 0; x < crosshair.width; x++)
                crosshair.SetPixel(x, y, new Color(crosshairColour.r, crosshairColour.g, crosshairColour.b, crosshair.GetPixel(x, y).a));
        crosshair.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        int halfW = w / 2, halfH = h / 2;

        //draws the crosshair on the screen
        GUI.DrawTexture(new Rect(halfW, halfH, crosshairSizeInPx, crosshairSizeInPx), crosshair, ScaleMode.StretchToFill, true, 0.0f);

        //calculate the fps and frame time then output it to the screen
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;
        style.font = font;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);

        //check if the player has their cursor hoving over an item and display a UI popup if they do
        Ray checkForItem = playerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit found;
        if (Physics.Raycast(checkForItem, out found, playerCamera.GetComponent<PickupDrop>().itemRange) && found.transform.tag == "item")
        {
            if (playerCamera.GetComponent<PickupDrop>().holdingItem == false)
            {
                
                if (gm.ControllerCheck())
                {
                    GUI.Label(new Rect(halfW, halfH + 30, 1, 20), "Press X to Interact", style);
                    Rect keyPromt = new Rect(halfW - 20, halfH + 50, 40, 40);
                    GUI.DrawTexture(keyPromt, Resources.Load<Texture2D>("KeyPrompts/" + "X"));
                }
                else
                {
                    GUI.Label(new Rect(halfW, halfH + 30, 1, 20), "Press E to Interact", style);
                    Rect keyPromt = new Rect(halfW - 20, halfH + 50, 40, 40);
                    GUI.DrawTexture(keyPromt, Resources.Load<Texture2D>("KeyPrompts/" + "E"));
                }

                
            }
        }

        //check if the player has their cursor hoving over an npc they can interact with
        Ray checkForNPC = playerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit npcFound;
        if (Physics.Raycast(checkForNPC, out npcFound, 3.0f) && npcFound.transform.tag == "npc")
        {
            dialogueSystem = npcFound.transform.GetComponent<DialogueSystem>();

            if (!dialogueSystem.guiIsShowing()) {
                GUI.Label(new Rect(halfW, halfH + 30, 1, 20), "Press E to talk", style);
                Rect keyPromt = new Rect(halfW - 20, halfH + 50, 40, 40);
                GUI.DrawTexture(keyPromt, Resources.Load<Texture2D>("KeyPrompts/" + "E"));
            }
        }
    }
}