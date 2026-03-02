using UnityEngine;
using System.Collections;

public class LevelEightEnd : MonoBehaviour
{
    public EndTrigger endTrigger;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        hasTriggered = true;

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("JUST PULL IT");

        endTrigger.TriggerEnd();
    }
}
