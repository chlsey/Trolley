using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Urinal : MonoBehaviour
{
    public Door door; 
    public AudioSource[] applauseSources;
    public AudioSource introSource;
    public Animator anim;
    public Animator Pee1anim;
    public Animator Pee2anim;
    public Animator Pee3anim;
    public Animator Pee4anim;
    public GameObject ePrompt;
    public AudioSource audioSource;
    public AudioClip PeeSFX;
    public AudioClip Zipper;
    public AudioClip correctVoiceLine;
    public AudioClip wrongVoiceLine;
    public AudioClip leadToDoorVoiceLine;
    public PlayerMovement playerMovement;
    public FacePlayerModel facePlayer; 
    public FacePlayerModel facePlayer2; 
    public FacePlayerModel facePlayer3; 
    public FacePlayerModel facePlayer4; 
    public bool correctUrinal;
    public bool inCorrectUrinalRight;
    

    private bool nearUrinal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        door.enabled = false;
        foreach (AudioSource source in applauseSources)
        {
            source.enabled = false;
        }
        nearUrinal = false;
        ePrompt.SetActive(false);
        facePlayer2.enabled = false;
        facePlayer.enabled = false;
        facePlayer3.enabled = false;
        facePlayer4.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(correctUrinal)
        {
            if (nearUrinal && (Input.GetKeyDown(KeyCode.E) || (Gamepad.current != null && Gamepad.current.buttonWest.isPressed)))
            {
                anim.SetTrigger("PlayerPee");
                Pee1anim.SetTrigger("Applause");
                Pee2anim.SetTrigger("Applause");
                Pee3anim.SetTrigger("Applause");
                Pee4anim.SetTrigger("Applause");

                audioSource.PlayOneShot(PeeSFX);
                StartCoroutine(DisableMovement());
                introSource.enabled = false;
                facePlayer.enabled = true;
                facePlayer2.enabled = true;
                facePlayer3.enabled = true;
                facePlayer4.enabled = true;
                foreach (AudioSource source in applauseSources)
                {
                    source.enabled = true;
                }
                audioSource.PlayOneShot(correctVoiceLine);

                VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
                {
                    new VOManager.SubtitleLine(" Yes! Yes! what a fine choice!", 140f, 4200f),

                });

                door.enabled = true;
                StartCoroutine(WaitForExitVoiceline());
                
            }
        }
        if(inCorrectUrinalRight)
        {
            if (nearUrinal && (Input.GetKeyDown(KeyCode.E) || (Gamepad.current != null && Gamepad.current.buttonWest.isPressed)))
            {
                VOManager.Instance.StopBackgroundMusic();
                introSource.enabled = false;
                anim.SetTrigger("PlayerPee");
                
                
                StartCoroutine(SlapRightWithDelay());
                audioSource.PlayOneShot(PeeSFX);
                audioSource.PlayOneShot(wrongVoiceLine);

                VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
                {
                    new VOManager.SubtitleLine("…What", 140f, 2000f),
                    new VOManager.SubtitleLine("You’ve broken the sacred bathroom rules.", 2000f, 3000f),
                    new VOManager.SubtitleLine("Disgusting..", 5000f, 3000f),

                });
                
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        nearUrinal = true;
        ePrompt.SetActive(true);
        Debug.Log("NearUrinal");
    }
    private void OnTriggerExit(Collider other)
    {
        nearUrinal = false;
        ePrompt.SetActive(false);
        Debug.Log("AwayFromUrinal");
    }
     IEnumerator DisableMovement()
    {
        playerMovement.enabled = false;
        yield return new WaitForSeconds(10);
        playerMovement.enabled = true;
    }
    IEnumerator SlapRightWithDelay()
    {
        yield return new WaitForSeconds(3);
        Pee2anim.SetTrigger("SlapRight");
    }
    IEnumerator WaitForExitVoiceline() {
        yield return new WaitForSeconds(8f);
        audioSource.PlayOneShot(leadToDoorVoiceLine);
        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Anyways, I'll meet you outside once you're done.", 0f, 4000f),

        });
    } 
}
