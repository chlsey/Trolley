using System.Collections;
using UnityEngine;

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
        StartCoroutine(PlayLevelFourIntroSeq());
        lever.enabled = false;
        
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

        yield return new WaitForSeconds(levelFourEndingOne.length); 


        VOManager.Instance.PlayLine(platformerTransitionClip);

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
