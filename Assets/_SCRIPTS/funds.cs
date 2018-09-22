using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class funds : MonoBehaviour
{
    public double money;
	// Use this for initialization
	void Start ()
    {
        money = 400;	
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void addingFunds(double add)
    {
        money = money + add;
    }

    public void removingFunds(double sub)
    {
        money = money - sub;
    }
}
