using UnityEngine;
using System.Collections;

public class LevelOneTrackB : MonoBehaviour
{
    public EndTrigger endTrigger;

    [Header("VO Clips")]
    public AudioClip endingClip;      
    public AudioClip levelTwoBIntroClip;  
    private bool hasTriggered = false;

    public AudioClip fasterLevelMusic;  

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        hasTriggered = true;
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        VOManager.Instance.PlayLine(endingClip);

        StartCoroutine(CurtainController.Instance.CloseCurtains());

        yield return new WaitForSeconds(endingClip.length);

        VOManager.Instance.StartBackgroundMusic(fasterLevelMusic);

        VOManager.Instance.PlayLine(levelTwoBIntroClip);

        yield return new WaitForSeconds(13);

        StartCoroutine(CurtainController.Instance.OpenCurtains());

        yield return new WaitForSeconds(1);

        Debug.Log("track a ending complete");

        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
