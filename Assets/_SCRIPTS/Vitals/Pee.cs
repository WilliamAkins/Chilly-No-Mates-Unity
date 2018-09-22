using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pee : MonoBehaviour
{
    private float changeBladderSpeed = 20.0f; //how fast the bladder var will fill up in seconds

    private GameObject player;
    private Vitals vitals;
    private CamMouseLook camMouseLook;

    private float timer = 0.0f;

    // Use this for initialization
    void Start ()
    {
        //get some components & objects
        player = GameObject.Find("Character");
        vitals = player.GetComponent<Vitals>();
        camMouseLook = player.transform.Find("FPPCamera").GetComponent<CamMouseLook>();

        vitals.setBladderState(true);

        //set the initial position to the player position
        transform.position = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //each frame ensure the pee position is on the player
        transform.position = player.transform.position;
        transform.eulerAngles = new Vector3(camMouseLook.transform.eulerAngles.x, player.transform.eulerAngles.y + 90.0f, 0.0f);

        vitals.setBladder((100.0f / changeBladderSpeed) * Time.deltaTime);

        if (vitals.getBladder() >= 100.0f)
        {
            //ensures the bladder value is exactly 100.0f | 100%
            vitals.setBladder(100.0f, true);

            //destroyScript();

            ParticleSystem emitter = GetComponent<ParticleSystem>();
            emitter.Stop();

            //create a timer before deleting the object
            timer += 1.0f * Time.deltaTime;
            if (timer >= 5.0f)
                destroyScript();
        }
    }

    private void destroyScript()
    {
        vitals.setBladderState(false);

        //deletes this game object
        Destroy(gameObject);

        print("You finished peeing boi!");
    }
}
