using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatorControllerScript : MonoBehaviour {

    Animator anim;
    NavMeshAgent nav;

    private float speed = 0;
    Vector3 lastPos;
    bool isMoving;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        lastPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        speed = nav.velocity.magnitude;
        if (speed != 0) isMoving = true;
        else { isMoving = false; }
        //anim.SetFloat("Speed", speed);
        anim.SetBool("Moving", isMoving);
	}
    private void LateUpdate()
    {
        lastPos = this.transform.position;
    }
}
