using UnityEngine;
using UnityEngine.InputSystem;

public class StallDoor : MonoBehaviour
{
    public Animator anim;
    public Collider doorCollider; 
    public float disable = 1.5f; 

    private bool isOpening = false;
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerInRange = false;
    }

    void Update()
    {
        if (playerInRange && !isOpening && (Input.GetKeyDown(KeyCode.E) || (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame)))
        {
            isOpening = true;

            anim.SetTrigger("Open");
            Debug.Log("OpenStall");

            StartCoroutine(DisableCollider());
        }
    }

    private System.Collections.IEnumerator DisableCollider()
    {
        doorCollider.enabled = false;

        yield return new WaitForSeconds(disable);

        doorCollider.enabled = true;
    }
}