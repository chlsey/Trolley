using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterTrucksVoiceController : MonoBehaviour
{
    public AudioClip introPt1;
    public AudioClip introPt2;
    public AudioClip introPt3;
    public AudioClip fail1;
    public AudioClip fail2;
    public AudioClip fail3;
    public AudioClip catMeow;
    public AudioClip themeSong;
    public AudioClip jumpSound;
    [Range(0f, 1f)]
    public float jumpVolume = 0.5f;
    public Health healthScript;
    public PlayerMovement playerMovement;
    public AudioSource sfxSource;
    bool failedSeqStarted;
    static bool playedIntro = false;
    private bool wasGrounded = true;
    private Coroutine introCoroutine;

    void Start()
    {
        // Always start the theme song
        VOManager.Instance.StartBackgroundMusic(themeSong);

        if (!SubwayGameManager.Instance.playedIntro)
        {
            if (introCoroutine != null)
                StopCoroutine(introCoroutine);
            introCoroutine = StartCoroutine(PlayClusterTruckIntro());
        }
    }

    void Update()
    {
        // Detect jump: player was grounded last frame but isn't now
        if (jumpSound != null && playerMovement != null && VOManager.Instance != null)
        {
            bool grounded = playerMovement.canJump;
            if (wasGrounded && !grounded)
            {
                VOManager.Instance.sfxSource.PlayOneShot(jumpSound, jumpVolume);
            }
            wasGrounded = grounded;
        }

        if (failedSeqStarted == false && healthScript.isDeathCoroutinePlaying == true && !SubwayGameManager.Instance.playedIntro)
        {
            if (introCoroutine != null)
            {
                StopCoroutine(introCoroutine);
                introCoroutine = null;
            }
            return;
        }
        if(failedSeqStarted == false && healthScript.isDeathCoroutinePlaying == true)
        {
            Debug.Log("player died, playing random Voiceline");
            if (introCoroutine != null)
            {
                StopCoroutine(introCoroutine);
                introCoroutine = null;
            }
            StartCoroutine(PlayDeathVoiceLine());
            failedSeqStarted = true;
        }
    }
    private IEnumerator PlayClusterTruckIntro()
    {
        Debug.Log("Intro playing");
        VOManager.Instance.PlayLine(introPt1);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Hear that?", 240f, 2500f),
        });

        yield return new WaitForSecondsRealtime(2);

        VOManager.Instance.PlaySoundFX(catMeow);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("*MEOW*", 440f, 2000f),
        });

        yield return new WaitForSecondsRealtime(2);

        VOManager.Instance.PlayLine(introPt2);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("That's right, we've got your beloved cat Daisy tied to a trolley heading down the cliff.", 240f, 8000f),
        });

        yield return new WaitForSecondsRealtime(8);

        SubwayGameManager.Instance.playedIntro = true;

        VOManager.Instance.PlayLine(introPt3);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Get to it, flip the lever, and Daisy lives another day.", 240f, 6000f),
            new VOManager.SubtitleLine("Good luck, and try not to fall off!", 6040f, 5000f),
        });

        yield return new WaitForSecondsRealtime(8);
    }

    private IEnumerator PlayDeathVoiceLine()
    {
        Debug.Log("player died, playing random Voiceline");
        // will randomly choose a death clip to play out of 3
        VOManager.Instance.StopAllCoroutines();
        int randomInt = Random.Range(1, 4);
        switch (randomInt)
        {
            case 1:
                VOManager.Instance.PlayLine(fail1);
                VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
                {
                    new VOManager.SubtitleLine("Daisy's waiting for you!", 140f, 4000f),

                });
                break;
            case 2:
                VOManager.Instance.PlayLine(fail2);
                VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
                {
                    new VOManager.SubtitleLine("Daisy's losing faith...", 140f, 4000f),

                });
                break;         
            case 3:
                VOManager.Instance.PlayLine(fail3);
                
                VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
                {
                    new VOManager.SubtitleLine("Come on.. you can't give up.", 140f, 4000f),

                });
                break;

            default:
                VOManager.Instance.PlayLine(fail1);
                VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
                {
                    new VOManager.SubtitleLine("Daisy's waiting for you!", 140f, 4000f),

                });
                
                break;
        }

        yield return new WaitForSeconds(10);
        failedSeqStarted = false;
    }

    IEnumerator DisableMovement()
    {
        playerMovement.enabled = false;
        yield return new WaitForSeconds(15);
        playerMovement.enabled = true;
    }
}
