using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseQuest : MonoBehaviour {
    public bool isFirst;

    public string questGiverName;

    public string questDirections;

    public GameObject[] nextQuestPoint;
    // Use this for initialization

    private float startTime;

    private float endTime;

    private bool hasBeenActivated = false;

    [SerializeField]
    private bool has3DText = true;
    
    public void setup()
    {
        startTime = Time.time;
        if (isFirst)
        {
            gameObject.SetActive(true);
            this.transform.parent.GetComponent<QuestSettings>().startTimeQuest = startTime;
            activateBool();
            updateQuest();
        }
        else
        {
            gameObject.SetActive(false);
            
        }
        

        this.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMesh>().text = questDirections;

        //stops the text mesh and coloured quad from showing if bool is false
        if (!has3DText)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void endQuest()
    {
        endTime = Time.time;
        this.transform.parent.GetComponent<QuestSettings>().endTimeQuest = endTime;
        this.transform.parent.GetComponent<QuestSettings>().questCompleted = true;
        if (nextQuestPoint.Length != 0)
        {
            foreach (GameObject g in nextQuestPoint)
            {
                if (!g.activeSelf)
                {
                    g.SetActive(true);
                }
            }
        }
    }

    public void continueQuest()
    {
        endTime = Time.time;
        if(nextQuestPoint.Length != 0)
        {
            foreach (GameObject g in nextQuestPoint)
            {
                if (!g.activeSelf)
                {
                    g.SetActive(true);
                }
            }
        }
        
    }

    public float getStartTime()
    {
        return startTime;
    }

    public float getEndTime()
    {
        return endTime;
    }

    public bool getActivateBool()
    {
        return hasBeenActivated;
    }
    public void activateBool()
    {
        hasBeenActivated = true;
    }

    public void updateQuest()
    {
        this.transform.parent.GetComponent<QuestSettings>().updateQuestHandler(questGiverName, questDirections);
    }
}