using UnityEngine;
using System.Collections;

public class LevelOneTrackA : MonoBehaviour
{
    public EndTrigger endTrigger;

    [Header("VO Clips")]
    public AudioClip endingClip;      
    public AudioClip levelTwoAIntroClip;  

    public AudioClip bgMusic;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        hasTriggered = true;
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        VOManager.Instance.StopBackgroundMusic();

        yield return CurtainController.Instance.CloseCurtains();

        VOManager.Instance.PlayLine(endingClip);

        yield return new WaitForSeconds(endingClip.length);

        VOManager.Instance.PlayLine(levelTwoAIntroClip);

        yield return new WaitForSeconds(5f);

        VOManager.Instance.StartBackgroundMusic(bgMusic);

        yield return CurtainController.Instance.OpenCurtains();

        // curtains open back up

        Debug.Log("track a ending complete");

        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
