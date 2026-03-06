using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleController : MonoBehaviour
{
    [SerializeField]
    ClockBehavior clockBehavior;


    // Update is called once per frame
    void Update()
    {
        if(clockBehavior.rotate)
        {
            Gamepad.current.SetMotorSpeeds(clockBehavior.trolleyProgress, clockBehavior.trolleyProgress);
        }
    }

    public async void OnHittingVictim()
    {
        Gamepad.current.SetMotorSpeeds(0.8f, 1.0f);

        await Task.Delay(200);

        // stop vibrating
        Gamepad.current.ResetHaptics();
    }
}
