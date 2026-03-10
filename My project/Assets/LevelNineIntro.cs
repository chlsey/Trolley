using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelNineIntro : MonoBehaviour
{
    public AudioClip levelNineIntroClip;  
    public AudioClip clockSFX;

    public TrolleyMovement trolleyMovement;
    // public CurtainController curtainController;
    public Lever lever;
    public ClockBehavior clock;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clock.rotate = false;
        trolleyMovement.followSpline = false;
        lever.enabled = false;

        StartCoroutine(PlayLevelNineIntro());


    }

    private IEnumerator PlayLevelNineIntro()
    {
        VOManager.Instance.PlayLine(levelNineIntroClip);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("And a grand finale for our rapid round!", 240f, 3000f),

        });
        
        yield return new WaitForSeconds(levelNineIntroClip.length);

        trolleyMovement.followSpline = true;
        clock.rotate = true;
        VOManager.Instance.PlaySoundFX(clockSFX);
        lever.enabled = true;

    }

}
