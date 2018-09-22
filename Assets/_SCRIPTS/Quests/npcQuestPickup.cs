using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcQuestPickup : BaseQuest
{

    public GameObject npcToTalkTo;

    private ItemHeldBool hasBeenSpokenTo;
    // Use this for initialization
    void Start ()
    {
        hasBeenSpokenTo = npcToTalkTo.GetComponent<ItemHeldBool>();

        if (hasBeenSpokenTo == null)
        {
            hasBeenSpokenTo = npcToTalkTo.AddComponent<ItemHeldBool>();
        }
        setup();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (gameObject.activeSelf)
        {
            if (!getActivateBool())
            {
                activateBool();
                updateQuest();
                
            }
            if (hasBeenSpokenTo.beingHeld)
            {
                if (transform.parent.GetChild(transform.parent.childCount - 1) != this.transform)
                {
                    continueQuest();
                }
                else
                {
                    endQuest();
                }
                this.gameObject.SetActive(false);
            }
        }
    }
}
