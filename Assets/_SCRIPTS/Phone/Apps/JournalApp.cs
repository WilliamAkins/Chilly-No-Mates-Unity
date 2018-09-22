using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalApp : MonoBehaviour {

    private GameObject phone;
    public Material backgroundList;
    public Material backgroundFull;
    public Material homescreen;

    int questSelection = 1;
    int oldSelection = 2;
    int pageNumber = 1;
    int questsOnPage = 0;
    int lineWidth = 23;
    int pages = 1;

    bool listBackgroundUpdate = true;
    bool fullBackgroundUpdate = true;
    bool bigScreen = false;

    Transform listPage;
    Transform fullPage;
    Transform completedPage;

    QuestHandler handler;

    private bool left = false;
    private bool right = false;
    private bool up = false;
    private bool down = false;

    // Use this for initialization
    void Start()
    {
        phone = GameObject.FindGameObjectWithTag("Phone").transform.GetChild(0).gameObject;
        listPage = transform.Find("List Screen");
        fullPage = transform.Find("Full Screen Quest");
        handler = GameObject.Find("GameManager").GetComponent<QuestHandler>();
        Clear();
    }

    // Update is called once per frame
    void Update()
    {
        //Updates the background if it is still the home screen.
        if (listBackgroundUpdate == true)
        {
            phone.transform.GetChild(0).GetComponent<MeshRenderer>().material = backgroundList;
            listBackgroundUpdate = false;
        }

        if ((Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Joystick1Button1)) && !bigScreen)
        {
            close();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            bigScreen = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.LeftArrow) || DpadLeft())
        {
            Clear();
            pageNumber--;
        }

        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.RightArrow) || DpadRight())
        {
            Clear();
            pageNumber++;
        }


        if (pageNumber < 1)
            pageNumber = pages;
        if (pageNumber > pages)
            pageNumber = 1;



        if (bigScreen && questSelection != 0)
            OpenFull(questSelection);

        if (!bigScreen)
        {
            LoadQuests(pageNumber);
            MakeSelection();
        }
    }

    void LoadQuests(int pageNumber)
    {
        listPage.Find("Page Number").GetComponent<TextMesh>().text = pageNumber.ToString();

        List<Quest> quests = handler.getActiveQuestList();

        //Gets the number of quests stored in the quest list
        int numberOfQuests = quests.Count;

        if (numberOfQuests != 0)
        {
            if (questSelection == 0)
                questSelection = 1;

            //How many pages of quests are there (+1 as <4 quests needs 1 page not 0)
            pages = (numberOfQuests / 4) + 1;

            //Which 4 quests should be loaded onto the current page (Will be multiples of 4 starting with 0)
            int questCount = (pageNumber - 1) * 4;

            //Updates questsOnPage for selection purposes
            questsOnPage = numberOfQuests - questCount;

            //Cycles through Quest (1-4) game objects on phone screen 
            for (int i = 1; i < 5; i++)
            {
                //Gets the current game object
                Transform current = listPage.Find("Quest (" + i.ToString() + ")");

                //Reformats colour so that textmesh will display it
                Color colour = quests[questCount].colour;
                Color colourOutput = new Color(colour.r, colour.g, colour.b);

                //Sets heading text and colour
                current.Find("Heading").GetComponent<TextMesh>().text = quests[questCount].title;
                current.Find("Heading").GetComponent<TextMesh>().color = colourOutput;

                //Sets quest giver text
                current.Find("Setter").GetComponent<TextMesh>().text = quests[questCount].giverName;

                //Ensures text doesn't go off the edge of the screen
                string instruction = quests[questCount].directions;
                if (instruction.Length > lineWidth)
                    instruction = instruction.Substring(0, lineWidth) + "...";

                current.Find("Current Instruction").GetComponent<TextMesh>().text = instruction;

                //If there are still quests to be displayed
                if (questCount < numberOfQuests)
                    questCount++;

                //If all the quests have been shown
                if (questCount == numberOfQuests)
                    break;
            }
        }
        else
        {
            questSelection = 0;
            Clear();
        }
        
    }

    void MakeSelection()
    {
        if (questsOnPage > 1)
        {
            if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.DownArrow) || DpadDown())
            {
                oldSelection = questSelection;
                questSelection++;
            }

            if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.UpArrow) || DpadUp())
            {
                oldSelection = questSelection;
                questSelection--;
            }

            if (questSelection < 1)
                questSelection = questsOnPage;
            if (questSelection > questsOnPage)
                questSelection = 1;
        }

        transform.Find("List Screen").Find("Quest (" + questSelection.ToString() + ")").Find("Heading").GetComponent<TextMesh>().fontSize = 150;
        transform.Find("List Screen").Find("Quest (" + oldSelection.ToString() + ")").Find("Heading").GetComponent<TextMesh>().fontSize = 120;
    }

    void OpenFull(int selection)
    {
        if(fullBackgroundUpdate)
        {
            phone.transform.GetChild(0).GetComponent<MeshRenderer>().material = backgroundFull;
            listPage.gameObject.SetActive(false);
            fullPage.gameObject.SetActive(true);

            List<Quest> quests = handler.getActiveQuestList();

            int questSelection = ((pageNumber - 1)*4) + selection;
            questSelection--;

            //Reformats colour so that textmesh will display it
            Color colour = quests[questSelection].colour;
            Color colourOutput = new Color(colour.r, colour.g, colour.b);

            //Sets heading text and colour
            fullPage.Find("Heading").GetComponent<TextMesh>().text = quests[questSelection].title;
            fullPage.Find("Heading").GetComponent<TextMesh>().color = colourOutput;

            //Sets quest giver text
            fullPage.Find("Setter").GetComponent<TextMesh>().text = quests[questSelection].giverName;

            //Ensures text doesn't go off the edge of the screen
            string instruction = quests[questSelection].directions;
            string output = instruction;

            if (instruction.Length > lineWidth)
                output = instruction.Insert(lineWidth, "-\n");

            fullPage.Find("Current Instruction").GetComponent<TextMesh>().text = output;

            string description = quests[questSelection].text;

            int descriptionLength = description.Length;
            int numOfLines = descriptionLength / lineWidth;
            int count = lineWidth;

            for (int i = 0; i < numOfLines; i++)
            {
                description = description.Insert(count, "-\n");
                count = count + lineWidth + 3;
            }

            description.TrimEnd('-');

            fullPage.Find("Description").GetComponent<TextMesh>().text = description;

        }




        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            phone.transform.GetChild(0).GetComponent<MeshRenderer>().material = backgroundList;
            fullBackgroundUpdate = true;
            listBackgroundUpdate = true;
            bigScreen = false;
            listPage.gameObject.SetActive(true);
            fullPage.gameObject.SetActive(false);
        }
    }

    void Clear()
    {
        for (int i = 1; i < 5; i++)
        {
            Transform current = listPage.Find("Quest (" + i.ToString() + ")");

            current.Find("Heading").GetComponent<TextMesh>().text = "";
            current.Find("Setter").GetComponent<TextMesh>().text = "";
            current.Find("Current Instruction").GetComponent<TextMesh>().text = "";
        }
    }

    void close()
    {
        //Resets phone to home screen to close the app
        phone.transform.GetChild(0).GetComponent<MeshRenderer>().material = homescreen;
        phone.GetComponent<MobilePhone>().appClosed = true;
        listBackgroundUpdate = true;
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
