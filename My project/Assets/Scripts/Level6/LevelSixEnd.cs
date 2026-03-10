using UnityEngine;
using System.Collections;

public class LevelSevenEnd : MonoBehaviour
{
    public EndTrigger endTrigger;

    // public AudioClip legalClip;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        hasTriggered = true;

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("RAPID: 1 v 91382028392983982398");

        // VOManager.Instance.PlayLine(legalClip);

        endTrigger.TriggerEnd();
    }
}
