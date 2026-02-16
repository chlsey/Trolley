using UnityEngine;
using System.Collections;

public class LevelTwoBTrackA : MonoBehaviour
{
    public EndTrigger endTrigger;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(13f);

        Debug.Log("track b ending complete");

        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
