using UnityEngine;
using System.Collections;

public class EndTrigger : MonoBehaviour
{
    public string outcomeId = "track_a";
    public string trolleyTag = "Trolley";

    private bool triggered = false;

    public void TriggerEnd()
    {
        if (triggered)
            return;

        if (LevelDirector.Active == null)
        {
            Debug.LogWarning("EndTrigger: LevelDirector.Active is null.");
            return;
        }

        triggered = true;
        Debug.Log("endtrigger: triggering next node");

        LevelDirector.Active.EndLevel(outcomeId);
    }


    // private void OnTriggerEnter(Collider other)
    // {
    //     if (triggered)
    //         return;

    //     if (!string.IsNullOrEmpty(trolleyTag) && !other.CompareTag(trolleyTag))
    //         return;

    //     if (LevelDirector.Active == null)
    //     {
    //         Debug.LogWarning("EndTrigger: LevelDirector.Active is null.");
    //         return;
    //     }

    //     triggered = true;
    //     StartCoroutine(EndSequence());
    // }

    // private IEnumerator EndSequence()
    // {
    //     yield return new WaitForSeconds(1);

        

    //     // still goes to next node after done
    //     LevelDirector.Active.EndLevel(outcomeId);
    // }
}
