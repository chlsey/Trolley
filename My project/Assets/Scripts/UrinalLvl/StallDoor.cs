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
    private bool interactionEnabled = true;

    void Start()
    {
        SetPromptVisible(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!interactionEnabled)
        {
            return;
        }

        playerInRange = true;
        SetPromptVisible(true);
    }

    private void OnTriggerExit(Collider other)
    {
        playerInRange = false;
        SetPromptVisible(false);
    }

    void Update()
    {
        if (!interactionEnabled)
        {
            return;
        }

        if (playerInRange && !isOpening && (Input.GetKeyDown(KeyCode.E) || (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame)))
        {
            SetPromptVisible(false);
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

    public void SetInteractionEnabled(bool enabled)
    {
        interactionEnabled = enabled;

        if (!interactionEnabled)
        {
            playerInRange = false;
            SetPromptVisible(false);
        }
    }

    public void HidePrompt()
    {
        playerInRange = false;
        SetPromptVisible(false);
    }

    private void SetPromptVisible(bool visible)
    {
        if (ePrompt != null)
        {
            ePrompt.SetActive(visible);
        }
    }
}
