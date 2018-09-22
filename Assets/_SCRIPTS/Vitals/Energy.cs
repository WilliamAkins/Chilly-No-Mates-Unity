using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    private Vitals vitals;
    private Image energyOverlay;
    private Camera FPPCamera;
    private PlayerController playerController;

    private float reduceEnergySpeed = 30.0f; //in real seconds

    private float initialFOV;
    private float normalSpeed;
    private float sprintSpeed;

    private bool inHyperState = false;
    private bool inTiredState = false;

    // Use this for initialization
    private void Start ()
    {
        //find the vitals script and sets an inital value
        GameObject player = GameObject.Find("Character");
        vitals = player.GetComponent<Vitals>();
        vitals.setEnergyState(true);

        //get some initial references
        energyOverlay = transform.Find("Fade").GetComponent<Image>();
        FPPCamera = player.transform.Find("FPPCamera").GetComponent<Camera>();
        playerController = player.GetComponent<PlayerController>();

        //get the initial fov
        initialFOV = FPPCamera.fieldOfView;

        normalSpeed = playerController.getNormalSpeed();
        sprintSpeed = playerController.getSprintSpeed();
    }

    // Update is called once per frame
    private void Update ()
    {
        //if the player is tired, show the overlay
        if (vitals.getEnergy() < 50.0f)
        {
            inTiredState = true;

            float calcAlpha = (1.0f / 50.0f) * (50.0f - vitals.getEnergy());

            energyOverlay.color = new Color(energyOverlay.color.r, energyOverlay.color.g, energyOverlay.color.b, calcAlpha);

            //lower the player speed based on the energy
            playerController.modifyNormalSpeed(normalSpeed - ((50 - vitals.getEnergy()) / 22));
            playerController.modifySprintSpeed(sprintSpeed - ((50 - vitals.getEnergy()) / 22));

        }
        else if (vitals.getEnergy() >= 50.0f && inTiredState)
        {
            inTiredState = false;

            //reset some values back to their original state
            playerController.modifyNormalSpeed(normalSpeed);
            playerController.modifySprintSpeed(sprintSpeed);
            vitals.setEnergy(50.0f, true);

            destroyScript();
        }

        //if the player is hyper
        if (vitals.getEnergy() > 50.0f)
        {
            inHyperState = true;

            FPPCamera.fieldOfView = initialFOV + vitals.getEnergy() / 8.0f;

            //increase the player speed as they get more hyper
            playerController.modifyNormalSpeed(normalSpeed + (vitals.getEnergy() / 12));
            playerController.modifySprintSpeed(sprintSpeed + (vitals.getEnergy() / 12));

            //slowly lower the speed to a normal value
            vitals.setEnergy(((50.0f / reduceEnergySpeed) * Time.deltaTime) * -1);
        }
        else if(vitals.getEnergy() <= 50.0f && inHyperState)
        {
            inHyperState = false;

            //reset some values back to their original state
            FPPCamera.fieldOfView = initialFOV;
            playerController.modifyNormalSpeed(normalSpeed);
            playerController.modifySprintSpeed(sprintSpeed);
            vitals.setEnergy(50.0f, true);

            destroyScript();
        }
    }

    private void destroyScript() {
        vitals.setEnergyState(false);

        //deletes this game object
        Destroy(gameObject);

        print("You now have neutral energy.");
    }
}