using UnityEngine;
using System.Collections;

public class LevelTwoATrackB : MonoBehaviour
{
    public EndTrigger endTrigger;

    [Header("VO Clips")]
    public AudioClip endingClip;      
    public AudioClip levelThreeIntroClip;  

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

        VOManager.Instance.PlayLine(endingClip);

        yield return new WaitForSeconds(7);

        // StartCoroutine(CurtainController.Instance.CloseCurtains());

        yield return new WaitForSeconds(endingClip.length - 5);
        

        // VOManager.Instance.PlayLine(levelThreeIntroClip);

        // yield return new WaitForSeconds(3);

        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
