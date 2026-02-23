using UnityEngine;
using System.Collections;

public class LevelOneTrackB : MonoBehaviour
{
    public EndTrigger endTrigger;

    [Header("VO Clips")]
    public AudioClip endingClip;      
    public AudioClip levelTwoBIntroClip;  
    private bool hasTriggered = false;

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

        VOManager.Instance.PlayLine(levelTwoBIntroClip);

        yield return new WaitForSeconds(8);

        StartCoroutine(CurtainController.Instance.OpenCurtains());

        yield return new WaitForSeconds(7);

        Debug.Log("track a ending complete");

        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
