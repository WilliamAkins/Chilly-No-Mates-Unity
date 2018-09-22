using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockedOut : MonoBehaviour
{
    public AudioClip bird;
    public AudioClip thud;

    private Vitals vitals;

    private GameObject fadeTop;
    private GameObject fadeBottom;

    private GameObject player;
    private CamMouseLook CamMouseLook;
    private PlayerController playerController;

    private AudioSource audioSource;

    bool knockedOutFinished = false;
    bool alreadyResetting = false;

    private float audioTimer = 0.0f;
    private bool sfxHasPlayed = false;

    //stores the current mode that the knocked out script is in
    public enum Mode { Dead, Drunk, Exhausted }
    private Mode CurrentMode = Mode.Dead;

    public void setMode(Mode newMode) 
    {
        CurrentMode = newMode;
    }

    // Use this for initialization
    public void Start ()
    {
        player = GameObject.Find("Character");

        //gets a reference to the vitals class
        vitals = player.GetComponent<Vitals>();

        vitals.setKnockedOutState(true);

        Debug.Log("You've been knocked out...");
        knockedOutFinished = false;

        fadeTop = transform.Find("fadeTop").gameObject;
        fadeBottom = transform.Find("fadeBottom").gameObject;

        print("The current mode is " + CurrentMode);

        //slightly rotates the player's z angle so that they can fall over
        
        //player.transform.rotation = Quaternion.Euler(player.transform.eulerAngles.x, player.transform.eulerAngles.y, -10.0f);

        //disbales the player controller script
        playerController = player.GetComponent<PlayerController>();
        playerController.enabled = false;

        //gets the FPPCamera and then disables the Cam Mouse Look script
        CamMouseLook = player.transform.Find("FPPCamera").GetComponent<CamMouseLook>();
        CamMouseLook.enabled = false;

        //unlocks the Z axis so that the player can fall over
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        //Random force applied to the body provides a more natural fall.
        Vector3 forceLocation = Random.onUnitSphere * 20;
        player.GetComponent<Rigidbody>().AddForce(forceLocation, ForceMode.Impulse);

        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	private void FixedUpdate ()
    {
        if (!knockedOutFinished)
        {
            playerController.enabled = false;
            CamMouseLook.enabled = false;
            Transform fadeTopPos = fadeTop.transform;
            Transform fadeBottomPos = fadeBottom.transform;

            //play the audio once the timer has finished
            audioTimer += 1.0f * Time.deltaTime;
            if (audioTimer >= 1.8f && !sfxHasPlayed)
            {
                sfxHasPlayed = true;
                audioSource.PlayOneShot(thud, 0.6f);
            }

            //Moves the top black overlay down to the centre
            if (fadeTop.GetComponent<RectTransform>().localPosition.y > -120)
                fadeTop.transform.position = Vector3.Lerp(fadeTopPos.position, new Vector3(fadeTopPos.position.x, fadeTopPos.position.y - 4.0f), 100.0f * Time.deltaTime);

            //moves the bottom black overlay up to the centre
            if (fadeBottom.GetComponent<RectTransform>().localPosition.y < 120)
            {
                fadeBottom.transform.position = Vector3.Lerp(fadeBottomPos.position, new Vector3(fadeBottomPos.position.x, fadeBottomPos.position.y + 4.0f), 100.0f * Time.deltaTime);
            } 
            else
            {
                knockedOutFinished = true;
            }
        }
        else
        {
            resetPlayer();
        }
    }

    private GameObject findClosedPoint()
    {
        GameObject[] points;
        points = GameObject.FindGameObjectsWithTag("RespawnPoint");
        GameObject closest = null;
        float distance = Mathf.Infinity;

        //gets the players current position
        Vector3 position = player.transform.position;

        //loop through each respawn point, looking for the closest one
        foreach (GameObject point in points)
        {
            Vector3 diff = point.transform.position - position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance)
            {
                closest = point;
                distance = curDistance;
            }
        }
        return closest;
    }

    private void resetPlayer()
    {
        if (!alreadyResetting) {
            alreadyResetting = true;

            Debug.Log("Knocked out sequence finished.");

            audioSource.PlayOneShot(bird, 1.0f);

            //sets the player position to the nearest respawn point
            player.transform.position = findClosedPoint().transform.position;
            player.transform.rotation = Quaternion.Euler(0, 0, 0);

            //Freezes all the rotation axis
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

            switch (CurrentMode)
            {
                case Mode.Dead:
                    //gets a reference to the health that's attached to the player and then reset the health
                    vitals.setHealth(100);
                    break;
                case Mode.Drunk:
                    vitals.setSoberness(100.0f, true);
                    break;
                case Mode.Exhausted:
                    vitals.setEnergy(50);
                    break;
                default:
                    break;
            }
            vitals.setKnockedOutState(false);
        }

        Transform fadeTopPos = fadeTop.transform;
        Transform fadeBottomPos = fadeBottom.transform;

        //Moves the top black overlay down to the centre
        if (fadeTop.GetComponent<RectTransform>().localPosition.y < 1200)
            fadeTop.transform.position = Vector3.Lerp(fadeTopPos.position, new Vector3(fadeTopPos.position.x, fadeTopPos.position.y + 4.0f), 100.0f * Time.deltaTime);

        //moves the bottom black overlay up to the centre
        if (fadeBottom.GetComponent<RectTransform>().localPosition.y > -1200)
        {
            fadeBottom.transform.position = Vector3.Lerp(fadeBottomPos.position, new Vector3(fadeBottomPos.position.x, fadeBottomPos.position.y - 4.0f), 100.0f * Time.deltaTime);
        }
        else
        {
            //re-enables the disabled scripts
            playerController.enabled = true;
            CamMouseLook.enabled = true;

            //since the reset seqence has finished, destroy the script
            alreadyResetting = false;
            destroyScript();
        }
    }

    private void destroyScript()
    {
        //deletes this game object
        Destroy(gameObject);
    }
}
