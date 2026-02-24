using System.Collections;
using UnityEngine;

public class LevelFourIntro : MonoBehaviour
{
    [Header("VO Clips")]
    public AudioClip levelFourIntroClip;  
    public AudioClip levelFourEndingOne;
    public AudioClip lightsOn;

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
    }

    void Update()
    {
        if (lever.leverFlipped == true && endingStarted == false)
        {
            trolleyMovement.followSpline = true;
            Debug.Log("lvl4 ending 1");
            StopCoroutine(PlayLevelFourIntroSeq());
            StartCoroutine(PlayLevelFourEndingOne());
            endingStarted = true;   
        }
    }

    private IEnumerator PlayLevelFourEndingOne()
    {
        VOManager.Instance.PlayLine(levelFourEndingOne);
        yield return new WaitForSeconds(6);
        StartCoroutine(CurtainController.Instance.CloseCurtains());

        yield return new WaitForSeconds(8);
        endTrigger.TriggerEnd();
        
    }
    private IEnumerator PlayLevelFourIntroSeq()
    {
        VOManager.Instance.PlayLine(levelFourIntroClip);
        Debug.Log("lvl 4 intro played!");
        
        yield return new WaitForSeconds(10);
        // // Show volunteer on screen
        jimmyLight.intensity = 30000f;
        VOManager.Instance.PlaySoundFX(lightsOn);

        // Show jimmy waving (at 15 seconds)
        yield return new WaitForSeconds(3);
        // jimmyLight.intensity = 30000f;
        // VOManager.Instance.PlaySoundFX(lightsOn);

        StartCoroutine(CurtainController.Instance.OpenCurtains());

        yield return new WaitForSeconds(2);

        // Gasp, lights turn red "Oh no where's the second track" (at 23 sec)
        yield return new WaitForSeconds(8);

        // "That's right! your lever will push jimmy onto the tracks" (at 29 sec)
        yield return new WaitForSeconds(6);

        // Show the Terms & Condition on screen (at 43 sec)
        yield return new WaitForSeconds(9);
        trolleyMovement.followSpline = true;
        clock.rotate = true;
    }
}
