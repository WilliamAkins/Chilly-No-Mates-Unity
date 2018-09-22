using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHub : MonoBehaviour {

    private GameObject[] pointsOfInterest;

	// Use this for initialization
	void Awake () {
        pointsOfInterest = GameObject.FindGameObjectsWithTag("AIMarker");
	}
	
	// Update is called once per frame
	void Update () {
	}

    public Transform findClosestPoint(Transform AI,ref List<Vector3> target)
    {
        float closest = 300000f;
        Transform freshTarget = this.transform;
        foreach (GameObject go in pointsOfInterest)
        {
            if(target.Count > 1)
            {
                if (go.transform.position != target[target.Count - 1] && go.transform.position != target[target.Count -2])
                {
                    float tempDist = Vector3.Distance(AI.position, go.transform.position);
                    if (tempDist <= closest)
                    {
                        Debug.Log("Found closest!");
                        closest = tempDist;
                        freshTarget = go.transform;
                    }
                }
            }
            else
            {
                    float tempDist = Vector3.Distance(AI.position, go.transform.position);
                    if (tempDist <= closest && tempDist >= 3f)
                    {
                        closest = tempDist;
                        freshTarget = go.transform;
                    }
            }
            
        }
        
        target.Add(freshTarget.position);
        return freshTarget;
    }

    public Transform findRandomPoint()
    {
        int index = (int)Random.Range(0f, (float)pointsOfInterest.Length);

        return pointsOfInterest[index].transform;
    }
}
