using UnityEngine;

public class BoundaryClipTrigger : MonoBehaviour
{
    private bool triggered = false;
    public AudioClip triggeredClip;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        triggered = true;
        StartCoroutine(VOManager.Instance.PlayLineWait(triggeredClip));
    }
}
