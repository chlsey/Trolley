using UnityEngine;
using System.Collections;

public class LevelTwoATrackB : MonoBehaviour
{
    public EndTrigger endTrigger;

    [Header("VO Clips")]
    public AudioClip endingClip;      
    // public AudioClip levelThreeIntroClip;  

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        VOManager.Instance.PlayLine(endingClip);

        yield return new WaitForSeconds(endingClip.length);

        Debug.Log("track b ending complete");

        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
