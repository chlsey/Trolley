using UnityEngine;
using UnityEngine.InputSystem;

public class Finger : MonoBehaviour
{
    public Animator animator;
    public string leverTag = "Lever";

    private bool nearLever = false;

    void Update()
    {
        bool pressedE = Input.GetKeyDown(KeyCode.E);
        bool pressedGamepad = Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame;

        if (nearLever && (pressedE || pressedGamepad))
        {
            if (animator != null)
            {
                animator.SetTrigger("Switch");
            }
            else
            {
                Debug.LogWarning("Animator is not assigned on Finger script!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check BOTH layer and tag
        if (other.CompareTag(leverTag))
        {
            nearLever = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(leverTag))
        {
            nearLever = false;
        }
    }
}