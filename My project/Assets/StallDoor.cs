using UnityEngine;
using UnityEngine.InputSystem;

public class StallDoor : MonoBehaviour
{
    public Animator anim;
    public AudioSource audio;
    public AudioClip fart;
    public Collider doorCollider; 
    public float disable = 1.5f; 
    public GameObject ePrompt;

    private bool isOpening = false;
    private bool playerInRange = false;

    void Start()
    {
        ePrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        playerInRange = true;
        ePrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        playerInRange = false;
        ePrompt.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && !isOpening && (Input.GetKeyDown(KeyCode.E) || (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame)))
        {
            ePrompt.SetActive(false);
            isOpening = true;
            
            anim.SetTrigger("Open");
            Debug.Log("OpenStall");
            audio.PlayOneShot(fart);
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