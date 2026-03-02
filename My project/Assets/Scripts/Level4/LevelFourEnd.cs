using UnityEngine;
using System.Collections;

public class LevelFourEnd : MonoBehaviour
{
    public EndTrigger endTrigger;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        hasTriggered = true;

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("10 worms v 9 worms");

        endTrigger.TriggerEnd();
    }
}
