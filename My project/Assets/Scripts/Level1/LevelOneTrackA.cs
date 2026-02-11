using UnityEngine;
using System.Collections;

public class LevelOneTrackA : MonoBehaviour
{
    public EndTrigger endTrigger;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(14f);

        Debug.Log("track a ending complete");


        // call endtrigger
        endTrigger.TriggerEnd();
    }
}
