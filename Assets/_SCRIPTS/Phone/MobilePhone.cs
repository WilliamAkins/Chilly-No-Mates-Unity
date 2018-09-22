using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilePhone : MonoBehaviour
{

    public DayNightCycle dayNightCycle;
    public Transform player;
    public Transform playerCamera;
    public bool appClosed = false;

    private int selection = 0;
    private int oldSelection = 0;
    private bool errorMessage = false;
    private bool inApp = false;
    private bool showTut = true;
    private bool tutShown = false;

    private Transform HourHand, MinuteHand, SecondHand;
    private float hourTime, minuteTime, secondsTime, day;
    private GameManager gm;

    private bool left = false;
    private bool right = false;
    private bool up = false;
    private bool down = false;

    // Use this for initialization
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        Vector3 phoneDimentions = this.GetComponent<Renderer>().bounds.size;

        dayNightCycle = GameObject.Find("DayAndNightSystem").GetComponent<DayNightCycle>();

        transform.parent.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("Player").transform.Find("Phone Camera").GetComponent<Camera>();
        transform.parent.GetComponent<Canvas>().planeDistance = 1;

        HourHand = transform.Find("Home Screen").Find("Clock").Find("Clock Hands").Find("Hours");
        MinuteHand = transform.Find("Home Screen").Find("Clock").Find("Clock Hands").Find("Minutes");
        SecondHand = transform.Find("Home Screen").Find("Clock").Find("Clock Hands").Find("Seconds");
        hourTime = (dayNightCycle.getTime() / 3600) - 1;
        minuteTime = dayNightCycle.getTime() / 60;
        secondsTime = dayNightCycle.getTime();
        day = dayNightCycle.getDay();

        HourHand.transform.eulerAngles.Set(0, 0, 30 * hourTime);
    }

    private void Update()
    {
        if (errorMessage == false)
            HomeScreenSelection();

        else
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                transform.Find("Notifications").Find("Update Message").gameObject.SetActive(false);
                errorMessage = false;
            }
        }

        if (inApp == true)
        {
            if (appClosed == true)
                openHome();
        }


        if(showTut == true)
        {
            if (gm.ControllerCheck())
                transform.Find("Notifications").Find("Controller Tutorial").gameObject.SetActive(true);
            else
                transform.Find("Notifications").Find("Keyboard Tutorial").gameObject.SetActive(true);

            showTut = false;
            tutShown = true;
        }

        if (tutShown == true)
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                if (gm.ControllerCheck())
                    transform.Find("Notifications").Find("Controller Tutorial").gameObject.SetActive(false);
                else
                    transform.Find("Notifications").Find("Keyboard Tutorial").gameObject.SetActive(false);

                tutShown = false;
            }

        }

        UpdateClock();
    }

    void OnGUI()
    {
        transform.GetChild(1).GetComponent<TextMesh>().text = formatHours(dayNightCycle.getTime());
        transform.GetChild(2).GetComponent<TextMesh>().text = formatMins(dayNightCycle.getTime());
        transform.GetChild(3).GetComponent<TextMesh>().text = dayNightCycle.getDay().ToString();
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


    void HomeScreenSelection()
    {
        if (!inApp)
        {
            oldSelection = selection;

     
            if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.LeftArrow) || DpadLeft())
                selection--;
            if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.RightArrow) || DpadRight())
                selection++;
            
            if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.UpArrow) || DpadUp())
            {
                switch (selection)
                {
                    case 18:
                        {
                            selection = 16;
                            break;
                        }
                    case 19:
                        {
                            selection = 17;
                            break;
                        }
                    case 20:
                        {
                            selection = 17;
                            break;
                        }
                    case 21:
                        {
                            selection = 17;
                            break;
                        }

                    default:
                        {
                            selection -= 4;
                            break;
                        }
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.DownArrow) || DpadDown())
            {
                switch(selection)
                {
                     case 14:
                        {
                            selection = 17;
                            break;
                        }
                    case 15:
                        {
                            selection = 17;
                            break;
                        }
                    case 16:
                        {
                            selection = 18;
                            break;
                        }
                    case 17:
                        {
                            selection = 19;
                            break;
                        }
                default:
                    {
                            selection += 4;
                            break;
                    }
                }
            }

            if (selection < 0)
                selection = 0;
            if (selection > 21)
                selection = 21;

            transform.GetChild(4).GetChild(oldSelection).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            transform.GetChild(4).GetChild(oldSelection).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white);

            transform.GetChild(4).GetChild(selection).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            transform.GetChild(4).GetChild(selection).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.grey);

            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button5) )
                Launch(selection);
        }
    }

    private void Launch(int selection)
    {
        switch (selection)
        {

            case 0:
                {
                    //Mail
                    AppUnavailable();
                    break;
                }
            case 1:
                {
                    //Calendar
                    AppUnavailable();
                    break;
                }
            case 2:
                {
                    //Photos
                    AppUnavailable();
                    break;
                }
            case 3:
                {
                    //Camera
                    AppUnavailable();
                    break;
                }
            case 4:
                {
                    //Maps
                    AppUnavailable();
                    break;
                }
            case 5:
                {
                    //Clock
                    AppUnavailable();
                    break;
                }
            case 6:
                {
                    //Weather
                    AppUnavailable();
                    break;
                }
            case 7:
                {
                    //News
                    AppUnavailable();
                    break;
                }
            case 8:
                {
                    //Contacts
                    AppUnavailable();
                    break;
                }
            case 9:
                {
                    //Inventory
                    transform.Find("Applications").Find("Inventory").gameObject.SetActive(true);
                    hideHome();
                    inApp = true;
                    break;
                }
            case 10:
                {
                    //Compass
                    AppUnavailable();
                    break;
                }
            case 11:
                {
                    //Journal
                    transform.Find("Applications").Find("Journal").gameObject.SetActive(true);
                    hideHome();
                    inApp = true;
                    break;
                }
            case 12:
                {
                    //Barker
                    AppUnavailable();
                    break;
                }
            case 13:
                {
                    //App Market
                    AppUnavailable();
                    break;
                }
            case 14:
                {
                    //Banking
                    AppUnavailable();
                    break;
                }
            case 15:
                {
                    //Find Friends
                    AppUnavailable();
                    break;
                }
            case 16:
                {
                    //Health
                    transform.Find("Applications").Find("Health").gameObject.SetActive(true);
                    hideHome();
                    inApp = true;
                    break;
                }
            case 17:
                {
                    //Settings
                    AppUnavailable();
                    break;
                }
            case 18:
                {
                    //Phone
                    AppUnavailable();
                    break;
                }
            case 19:
                {
                    //Internet
                    AppUnavailable();
                    break;
                }
            case 20:
                {
                    //Messages
                    AppUnavailable();
                    break;
                }
            case 21:
                {
                    //Music
                    AppUnavailable();
                    break;
                }
        }
    }

    private void hideHome()
    {
        //transform.GetChild(0)... change background here
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false);
    }

    private void openHome()
    {
        //transform.GetChild(0)... change background here
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(true);
        transform.GetChild(4).gameObject.SetActive(true);

        inApp = false;
        appClosed = false;
    }

    private void AppUnavailable()
    {
        if(tutShown == false)
        {
            transform.GetChild(6).GetChild(0).gameObject.SetActive(true);
            errorMessage = true;
        }
    }

    private string formatMins(int currentTime)
    {
        string newTime;
        int mins;
        currentTime %= 3600;
        mins = currentTime / 60;

        string minuteUpdate;

        if (mins < 10)
            minuteUpdate = "0" + mins.ToString();
        else
            minuteUpdate = mins.ToString();
        newTime = minuteUpdate;

        return newTime;
    }
    private string formatHours(int currentTime)
    {
        string newTime;
        int hours;
        hours = currentTime / 3600;
        currentTime %= 3600;

        string hourUpdate;
        hourUpdate = hours.ToString();

        newTime = hourUpdate + ":";

        return newTime;
    }

    private void UpdateClock()
    {
        float hour, min, sec;

        hour = dayNightCycle.getTime();
        hour = hour / 3600;

        min = dayNightCycle.getTime();
        min = min / 60;

        sec = dayNightCycle.getTime();

        if (hour > hourTime + 1)
        {
            float temp = hour;
            hour -= hourTime;
            HourHand.Rotate(0, 0, 30 * hour);
            hourTime = temp;
        }

        if (min > minuteTime + 1)
        {
            float temp = min;
            min -= minuteTime;
            MinuteHand.Rotate(0, 0, 6 * min);
            minuteTime = temp;
        }

        if (sec > secondsTime + 1)
        {
            float temp = sec;
            sec -= secondsTime;
            SecondHand.Rotate(0, 0, 6 * sec);
            secondsTime = temp;

        }

        if (dayNightCycle.getDay() > day)
        {
            hourTime = dayNightCycle.getTime() / 3600;
            minuteTime = dayNightCycle.getTime() / 60;
            secondsTime = dayNightCycle.getTime();
            HourHand.Rotate(0, 0, 30);
            day++;
        }
    }

}