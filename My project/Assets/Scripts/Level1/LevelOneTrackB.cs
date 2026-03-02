using UnityEngine;
using System.Collections;

public class LevelOneTrackB : MonoBehaviour
{
    public EndTrigger endTrigger;

    [Header("VO Clips")]
    public AudioClip endingClip;      

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

        yield return new WaitForSeconds(endingClip.length - 4);

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("Level 2: 5 v 4");

        endTrigger.TriggerEnd();
    }
}
