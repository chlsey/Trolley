using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("WOW!", 640f, 1000f),

            new VOManager.SubtitleLine("Ok...", 1640f, 1000f),

            new VOManager.SubtitleLine("What about ten people?", 2640f, 1500f),

        });

        yield return new WaitForSeconds(endingClip.length - 1);

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("Level 2: 10 v 1");

        endTrigger.TriggerEnd();
    }
}
