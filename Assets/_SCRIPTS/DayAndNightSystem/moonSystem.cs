using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonSystem : MonoBehaviour
{
    private Transform player;

    // Use this for initialization
    private void Start ()
    {
        //gets the transform of the player
        player = GameObject.Find("Character").transform;
    }
	
	// Update is called once per frame
	private void Update ()
    {
        //rotates the moon so that it always faces the player
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, player.position - transform.position, 180.0f, 0.0f));
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
