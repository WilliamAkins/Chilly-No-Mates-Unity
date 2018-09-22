using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class BasicAI : MonoBehaviour {
   
    private NavMeshAgent nav;

    private AIHub hub;

    private Transform target;
    private Vector3 tempTarget;
    public Transform home;

    private Transform player;

    [SerializeField]
    private float chanceOfExploration = 5;
    [SerializeField]
    private float chanceOfReturningToHome = 1;
    [SerializeField]
    private float pauseTime = 0f;
    [SerializeField]
    private float pauseDistance = 1.0f;

    bool atLocation = false;

    List<Vector3> previousTargets = new List<Vector3>();


	// Use this for initialization
	void Start () {
        hub = GameObject.Find("AIHub").GetComponent<AIHub>();
        nav = this.GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (home == null) home = this.transform;
        if (pauseTime == 0f) pauseTime = Random.Range(1.0f, 7.0f);
        if (target == null)
        {
            target = hub.findClosestPoint(this.transform, ref previousTargets);
            tempTarget = target.position;
            nav.SetDestination(tempTarget);
        }


	}
	
	// Update is called once per frame
	void Update () {
        if (!atLocation)
        {
            if (Vector3.Distance(target.position, this.transform.position) <= pauseDistance)
            {
                nav.SetDestination(this.transform.position);
                nav.isStopped = true;

                atLocation = true;

                float random = Random.Range(0.0f, 100.0f);
                
                if (random <= chanceOfReturningToHome)
                    target = home;
                else if (random <= chanceOfExploration)
                    target = hub.findRandomPoint();
                else
                    target = hub.findClosestPoint(transform,ref previousTargets);
                
                Invoke("setNewDestination", pauseTime);
            }
        }
        if(Vector3.Distance(this.transform.position, player.position) <= pauseDistance)
        {
            if (!nav.isStopped) nav.isStopped = true;
        }
        else
        {
            if(nav.isStopped)nav.isStopped = false;
        }
	}

    void setNewDestination()
    {

        nav.enabled = true;
        tempTarget = target.position;
        nav.SetDestination(tempTarget);
        atLocation = false;
    }


}
