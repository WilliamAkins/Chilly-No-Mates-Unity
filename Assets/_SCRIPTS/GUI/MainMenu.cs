using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public GameObject MenuSystem;
    public GameObject character;

    PlayerController pc;
    CamMouseLook cml;
    private void Start()
    {
        character = GameObject.Find("Character");
        MenuSystem = this.transform.parent.gameObject;
        if(character != null)
        {
            pc = character.GetComponent<PlayerController>();
            cml = character.GetComponentInChildren<CamMouseLook>();
        }
    }
    public void playGame()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            MenuSystem.SetActive(false);
            cml.enabled = true;
            pc.enabled = true;
            transform.parent.gameObject.SetActive(false);
        }
        
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
