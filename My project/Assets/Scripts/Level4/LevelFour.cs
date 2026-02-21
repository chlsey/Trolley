using System.Collections;
using UnityEngine;

public class LevelFourIntro : MonoBehaviour
{
    [Header("VO Clips")]
    public AudioClip levelFourIntroClip;  

    public TrolleyMovement trolleyMovement;
    public CurtainController curtainController;

    public Light trackOneLight;
    public Light trackTwoLight;
    public Light jimmyLight;
    public GameObject jimmy;
    void Start()
    {
        trolleyMovement.followSpline = false;
        StartCoroutine(PlayLevelFourIntroSeq());
    }

    private IEnumerator PlayLevelFourIntroSeq()
    {
        yield return CurtainController.Instance.CloseCurtains();
        VOManager.Instance.PlayLine(levelFourIntroClip);
        Debug.Log("lvl 4 intro played!");
        
        yield return new WaitForSeconds(13);
        // Show volunteer on screen

        // Show jimmy waving (at 15 seconds)

        // Gasp, lights turn red "Oh no where's the second track" (at 23 sec)

        // "That's right! your lever will push jimmy onto the tracks" (at 29 sec)

        // Show the Terms & Condition on screen (at 43 sec)
        trolleyMovement.followSpline = true;
        yield return curtainController.OpenCurtains();

        






    }
}
