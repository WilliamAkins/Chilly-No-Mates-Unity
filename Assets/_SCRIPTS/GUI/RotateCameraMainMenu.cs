using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraMainMenu : MonoBehaviour
{

    public Transform target;
    public float rotationSpeed = 4f;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.Translate(Vector3.right * Time.deltaTime * rotationSpeed);
    }
}
