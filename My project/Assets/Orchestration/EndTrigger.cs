using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public string outcomeId = "track_a";
    public string trolleyTag = "Trolley";

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(trolleyTag) && !other.CompareTag(trolleyTag))
        {
            return;
        }

        if (LevelDirector.Active == null)
        {
            Debug.LogWarning("EndTrigger: LevelDirector.Active is null.");
            return;
        }

        LevelDirector.Active.EndLevel(outcomeId);
    }
}
