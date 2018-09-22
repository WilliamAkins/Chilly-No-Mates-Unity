using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drunk : MonoBehaviour
{
    public float reduceDrunkSpeed = 60.0f; //In seconds, going from 0 - 100 soberness

    private Vitals vitals;
    private Camera FPPCamera;
    private Image drunkOverlay;
    private Image drunkFade;

    //private float sensitivityX = 15.0f;
    //private float sensitivityY = 15.0f;

    //private float smooth = 1.0f;
    //private float smoothXAxis;
    //private float smoothYAxis;

    //private float rotationX = 0.0f;
    //private float rotationY = 0.0f;

    private bool fovIncrease = true;
    private bool drunkEnding = false;
    private float fovModifier = 10.0f;

    private float initialFOV;
    private float resetFOVTimer = 0.0f;

    // Use this for initialization
    private void Start()
    {
        //get some components & objects
        GameObject player = GameObject.Find("Character");
        vitals = player.GetComponent<Vitals>();
        vitals.setDrunkState(true);

        FPPCamera = player.transform.Find("FPPCamera").GetComponent<Camera>();

        //get the 2 sub images within the drunk canvas object
        drunkOverlay = transform.Find("DrunkOverlay").GetComponent<Image>();
        drunkFade = transform.Find("Fade").GetComponent<Image>();

        //get the initial fov
        initialFOV = FPPCamera.fieldOfView;
        
    }

    private void Update()
    {
        //increases and lowers the fov over time
        if (fovIncrease) {
            FPPCamera.fieldOfView += fovModifier * Time.deltaTime;

            if (FPPCamera.fieldOfView >= 140.0f)
            {
                fovIncrease = false;
            }
        } 
        else
        {
            FPPCamera.fieldOfView -= fovModifier * Time.deltaTime;

            if (FPPCamera.fieldOfView <= 120.0f)
            {
                fovIncrease = true;
            }
        }

        //each frame, increase the soberness of the player
        vitals.setSoberness((100.0f / reduceDrunkSpeed) * Time.deltaTime);

        //slowly change the smooth value for the camera lerp each frame
        //smooth = vitals.getSoberness() / 10.0f;

        //slowly lower the fov modifer each frame
        fovModifier -= ((10.0f / reduceDrunkSpeed) * Time.deltaTime);

        //update the alpha of the drunk overlay image
        float drunkOverlayAlpha = (0.1f / 100.0f) * (100.0f - vitals.getSoberness());
        drunkOverlay.color = new Color(drunkOverlay.color.r, drunkOverlay.color.g, drunkOverlay.color.b, drunkOverlayAlpha);
        //print("drunkOverlayAlpha = " + drunkOverlay.color.a);

        //update the alpha of the drunk fade image
        float drunkFadeAlpha = (1.0f / 100.0f) * (100.0f - vitals.getSoberness());
        drunkFade.color = new Color(drunkFade.color.r, drunkFade.color.g, drunkFade.color.b, drunkFadeAlpha);
        //print("drunkFadeAlpha = " + drunkFade.color.a);

        //if the player is now sober again
        if (vitals.getSoberness() >= 100.0f)
        {
            drunkEnding = true;

            //ensures the sober value is exactly 100.0f | 100%
            vitals.setSoberness(100.0f, true);

            //reset the player fov over time
            FPPCamera.fieldOfView = Mathf.Lerp(FPPCamera.fieldOfView, initialFOV, resetFOVTimer);
            resetFOVTimer += (1.0f / 100.0f) * Time.deltaTime;
        }

        //only destroy the script one the fov has finished resetting
        if (FPPCamera.fieldOfView - Mathf.Abs(initialFOV) < 1.0f && drunkEnding)
            destroyScript();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        ////lerp the x and y pos of the mouse as the player moves the mouse
        //smoothXAxis = Mathf.Lerp(smoothXAxis, Input.GetAxis("Mouse X"), Time.deltaTime * smooth);
        //smoothYAxis = Mathf.Lerp(smoothYAxis, Input.GetAxis("Mouse Y"), Time.deltaTime * smooth);

        //rotationX += smoothXAxis * sensitivityX;
        //rotationY += smoothYAxis * sensitivityY;

        //FPPCamera.transform.localRotation = Quaternion.AngleAxis(rotationY, Vector3.right);
        //FPPCamera.transform.parent.localRotation = Quaternion.AngleAxis(rotationX, FPPCamera.transform.parent.up);
    }

    private void destroyScript()
    {
        vitals.setDrunkState(false);

        //deletes this game object
        Destroy(gameObject);

        print("You sober again boi!");
    }
}
