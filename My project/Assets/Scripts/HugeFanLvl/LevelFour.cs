using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class LevelFourIntro : MonoBehaviour
{
    [Header("VO Clips")]
    public AudioClip levelFourIntroClip;  
    public AudioClip levelFourEndingOne;
    public AudioClip lightsOn;
    public AudioClip gasp;
    public AudioClip clockSFX;
    public AudioClip platformerTransitionClip;

    public AudioClip lvlMusic;

    public EndTrigger endTrigger;
    public TrolleyMovement trolleyMovement;
    // public CurtainController curtainController;
    public Lever lever;
    public ClockBehavior clock;

    // public Light trackOneLight;
    // public Light trackTwoLight;
    public Light jimmyLight;
    public GameObject jimmy;
    
    private bool endingStarted;

    void Start()
    {
        jimmyLight.intensity = 0f;
        endingStarted = false;
        clock.rotate = false;
        trolleyMovement.followSpline = false;
        lever.enabled = false;
        StartCoroutine(PlayLevelFourIntroSeq());
        
    }

    void Update()
    {
        if (lever.leverFlipped == true && endingStarted == false)
        {
            endingStarted = true; 
            trolleyMovement.followSpline = true;
            Debug.Log("lvl4 ending 1");
            // FindObjectOfType<LevelNameDisplay>().ShowLevelName("Make the trolley do a LOOP!");
            StopCoroutine(PlayLevelFourIntroSeq());
            StartCoroutine(PlayLevelFourEndingOne());
              
        }
    }

    private IEnumerator PlayLevelFourEndingOne()
    {
        VOManager.Instance.StopBackgroundMusic();
        VOManager.Instance.PlayAudience(gasp);
        VOManager.Instance.PlayLine(levelFourEndingOne);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("You’re very comfortable at the controls, aren’t you!", 240f, 3500f),

        });

        yield return new WaitForSeconds(levelFourEndingOne.length); 


        VOManager.Instance.PlayLine(platformerTransitionClip);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Anyhow, this is getting boring.", 140f, 3800f),

            new VOManager.SubtitleLine("How about a change in scenery?", 4000f, 3000f),

            new VOManager.SubtitleLine("I must warn you,", 7000f, 1500f),

            new VOManager.SubtitleLine("The lever won't be so close this time...", 8500f, 2500f),

            new VOManager.SubtitleLine("and the trolley won't be so far away.", 11000f, 3000f),
        });

        yield return new WaitForSeconds(platformerTransitionClip.length);

        // scene change to platformer level
        FindObjectOfType<NextScene>().TriggerSceneChange();
        
        endTrigger.TriggerEnd();
        
    }
    private IEnumerator PlayLevelFourIntroSeq()
    {
        FindObjectOfType<LevelNameDisplay>().ShowLevelName("SPECIAL GUEST");

        VOManager.Instance.PlayLine(levelFourIntroClip);
        LightManager.Instance.TurnOffTrackB();
        Debug.Log("lvl 4 intro played!");

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine(" I know, You’re getting tired of the same setup.", 240f, 3500f),

            new VOManager.SubtitleLine("Don’t worry, I know just how to spice things up!", 4000f, 3800f),

            new VOManager.SubtitleLine("We’ve got a lucky fan from the audience who will be with us.", 7640f, 3200f),

            new VOManager.SubtitleLine("Can you give us a little wave Jimmy!", 10800f, 2500f),

            new VOManager.SubtitleLine("*Gasp* Where is the second track?", 15040f, 2300f),

            new VOManager.SubtitleLine("That's right, ", 18540f, 1500f),

            new VOManager.SubtitleLine("this time, your lever will push Jimmy onto the tracks before the trolley gets to the five people to save them.", 19540f, 8000f),

            new VOManager.SubtitleLine("Jimmy’s fine with it!", 27040f, 2000f),

            new VOManager.SubtitleLine("he signed our terms and conditions which SPECIFICALLY said:", 29040f, 3900f),

            new VOManager.SubtitleLine("an audience member at any moment can be asked to participate in the trolley problem....", 33840f, 3000f),

            new VOManager.SubtitleLine("...this show is not responsible for any harm that may be caused to them at any point....", 37040f, 3000f),
        

        });

        yield return new WaitForSeconds(3);

        VOManager.Instance.StopBackgroundMusic();

        yield return new WaitForSeconds(3);

        VOManager.Instance.StartBackgroundMusic(lvlMusic);
        
        yield return new WaitForSeconds(7);
        // // Show volunteer on screen
        jimmyLight.intensity = 2000f;
        VOManager.Instance.PlaySoundFX(lightsOn);

        // Show jimmy waving (at 15 seconds)
        yield return new WaitForSeconds(2);

        // StartCoroutine(CurtainController.Instance.OpenCurtains());

        // Gasp, lights turn red "Oh no where's the second track" (at 23 sec)
        yield return new WaitForSeconds(5);

        // "That's right! your lever will push jimmy onto the tracks" (at 29 sec)
        yield return new WaitForSeconds(6);

        // Show the Terms & Condition on screen (at 43 sec)
        yield return new WaitForSeconds(9);
        
        trolleyMovement.followSpline = true;
        clock.rotate = true;
        VOManager.Instance.PlaySoundFX(clockSFX);
        lever.enabled = true;
    }
}
