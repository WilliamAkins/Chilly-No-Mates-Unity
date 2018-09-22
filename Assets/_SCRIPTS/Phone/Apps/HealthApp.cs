using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthApp : MonoBehaviour {

    private Vitals vitals;
    private GameObject phone;
    private bool backgroundUpdate = true;
    public Material background;
    public Material homescreen;

    int drunkLevel;
    int soberLevel;
    bool timedReset = false;
    float timer = 0.0f;


    // Use this for initialization
    void Start ()
    {
        vitals = GameObject.Find("Character").GetComponent<Vitals>();
        phone = GameObject.FindGameObjectWithTag("Phone").transform.GetChild(0).gameObject;
    }
	
	// Update is called once per frame
	void Update ()
    {

        if (timedReset)
            timer += 1.0f * Time.deltaTime;

        if (timedReset && timer >= 3.5f)
            {
                timedReset = false;
                ResetBar();
            }
        //Updates the background if it is still the home screen.
        if (backgroundUpdate == true)
        {
            phone.transform.GetChild(0).GetComponent<MeshRenderer>().material = background;
            backgroundUpdate = false;
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            close();
        }

        transform.Find("HP").GetComponent<TextMesh>().text = vitals.health.ToString();
        transform.Find("Energy").GetComponent<TextMesh>().text = ((int)Mathf.Round(vitals.energy)).ToString();
        transform.Find("Bladder").GetComponent<TextMesh>().text = ((int)Mathf.Round(vitals.bladder)).ToString();

        drunkLevel = (int)Mathf.Round(vitals.soberness) / 10;
        soberLevel = 10 - drunkLevel;


        if (!vitals.getKnockedOutState())
        {
            for (int i = 0; i < soberLevel; i++)
            {
                transform.Find("Drunk Level (" + (i + 1).ToString() + ")").gameObject.SetActive(true);
            }

            for (int i = soberLevel; i <= 10; i++)
            {
                if (soberLevel != 0)
                    transform.Find("Drunk Level (" + i.ToString() + ")").gameObject.SetActive(false);
            }
        }

        if (vitals.getKnockedOutState())
        {
            transform.Find("Drunk Level (10)").gameObject.SetActive(true);
            timedReset = true;
            print("hit");
        }
    }

    private void ResetBar()
    {
        for (int i = 1; i < 11; i++)
        {
            transform.Find("Drunk Level (" + i.ToString() + ")").gameObject.SetActive(false);
            timer = 0.0f;
        }
    }

    void close()
    {
        //Resets phone to home screen to close the app
        phone.transform.GetChild(0).GetComponent<MeshRenderer>().material = homescreen;
        phone.GetComponent<MobilePhone>().appClosed = true;
        backgroundUpdate = true;
        gameObject.SetActive(false);
    }
}
