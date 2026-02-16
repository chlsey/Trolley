using UnityEngine;
using System.Collections;

public class LevelTwoATrackA : MonoBehaviour
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

        Debug.Log("track a ending complete");


        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
