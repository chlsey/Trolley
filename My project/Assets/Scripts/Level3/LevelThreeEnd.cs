using UnityEngine;
using System.Collections;

public class LevelThreeEnd : MonoBehaviour
{
    public EndTrigger endTrigger;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        hasTriggered = true;

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("Solving world hunger");

        endTrigger.TriggerEnd();
    }
}
