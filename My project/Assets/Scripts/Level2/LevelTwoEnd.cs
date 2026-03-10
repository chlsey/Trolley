using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelTwoEnd : MonoBehaviour
{
    public EndTrigger endTrigger;

    [Header("VO Clips")]
    public AudioClip endingClip;  

    public AudioClip musicClip;  

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
            new VOManager.SubtitleLine("Fascinating!", 240f, 1400f),

            new VOManager.SubtitleLine("Let’s speed things up some more shall we?", 1840f, 3500f),

            new VOManager.SubtitleLine("Ladies and gentlemen, let’s fire up the rapid rounds?", 5440f, 5000f),

            new VOManager.SubtitleLine("Try not to kill that many people will ya?", 11240f, 2500f),


        });

        yield return new WaitForSeconds(endingClip.length);

        VOManager.Instance.StartBackgroundMusic(musicClip);

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("RAPID: 3 v 2");

        endTrigger.TriggerEnd();
    }
}
