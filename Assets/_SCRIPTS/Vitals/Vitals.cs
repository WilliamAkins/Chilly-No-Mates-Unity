using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vitals : MonoBehaviour
{
    [Range(1, 100)]
    public int health = 100;
    [Range(0, 100)]
    public float soberness = 100.0f;
    [Header("50% energy is standard.")]
    [Header("More than that is hyper & less is tired.")]
    [Range(0, 100)]
    public float energy = 50.0f;
    [Range(0, 100)]
    public float bladder = 100.0f; //100 = not needing a pee, 0 = player will pee

    private bool playerIsKnockedOut = false;
    private bool playerIsDrunk = false;
    private bool playerEnergyNotNeutral = false;
    private bool playerIsPeeing = false;

    //set to true if the player decides they wish to pee not automatically
    private bool forcedPee = false;

    public void setKnockedOutState(bool KnockedOutArg)
    {
        playerIsKnockedOut = KnockedOutArg;
    }

    public bool getKnockedOutState()
    {
        return playerIsKnockedOut;
    }

    public void setDrunkState(bool drunkArg)
    {
        playerIsDrunk = drunkArg;
    }

    public void setEnergyState(bool energyArg)
    {
        playerEnergyNotNeutral = energyArg;
    }

    public void setBladderState(bool bladderArg)
    {
        playerIsPeeing = bladderArg;
    }

    public void setHealth(int newHealth)
    {
        health = newHealth;
    }

    public int getHealth() {
        return health;
    }

    public void setSoberness(float newSoberness, bool drunkOverride = false)
    {
        if (!drunkOverride)
            soberness += newSoberness;
        else
            soberness = newSoberness;

        //ensure the float value for soberness is exactly 0.0f if it drops this low
        if (soberness <= 0.0f)
            soberness = 0.0f;
    }

    public float getSoberness()
    {
        return soberness;
    }

    public void setEnergy(float newEnergy, bool energyOverride = false)
    {
        if (!energyOverride)
        {
            if ((energy + newEnergy) > 100.0f)
                energy = 100.0f;
            else
                energy += newEnergy;
        } 
        else if (energy <= 100.0f)
            energy = newEnergy;
        else
            energy = 100.0f;
    }

    public float getEnergy()
    {
        return energy;
    }

    public void setBladder(float newBladder, bool bladderOverride = false)
    {
        if (!bladderOverride)
            bladder += newBladder;
        else
            bladder = newBladder;
    }

    public float getBladder()
    {
        return bladder;
    }

    // Update is called once per frame
    private void Update ()
    {
        //debug command to instalty kill the player
        debugSetHP();

        //debug command to instalty make the player drunk
        debugSetSoberness();

        //debug command to instantly change the players energy levels
        debugSetEnergy();

        //debug command to instantly make the player pee
        debugSetBladder();

        //knock the player out
        if ((health == 0 || soberness == 0.0f || energy == 0.0f) && !playerIsKnockedOut)
        {
            GameObject knockedOutObj = Instantiate(Resources.Load("Prefabs/KnockedOut"), Vector3.zero, Quaternion.identity) as GameObject;

            if (health == 0)
                knockedOutObj.GetComponent<KnockedOut>().setMode(KnockedOut.Mode.Dead);
            else if (soberness == 0.0f)
                knockedOutObj.GetComponent<KnockedOut>().setMode(KnockedOut.Mode.Drunk);
            else if (energy == 0.0f)
                knockedOutObj.GetComponent<KnockedOut>().setMode(KnockedOut.Mode.Exhausted);
        }

        //make the player drunk
        if (soberness < 100.0f && !playerIsDrunk)
        {
            GameObject drunkObj = Instantiate(Resources.Load("Prefabs/Drunk"), Vector3.zero, Quaternion.identity) as GameObject;
        }

        //make the player tired or hyper
        if (energy != 50.0f && !playerEnergyNotNeutral)
        {
            GameObject energyObj = Instantiate(Resources.Load("Prefabs/Energy"), Vector3.zero, Quaternion.identity) as GameObject;
        }

        //make the player pee
        if ((bladder <= 0.0f || forcedPee) && !playerIsPeeing) {
            forcedPee = false;

            GameObject peeObj = Instantiate(Resources.Load("Prefabs/Pee"), Vector3.zero, Quaternion.identity) as GameObject;
        }

        //slowly lower the player's bladder value over time
        if (!playerIsPeeing)
            lowerPeeValue();

        startPlayerPeeing();
    }

    private void lowerPeeValue()
    {
        float bladderDelta = (0.5f * Time.deltaTime) * -1;
        setBladder(bladderDelta);
    }

    private void startPlayerPeeing()
    {
        if (Input.GetButtonDown("Pee"))
        {
            if (getBladder() <= 90) {
                forcedPee = true;
                print("player will now pee.");
            }
            else
            {
                print("boi, you don't need a pee yet!");
            }
        }
    }

    //DEBUG COMMANDS BELOW | NOT FOR RELEASE
    private void debugSetSoberness() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (Input.GetKeyDown(KeyCode.E)) {
                setSoberness(-10.0f);
                print("Player is now " + (100 - getSoberness()) + "% drunk.");
            }
        }
    }

    private void debugSetHP()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                setHealth(0);
                print("HP is now " + getHealth() + ".");
            }
        }
    }

    private void debugSetEnergy()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                setEnergy(10.0f);
                print("Player now has " + getEnergy() + " energy.");
            }
        }
    }

    private void debugSetBladder()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                setBladder(0.0f, true);
                print("Player now has " + getBladder() + " bladder.");
            }
        }
    }
    //END DEBUG COMMANDS
}
