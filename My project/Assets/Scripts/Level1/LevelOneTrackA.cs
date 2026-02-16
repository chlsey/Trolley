using UnityEngine;
using System.Collections;

public class LevelOneTrackA : MonoBehaviour
{
    public EndTrigger endTrigger;

    [Header("VO Clips")]
    public AudioClip endingClip;      
    public AudioClip levelTwoAIntroClip;  

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        VOManager.Instance.PlayLine(endingClip);

        yield return new WaitForSeconds(endingClip.length);

        VOManager.Instance.PlayLine(levelTwoAIntroClip);

        yield return new WaitForSeconds(5f);

        // curtains open back up

        Debug.Log("track a ending complete");

        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
