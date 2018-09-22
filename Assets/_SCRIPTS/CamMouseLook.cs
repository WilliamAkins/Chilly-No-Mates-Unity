using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMouseLook : MonoBehaviour {

    Vector2 mouseLook;
    Vector2 smoothV;
    public float sensitivity = 5.0f;
    public float smoothing = 2.0f;

    [SerializeField]
    Vitals vitalClass;

    GameObject character;

    void Start()
    {
        character = this.transform.parent.gameObject;
    }

    void Update()
    {
        if (vitalClass.getSoberness() != 100.0f)
        {
            var md = new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

            md = Vector2.Scale(md, new Vector2(sensitivity * 2 * smoothing, sensitivity * 2 * smoothing));
            //lerp the x and y pos of the mouse as the player moves the mouse
            smoothV.x = Mathf.Lerp(smoothV.x, md.x, Time.deltaTime * smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, md.y, Time.deltaTime * smoothing);

            mouseLook += smoothV;
        }
        else
        {
            var md = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
            smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
            mouseLook += smoothV;
        }
        

        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);

        
    }

    void FixedUpdate()
    {
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }
}
