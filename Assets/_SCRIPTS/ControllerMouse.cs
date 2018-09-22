using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMouse : MonoBehaviour {

    public float clickTime = 1f;
    public float sensitivity = 5f;
    private Vector2 _pos;

    bool clickBool = true;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Cursor.lockState == CursorLockMode.None)
        {
            _pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (Input.GetAxis("ControllerX") != 0)
            {
                _pos.x += Input.GetAxis("ControllerX") * sensitivity;
            }
            if (Input.GetAxis("ControllerY") != 0)
            {
                _pos.y += Input.GetAxis("ControllerY") * sensitivity;
            }
            CursorControl.SetLocalCursorPos(_pos);
            
            if (Input.GetAxisRaw("ControllerA") != 0 && clickBool)
            {
                clickBool = false;
                StartCoroutine(resetClick(clickTime));
                Debug.Log("Pressed button!");
                CursorControl.SimulateLeftClick();
            }

            if(Input.GetAxisRaw("DPadVertical") != 0)
            {
                sensitivity += Input.GetAxisRaw("DPadVertical");
                if(sensitivity <= 0)
                {
                    sensitivity = 1;
                }
                if (sensitivity >= 100)
                {
                    sensitivity = 100f;
                }
            }
        }
	}
    IEnumerator resetClick(float click)
    {
        float ResumeTime = Time.realtimeSinceStartup + click;
        while (Time.realtimeSinceStartup < ResumeTime)
        {
            yield return null;
        }
        clickBool = true;
    }
}
