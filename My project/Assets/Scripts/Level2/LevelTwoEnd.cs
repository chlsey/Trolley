using UnityEngine;
using System.Collections;

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

        yield return new WaitForSeconds(endingClip.length);

        VOManager.Instance.StartBackgroundMusic(musicClip);

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("RAPID: 3 v 2");

        endTrigger.TriggerEnd();
    }
}
