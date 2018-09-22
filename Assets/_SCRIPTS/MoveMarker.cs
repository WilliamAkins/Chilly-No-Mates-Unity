using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMarker : MonoBehaviour
{
    private Vector3 initialPos;
    private Vector3 initialRot;

    private float yModifier = 0.0f;
    private sbyte ext = 1;

    private float newRot = 0.0f;

    // Use this for initialization
    private void Start ()
    {
        //set the initial rotation of the marker
        transform.eulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);

        //get initial transforms of the marker
        initialPos = transform.position;
        initialRot = transform.eulerAngles;
    }
	
	// Update is called once per frame
	private void Update ()
    {
        //ensures the marker goes up and down over time with an extension to the yModifier
        if (yModifier >= 1.0f)
            ext = -1;
        else if (yModifier <= 0.0f)
            ext = 1;

        //modifies the y value of the marker
        yModifier += (1.0f * Time.deltaTime) * ext;

        newRot += (50.0f * Time.deltaTime);

        //sets the new position of the marker
        transform.position = new Vector3(initialPos.x, initialPos.y + yModifier, initialPos.z);

        transform.eulerAngles = new Vector3(initialRot.x, newRot, initialRot.z);
    }
}
