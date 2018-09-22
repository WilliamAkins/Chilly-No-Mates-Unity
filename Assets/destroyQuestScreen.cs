using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyQuestScreen : MonoBehaviour {
    [SerializeField]
    GameManager gameManager;

    PlayerController pc;
    CamMouseLook cml;
    // Use this for initialization
    void Start () {
        if (gameManager == null)
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pc = gameManager.character.GetComponent<PlayerController>();
        cml = gameManager.character.GetComponentInChildren<CamMouseLook>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Pause") != 0 && transform.GetChild(0).gameObject.activeSelf)
        {
            removeQuestScreenCanvas();
        }
	}

    public void removeQuestScreenCanvas()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.GetChild(0).gameObject.SetActive(false);
        cml.enabled = true;
        pc.enabled = true;
        Time.timeScale = 1.0f;
    }
}
