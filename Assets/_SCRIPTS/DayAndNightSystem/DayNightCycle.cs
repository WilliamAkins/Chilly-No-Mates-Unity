using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour 
{
    [Header("Length of day/night in actual minutes")]
    public float dayLengthInMins = 1;
    public float nightLengthInMins = 1;

    [Header("Start time as a 24-hour clock")]
    [Range(0,24)]
    public float startTimeInHours = 7;

    //total seconds in 1 day
    private const int SECS_IN_DAY = 24 * 3600;

    //the time that each time state begins
    private const int START_SUNRISE = 5 * 3600;
    private const int START_DAY = 7 * 3600;
    private const int START_SUNSET = 17 * 3600;
    private const int START_NIGHT = 19 * 3600;

    //References to various objects
    private Light sun;
    private GameObject[] streetLights;

    //stores the current state of the day night cycle
    public enum TimeState { Sunrise, Day, Sunset, Night }
    private TimeState currentTimeState = TimeState.Day;

    //the number of days that have passed
    private byte day = 1;
    //the total time the player entered, converted to seconds
    private float dayLengthInSecs = 0;
    //the current time of day in the game, in seconds
    private float time = 0;
    //says whether the once the time state has changed, has it updated anything that needed updating
    private bool stateUpdated = false;
    //This is the speed at which state transitions will occur with effects such as fading out lights etc
    private float interpolationSpeed = 0.0f;

    //----------[ START PUBLIC FUNCTIONS ]----------
    public int getTime()
    {
        return (int)time;
    }

    public byte getDay()
    {
        return day;
    }

    public float getInGameSecsPerRealSecs()
    {
        return (SECS_IN_DAY / dayLengthInSecs) * Time.deltaTime;
    }

    public float getInterpolationSpeed()
    {
        return interpolationSpeed;
    }

    public TimeState getTimeState() {
        //returns the current time state
        return currentTimeState;
    }
    //----------[ END PUBLIC FUNCTIONS ]----------

    private void setTimeState(TimeState timeState)
    {
        //Updates the current time state
        currentTimeState = timeState;
    }

    private void setInterpolationSpeed() {
        switch (currentTimeState) {
            case TimeState.Sunrise:
                interpolationSpeed = (1 / ((START_DAY - START_SUNRISE) / getInGameSecsPerRealSecs()));
                break;
            case TimeState.Sunset:
                interpolationSpeed = (1 / ((START_NIGHT - START_SUNSET) / getInGameSecsPerRealSecs()));
                break;
            default:
                interpolationSpeed = 0.0f;
                break;
        }
    }

    private void updateWorldLights(bool enableLights)
    {
        //Loop through each object with a streetlight tag and update its state
        foreach (GameObject streetLight in streetLights)
            streetLight.SetActive(enableLights);
    }

    private void updateSunrise()
    {
        sun.intensity += interpolationSpeed;

        //check if the state should update to the next one
        if (time >= START_DAY)
            setTimeState(TimeState.Day);
    }

    private void updateDay()
    {
        //update any changes to the system 1 time per state change
        if (!stateUpdated)
        {
            stateUpdated = true;
            updateWorldLights(false);
        }

        //check if the state should update to the next one
        if (time >= START_SUNSET)
        {
            stateUpdated = false;
            setTimeState(TimeState.Sunset);
        }
    }

    private void updateSunset()
    {
        sun.intensity -= interpolationSpeed;

        //check if the state should update to the next one
        if (time >= START_NIGHT)
            setTimeState(TimeState.Night);
    }

    private void updateNight()
    {
        //update any changes to the system 1 time per state change
        if (!stateUpdated)
        {
            stateUpdated = true;
            updateWorldLights(true);
        }

        //check if the state should update to the next one
        if (time >= START_SUNRISE && time < START_DAY)
        {
            stateUpdated = false;
            setTimeState(TimeState.Sunrise);
        }
    }

    // Use this for initialization
    private void Start () {
        //Get a reference to sun object
        sun = gameObject.transform.Find("Sun").GetComponent<Light>();

        //convert the minutes entered into seconds
        dayLengthInSecs = dayLengthInMins * 60;

        //Sets the current time to the entered time and moves the day and night system to the correct starting position
        time = startTimeInHours * 3600;
        transform.RotateAround(Vector3.zero, Vector3.left, 15 * startTimeInHours);

        //Gets all the street lights from the scene
        streetLights = GameObject.FindGameObjectsWithTag("StreetLight");
    }

    // Update is called once per frame
    private void Update ()
    {
        //rotates the light around a pivot; in degrees per second
        transform.RotateAround(Vector3.zero, Vector3.left, (360 / dayLengthInSecs) * Time.deltaTime);

        //updates the state transition interpolation speed each frame
        setInterpolationSpeed();

        //Stores the current game time in seconds
        time += getInGameSecsPerRealSecs();

        //If the day has ended, set it to the next day and reset the time
        if (time >= SECS_IN_DAY)
        {
            day++;
            time = 0;
        }

        //updates the current state of the day and night system
        switch(currentTimeState)
        {
            case TimeState.Sunrise:
                updateSunrise();
                break;
            case TimeState.Day:
                updateDay();
                break;
            case TimeState.Sunset:
                updateSunset();
                break;
            case TimeState.Night:
                updateNight();
                break;
            default:
                break;
        }
    }
}