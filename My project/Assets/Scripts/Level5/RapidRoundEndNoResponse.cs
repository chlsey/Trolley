using UnityEngine;
using System.Collections;

public class RapidRoundEndNoResponse : MonoBehaviour
{
    public EndTrigger endTrigger;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        hasTriggered = true;
        endTrigger.TriggerEnd();
    }
}
