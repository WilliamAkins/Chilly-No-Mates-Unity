using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StarSystem : MonoBehaviour
{
    public Color starColour = Color.white;
    public Color lerpedColour = Color.white;
    public int maxStars = 600;
    public float starSize = 10.0f;
    public float starMaxDistance = 600.0f;
    public float starMinDistance = 300.0f;

    //stores the alpha value of all the stars
    private float starAlpha = 1.0f;

    private ParticleSystem.Particle[] points;

    private DayNightCycle dayNightCycle;

    private void createStars() {
        points = new ParaticleSystem.Particle[maxStars];

        //loop through each star and set its initial position, colour and size
        for (int i = 0; i < maxStars; i++) {
            points[i].position = Random.onUnitSphere * Random.Range(starMinDistance, starMaxDistance) + transform.position;
            points[i].startColor = new Color(starColour.r, starColour.g, starColour.b, starColour.a);
            points[i].startSize = starSize;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, starMinDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, starMaxDistance);
    }

    // Use this for initialization
    private void Start()
    {
        //gets a reference to the day and night cycle
        dayNightCycle = GameObject.Find("DayAndNightSystem").GetComponent<DayNightCycle>();

        createStars();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //checks which state the day and night cycle is in and adjusts the stars depending on that
        switch (dayNightCycle.getTimeState())
        {
            case DayNightCycle.TimeState.Sunrise:
                //Fade out the stars
                starAlpha -= dayNightCycle.getInterpolationSpeed();

                for (int i = 0; i < maxStars; i++)
                    points[i].startColor = new Color(points[i].startColor.r, points[i].startColor.g, points[i].startColor.b, starAlpha);

                break;
            case DayNightCycle.TimeState.Day:
                //Don't display the stars
                if (starAlpha != 0.0f)
                {
                    starAlpha = 0.0f;

                    for (int i = 0; i < maxStars; i++)
                        points[i].startColor = new Color(points[i].startColor.r, points[i].startColor.g, points[i].startColor.b, 0.0f);
                }

                break;
            case DayNightCycle.TimeState.Sunset:
                //Fade in the stars
                starAlpha += dayNightCycle.getInterpolationSpeed();

                for (int i = 0; i < maxStars; i++)
                    points[i].startColor = new Color(points[i].startColor.r, points[i].startColor.g, points[i].startColor.b, starAlpha);

                break;
            case DayNightCycle.TimeState.Night:
                //Run the star system
                for (int i = 0; i < maxStars; i++)
                {
                    //each frame change the colours of the stars
                    lerpedColour = Color.Lerp(Color.white, starColour, Mathf.PingPong(Time.time, Random.Range(1.0f, 2.0f)));
                    points[i].startColor = new Color(lerpedColour.r, lerpedColour.g, lerpedColour.b, 1.0f);
                    //slightly change the size of the stars each frame
                    points[i].startSize = Random.Range(starSize - 1, starSize + 1);
                }
                break;
            default:
                break;
        }

        GetComponent<ParticleSystem>().SetParticles(points, points.Length);
    }
}
