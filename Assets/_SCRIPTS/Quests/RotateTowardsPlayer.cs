using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour {
    
    //Rotation specifically for Quads
    Transform player;
    

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Character").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(player != null)
        {
            //Rotation for the icon
            transform.LookAt(player.position);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x + 180, transform.eulerAngles.y, transform.eulerAngles.z + 180);

            //Scale for the icon
            float distance = (transform.position - player.position).magnitude;
            transform.localScale = new Vector3(distance * 0.1f, distance * 0.1f, distance * 0.1f);
        }
	}
}
