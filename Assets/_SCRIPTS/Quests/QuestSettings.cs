using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class QuestSettings : MonoBehaviour {

    public Color questColor;

    public bool questCompleted = false;
    private float childrenIndex;

    public string questTitle = "Please give me a title.";

    protected Time completionTime;

    [SerializeField]
    GameObject QuestScreen;

    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    QuestHandler questHandler;

    PlayerController pc;
    CamMouseLook cml;

    public float startTimeQuest;
    public float endTimeQuest;


    [TextArea]
    public string questText;

    // Use this for initialization
    void Awake () {
        if (gameManager == null)
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (questHandler == null)
            questHandler = GameObject.Find("GameManager").GetComponent<QuestHandler>();
        if (QuestScreen == null)
            QuestScreen = GameObject.Find("QuestCompletedScreen");
        childrenIndex = transform.childCount;
        //sets the color for each quad of the children
        for (int i = 0; i < childrenIndex; i++)
        {
            if (transform.GetChild(i).childCount != 0)
            {
                Transform child = transform.GetChild(i).GetChild(0);
                if (child != null)
                {
                    if (child.name == "Quad")
                    {
                        Renderer rend = child.GetComponent<Renderer>();
                        rend.material.color = questColor;
                        rend.material.SetColor("_EmissionColor", questColor);
                    }
                }
            }
        }
        


        pc = gameManager.character.GetComponent<PlayerController>();
        cml = gameManager.character.GetComponentInChildren<CamMouseLook>();

    }
	
	// Update is called once per frame
	void Update () {

        if (questCompleted)
        {
            initializeQuestScreen();
            questHandler.switchQuest(questTitle);
            questCompleted = false;
        }
	}

    public void updateQuestHandler(string nGivername, string nDirections)
    {
        questHandler.addQuest(questTitle, questColor, nGivername, questText, nDirections);
    }

    void initializeQuestScreen()
    {
        float totalQuestTime = endTimeQuest - startTimeQuest;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cml.enabled = false;
        pc.enabled = false;
        Time.timeScale = 0f;
        QuestScreen.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = this.questTitle;
        QuestScreen.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = ("Time: " + totalQuestTime.ToString("n1"));
        QuestScreen.transform.GetChild(0).gameObject.SetActive(true);
    }

    
}