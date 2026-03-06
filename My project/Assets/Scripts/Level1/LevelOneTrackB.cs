using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("The classic utilitarian move", 100f, 2500f),

            new VOManager.SubtitleLine("Alright. Let's tighten the margins", 2500f, 4000f),

        });

        yield return new WaitForSeconds(endingClip.length - 4);

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("Level 2: 5 v 4");

        endTrigger.TriggerEnd();
    }
}
