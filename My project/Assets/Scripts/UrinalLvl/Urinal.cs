using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Urinal : MonoBehaviour
{
    public UrinalDilemmaState state;
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
        foreach (AudioSource source in applauseSources)
        {
            source.enabled = false;
        }
        nearUrinal = false;
        SetPromptVisible(false);

        if (facePlayer2 != null) facePlayer2.enabled = false;
        if (facePlayer != null) facePlayer.enabled = false;
        if (facePlayer3 != null) facePlayer3.enabled = false;
        if (facePlayer4 != null) facePlayer4.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(correctUrinal)
        {
            if (nearUrinal && (Input.GetKeyDown(KeyCode.E) || (Gamepad.current != null && Gamepad.current.buttonWest.isPressed)))
            {
                if (state != null && !state.TryCommitUrinal(this))
                {
                    return;
                }

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

                if (state != null)
                {
                    state.UnlockExit();
                }
                StartCoroutine(WaitForExitVoiceline());
                
            }
        }
        if(inCorrectUrinalRight)
        {
            if (nearUrinal && (Input.GetKeyDown(KeyCode.E) || (Gamepad.current != null && Gamepad.current.buttonWest.isPressed)))
            {
                if (state != null && !state.TryCommitUrinal(this))
                {
                    return;
                }

                VOManager.Instance.StopBackgroundMusic();
                introSource.enabled = false;
                anim.SetTrigger("PlayerPee");
                playerMovement.moveSpeed = 0;
                
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
        if (state != null && state.HasUrinalSelection)
        {
            return;
        }

        nearUrinal = true;
        SetPromptVisible(true);
        Debug.Log("NearUrinal");
    }
    private void OnTriggerExit(Collider other)
    {
        nearUrinal = false;
        SetPromptVisible(false);
        Debug.Log("AwayFromUrinal");
    }
    public void HidePrompt()
    {
        nearUrinal = false;
        SetPromptVisible(false);
    }
    private void SetPromptVisible(bool visible)
    {
        if (ePrompt != null)
        {
            ePrompt.SetActive(visible);
        }
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
