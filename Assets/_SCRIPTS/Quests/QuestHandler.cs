using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHandler : MonoBehaviour {


    List<Quest> activeQuestList = new List<Quest>();
    List<Quest> inactiveQuestList = new List<Quest>();
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void addQuest(string title, Color colour, string giverName, string text, string directions)
    {
        bool alreadyExists = false;
        Quest questToAdd = new Quest(title, colour, giverName, text, directions);
        if(activeQuestList.Count != 0)
        {
            foreach (Quest q in activeQuestList)
            {
                if(q.title == questToAdd.title)
                {
                    alreadyExists = true;
                    q.giverName = questToAdd.giverName;
                    q.directions = questToAdd.directions;
                }
            }
            if (!alreadyExists)
            {
                activeQuestList.Add(questToAdd);
            }
        }
        else
        {
            activeQuestList.Add(questToAdd);
        }
    }

    public void switchQuest(string title)
    {
        int index = 0;
        if(activeQuestList.Count != 0)
        {
            for (int i = 0; i < activeQuestList.Count; i++)
            {
                if (activeQuestList[i].title == title)
                {
                    index = i;
                    

                }
            }
            inactiveQuestList.Add(activeQuestList[index]);
            activeQuestList.Remove(activeQuestList[index]);
        }
    }

    public List<Quest> getActiveQuestList()
    {
        return activeQuestList;
    }
    public List<Quest> getInactiveQuestList()
    {
        return inactiveQuestList;
    }
}
