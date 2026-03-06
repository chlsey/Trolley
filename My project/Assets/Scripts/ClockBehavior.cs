using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClockBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject trolley;
    [SerializeField]
    private GameObject startPoint;
    [SerializeField]
    private GameObject endPoint;
    [SerializeField]
    private GameObject clockHand;
    public bool rotate; // bool to make trolley stop once its past end point
    public float trolleyProgress; // a value from 0 to 1 of how far along the trolley is along the path
    private float pathLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotate = true;
        trolleyProgress = 0f;
        pathLength = Vector3.Distance(startPoint.transform.position, endPoint.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        trolleyProgress = Vector3.Distance(startPoint.transform.position, trolley.transform.position) / pathLength;
        if(rotate)
        {
            clockHand.transform.localRotation = Quaternion.Euler(0, 0, -trolleyProgress * 360);
            // Gamepad.current.SetMotorSpeeds((float)Math.Pow(trolleyProgress, 2), (float)Math.Pow(trolleyProgress, 3));
            if (Vector3.Distance(trolley.transform.position, endPoint.transform.position) < 1)
            {
                // Gamepad.current.ResetHaptics();
                rotate = false;
            }
        }
        
    }
}
