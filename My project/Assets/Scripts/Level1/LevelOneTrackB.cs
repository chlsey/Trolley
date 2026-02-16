using UnityEngine;
using System.Collections;

public class LevelOneTrackB : MonoBehaviour
{
    public EndTrigger endTrigger;

    [Header("VO Clips")]
    public AudioClip endingClip;      
    public AudioClip levelTwoBIntroClip;  

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        VOManager.Instance.PlayLine(endingClip);

        yield return new WaitForSeconds(endingClip.length);

        VOManager.Instance.PlayLine(levelTwoBIntroClip);

        yield return new WaitForSeconds(levelTwoBIntroClip.length);

        Debug.Log("track a ending complete");

        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
