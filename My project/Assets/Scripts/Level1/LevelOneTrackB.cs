using UnityEngine;
using System.Collections;

public class LevelOneTrackB : MonoBehaviour
{
    public EndTrigger endTrigger;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(13f);

        Debug.Log("track B ending complete");

        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
