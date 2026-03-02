using UnityEngine;
using System.Collections;

public class LevelOneTrackA : MonoBehaviour
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

        yield return new WaitForSeconds(endingClip.length - 1);

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("Level 2: 10 v 1");

        endTrigger.TriggerEnd();
    }
}
