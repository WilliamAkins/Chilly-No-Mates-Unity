using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(DialogueSystem))]
public class DialogueSystemHandle : Editor
{
    protected virtual void OnSceneGUI()
    {
        DialogueSystem npc = (DialogueSystem)target;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = Color.white;
        style.fontSize = 13;
        style.alignment = TextAnchor.MiddleCenter;

        Handles.Label(npc.transform.position + new Vector3(0, 1.4f, 0), "ID: " + npc.npcID, style);

        Handles.color = Color.magenta;
        Handles.DrawWireDisc(npc.transform.position, Vector3.up, npc.npcRange); 
        Handles.Label(npc.transform.position + new Vector3(npc.npcRange, 0, 0), "NPC Entry Range", style);

        Handles.color = Color.cyan;
        Handles.DrawWireDisc(npc.transform.position, Vector3.up, npc.npcRange * 2);
        Handles.Label(npc.transform.position + new Vector3(npc.npcRange * 2, 0, 0), "NPC Exit Range", style);
    }
}
#endif

public class DialogueSystem : MonoBehaviour
{
    //stores values to reference against the dialogue database
    public int npcID = 2;

    //says whether the npc has a quest to give
    public bool questGiver = false;

    //stores the range at which the npc can interact with the player
    public float npcRange = 7.0f;

    private GameManager gameManager;

    private static bool GUIShowing = false;
    private byte optionSelected = 1;
    private byte maxOption = 0;
    private Transform player;

    //stores the various dialogue text that will be displayed
    private string[] dbDialogue = new string[12];

    //stores the values that states whether the player has already said the responses to the current dialogue text
    private char[] playerHasSaid = new char[3];

    //Stores the next dialogue that the NPC will display after the initial one | default of 1 which is the initial dialogue
    private int nextDialogue = 1;

    private GameObject dialoguePopup;
    private GameObject dialogueOverlay;
    private GameObject namePopup;
    private GameObject marker;
   
    //stores a reference to the question and response text lines for the dialogue overlay
    private GameObject question;
    private GameObject[] option = new GameObject[4];

    [SerializeField]
    private GameObject[] questsToActivate;

    public bool guiIsShowing()
    {
        return GUIShowing;
    }

    // Use this for initialization
    void Start()
    {
        print("Dialogue system debugging commands are currently enabled. Disable them before release. (Ctrl + X to clear DB)");

        //get the game manager script
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //Stores a reference to the actual player transformation
        player = GameObject.Find("Character").transform;

        //show the npc name above their head
        showName();

        //if the player is a quest giver then display the quest icon
        if (questGiver)
            displayQuestIcon();
    }

    // Update is called once per frame
    void Update()
    {
        //Updates the npc name to that it always faces the player
        namePopup.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, player.position - transform.position, 10.0f, 0.0f));
        namePopup.transform.rotation = Quaternion.Euler(0.0f, namePopup.transform.eulerAngles.y + 180, namePopup.transform.eulerAngles.z);

        namePopup.transform.position = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);

        if (Input.GetButtonDown("Interact") && !GUIShowing)
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //checks that the player is near an npc when trying to interact
            if (Physics.Raycast(ray, out hit, npcRange) && hit.transform.tag == "npc")
            {
                print("Player is talking to the NPC...");

                //set the nap popup to not active, so it's hidden while player is talking to them
                namePopup.SetActive(false);

                //establish a connection to the database and find the correct data
                dbConnect();

                //now create a GUI to display the data
                spawnGUI();
            }
        }

        //Check if the popup that shows above NPC heads exists
        if (dialoguePopup)
        {
            //Updates the popup position so it always faces the player
            dialoguePopup.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, player.position - transform.position, 10.0f, 0.0f));
            dialoguePopup.transform.rotation = Quaternion.Euler(0.0f, dialoguePopup.transform.eulerAngles.y + 180, dialoguePopup.transform.eulerAngles.z);

            //checks whether the player has pressed any key to change their option selected and then will update the GUI
            checkSelectedOption();

            //gets the distance between the player and the npc each frame
            float distToPlayer = Vector3.Distance(player.position, transform.position);

            //if the players moves too far away from the npc, remove the dialogue GUI and set the dialogueID to the character phase
            if (distToPlayer > npcRange && GUIShowing)
            {
                destroyGUI();

                //sets the dialogueID to character phase
                nextDialogue = Int32.Parse(dbDialogue[11]);
            }

            //check if the player clicked on the mouse which means they confirmed their dialogue option
            //Configured to use the left mouse btn and the return key
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
            {
                updateDBPlayerSaid();

                //sets the next value to the corresponding next dialogue ID from the database
                nextDialogue = Int32.Parse(dbDialogue[optionSelected + 3]);

                //if the player chooses to end the conversation
                if (nextDialogue == 0) {
                    destroyGUI();

                    //sets the dialogueID to character phase
                    nextDialogue = Int32.Parse(dbDialogue[11]);
                }

                //check if an outcome type was met
                selectOutcomeType();

                //Establishes a new connection to the database with a different dialogueID
                dbConnect();

                //changes the text in the dialogue overlay GUI to the new text from the database
                changeGUIText();

                //resets the currently selected response option to 1
                clearPreviousOption(optionSelected);
                optionSelected = 1;
            }

            //This command is used to run commands like reset the database, it should be removed on release
            debugCommands();
        }
    }

    void spawnGUI()
    {
        GUIShowing = true;

        //Spawns an instance of the dialogue popup above the NPC head and set its text
        dialoguePopup = Instantiate(Resources.Load("DialogueSystem/DialoguePopup"), new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity) as GameObject;

        //spawns an instance of the large GUI popup that allows the player to choose responses
        dialogueOverlay = Instantiate(Resources.Load("DialogueSystem/DialogueOverlay"), Vector3.zero, Quaternion.identity) as GameObject;

        //Scale the dialogue panel to the screen resolution
        Transform overlay = dialogueOverlay.transform.Find("DialoguePanel");
        overlay.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 200);
        overlay.position = new Vector2(Screen.width * 0.5f, 0);

        //Gets a reference to the dialogue panel question
        question = dialogueOverlay.transform.Find("DialoguePanel/Question").gameObject;

        //Gets a reference to the dialogue panel response options
        for (int i = 1; i <= 3; i++)
            option[i] = dialogueOverlay.transform.Find("DialoguePanel/Option" + i).gameObject;

        //update the text in each of the text boxes in the dialogue panel
        changeGUIText();

        //Calls this at the start to highlight a default option
        changeSelectedOption();
    }

    void destroyGUI()
    {
        GUIShowing = false;

        //destory both the popup above the NPC head and the dialogue GUI
        Destroy(dialoguePopup);
        Destroy(dialogueOverlay);

        //if playe is no longer talking to the npc then show the npc name
        namePopup.SetActive(true);
    }

    void changeGUIText()
    {
        //sets the dialogue text for both the popup above the npc head and the dialogue overlay panel
        dialoguePopup.GetComponent<TextMesh>().text = dbDialogue[0];
        question.GetComponent<Text>().text = dbDialogue[10] + ": " + dbDialogue[0];

        for (int i = 1; i <= 3; i++)
        {
            option[i].GetComponent<Text>().text = dbDialogue[i];

            //sets the text colour to white or grey depending on whether the player has said the text before or not
            if (playerHasSaid[i - 1] == '1')
            {
                option[i].GetComponent<Text>().color = Color.gray;
            }
            else
            {
                option[i].GetComponent<Text>().color = Color.white;
            }
        }
    }

    void checkSelectedOption()
    {
        //checks the inputs to see whether the player has pressed a button to change the currently selected option
        if (Input.GetButtonDown("Dialogue 1"))
        {
            if (maxOption >= 1)
            {
                clearPreviousOption(optionSelected);
                optionSelected = 1;
            }
        } 
        else if (Input.GetButtonDown("Dialogue 2"))
        {
            if (maxOption >= 2)
            {
                clearPreviousOption(optionSelected);
                optionSelected = 2;
            }
        } 
        else if (Input.GetButtonDown("Dialogue 3")) 
        {
            if (maxOption >= 3)
            {
                clearPreviousOption(optionSelected);
                optionSelected = 3;
            }
        }

        //Handles the scroll wheel with scrolling up and down
        var scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollValue > 0f)
        {
            // scroll up
            clearPreviousOption(optionSelected);
            if (optionSelected > 1)
                optionSelected--;
            else
                optionSelected = maxOption;
        } 
        else if (scrollValue < 0f)
        {
            // scroll down
            clearPreviousOption(optionSelected);
            if (optionSelected < maxOption)
                optionSelected++;
            else
                optionSelected = 1;
        }

        //Selects the new dialogue option
        changeSelectedOption();
    }

    void clearPreviousOption(byte previousOption)  
    {
        //disables the highlighting for the previously selected option
        GameObject OptionSelected = dialogueOverlay.transform.Find("DialoguePanel/Selected" + previousOption).gameObject;
        OptionSelected.GetComponent<Image>().enabled = false;
    }

    void changeSelectedOption()
    {
        //Finds and then highlists the selected response option that is selected
        GameObject OptionSelected = dialogueOverlay.transform.Find("DialoguePanel/Selected" + optionSelected).gameObject;
        OptionSelected.GetComponent<Image>().enabled = true;
    }

    void dbConnect()
    {
        //Path to database
        string path = Application.dataPath + "/StreamingAssets";
        string conn = "URI=file:" + path + "/dialogueDB.db";

        //string conn = "URI=file:" + Application.dataPath + "/Resources/DialogueSystem/dialogueDB.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection) new SqliteConnection(conn);

        //Open connection to the database
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT D.Dialogue, D.ResponseTop, D.ResponseMiddle, D.ResponseBottom, D.ResponseTopGoTo, D.ResponseMiddleGoTo, D.ResponseBottomGoTo, D.PlayerHasSaid, D.OutcomeType, D.OutcomeVal, C.Name, C.Phase " +
                          "FROM Dialogue AS D " +
                          "INNER JOIN Character AS C " +
                          "ON D.CharacterID = C.CharacterID " +
                          "WHERE D.CharacterID = " + npcID + " AND D.DialogueID = " + nextDialogue;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            dbDialogue[0] = reader.GetString(0); //Dialogue

            //check that the responses are valid before adding them to the string array, also checks how many responses will be displayed and adjusts the dialogue panel depending on that
            maxOption = 0;
            for (int i = 1; i <= 3; i++)
            {
                if (reader[i] != DBNull.Value)
                {
                    dbDialogue[i] = i + ") " + reader.GetString(i); //Responses
                    maxOption++;
                }
                else
                {
                    dbDialogue[i] = "";
                }

            }

            dbDialogue[4] = reader.GetInt32(4).ToString(); //ResponseTopGoTo
            dbDialogue[5] = reader.GetInt32(5).ToString(); //ResponseMiddleGoTo
            dbDialogue[6] = reader.GetInt32(6).ToString(); //ResponseBottomGoTo
            dbDialogue[7] = reader.GetString(7); //PlayerHasSaid
            dbDialogue[8] = reader.GetInt32(8).ToString(); //OutcomeType
            dbDialogue[9] = reader.GetInt32(9).ToString(); //OutcomeVal
            dbDialogue[10] = reader.GetString(10); //NPC Name
            dbDialogue[11] = reader.GetInt32(11).ToString(); //NPC Phase
        }

        //Loop through each character in the PlayerHasSaid string, looking for the values and inserting them into a char array
        for (int i = 0; i < dbDialogue[7].Length; i++) {
            if (dbDialogue[7][i] != ',')
                playerHasSaid[i / 2] = dbDialogue[7][i];
        }

        //close the connection to the database once done
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    void updateDBPlayerSaid()
    {
        //updates the PlayerHasSaid array which will be passed into the database
        playerHasSaid[optionSelected - 1] = '1';

        //concatinate a new string to hold the playerHasSaid data
        string updatedPlayerHasSaid = "\"" + playerHasSaid[0] + "," + playerHasSaid[1] + "," + playerHasSaid[2] + "\"";

        //Path to database
        string path = Application.dataPath + "/StreamingAssets";
        string conn = "URI=file:" + path + "/dialogueDB.db";

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);

        //Open connection to the database
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlInsert = "UPDATE Dialogue " +
                           "SET PlayerHasSaid = " + updatedPlayerHasSaid +
                           " WHERE CharacterID = " + npcID + " AND DialogueID = " + nextDialogue;

        dbcmd.CommandText = sqlInsert;
        IDataReader reader = dbcmd.ExecuteReader();

        //close the connection to the database once done
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    void resetDB()
    {
        //Path to database
        string path = Application.dataPath + "/StreamingAssets";
        string conn = "URI=file:" + path + "/dialogueDB.db";

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);

        //Open connection to the database
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlInsert = "UPDATE Dialogue " +
                           "SET PlayerHasSaid = " + "\"0,0,0\"";

        dbcmd.CommandText = sqlInsert;
        IDataReader reader = dbcmd.ExecuteReader();

        //close the connection to the database once done
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    private void showName()
    {

        string npcName = "placeholder name";

        //Path to database
        string path = Application.dataPath + "/StreamingAssets";
        string conn = "URI=file:" + path + "/dialogueDB.db";

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);

        //Open connection to the database
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT C.Name " +
                          "FROM Character AS C " +
                          "WHERE C.CharacterID = " + npcID;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            npcName = reader.GetString(0); //Dialogue
        }

        //close the connection to the database once done
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        namePopup = Instantiate(Resources.Load("DialogueSystem/NamePopup"), new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity) as GameObject;
        namePopup.GetComponent<TextMesh>().text = npcName;
    }

    private void displayQuestIcon() {
        marker = Instantiate(Resources.Load("Marker"), new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z), Quaternion.identity) as GameObject;
    }

    private void selectOutcomeType()
    {
        switch (Int32.Parse(dbDialogue[8]))
        {
            case 0:
                dialogueStartQuest();
                break;
            case 1:
                dialogueEndQuest();
                break;
            case 2:
                dialogueGiveItem();
                break;
            case 3:
                dialogueRecieveItem();
                break;
            case 4:
                dialogueVendor();
                break;
            default:
                break;
        }
    }

    private void dialogueStartQuest()
    {
        if (questsToActivate.Length != 0)
        {
            foreach (GameObject g in questsToActivate)
            {
                if (!g.activeSelf)
                {
                    //OutcomeType = 0

                    g.SetActive(true);

                    Destroy(marker);
                }
            }
        }
    }

    private void dialogueEndQuest()
    {
        //OutcomeType = 1
        if (GetComponent<ItemHeldBool>() != null) GetComponent<ItemHeldBool>().beingHeld = true;
        print("ending a quest with an NPC!");
    }

    private void dialogueGiveItem()
    {
        //OutcomeType = 2
        print("give the player an item from an NPC!");
    }

    private void dialogueRecieveItem()
    {
        //OutcomeType = 3
        print("give the NPC an item from the player!");
    }

    private void dialogueVendor()
    {
        //OutcomeType = 4
        print("starting the vendor with an NPC!");

        gameManager.startNPCVendor(true);
    }

    private void debugCommands()
    {
        //This function should be displayed or removed on release, it's only for testing...
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.X))

            {
                print("RESETTING THE DATABASE | This is a debug command, please delete on release.");
                resetDB();
            }
        }
            
    }
}