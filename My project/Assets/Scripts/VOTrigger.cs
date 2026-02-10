using UnityEngine;

public class VOTrigger : MonoBehaviour
{
    public string trolleyTag = "Trolley";

    public AudioClip voiceoverClip;

    private bool triggered = false;


    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(trolleyTag)) return;

        triggered = true;

        if (voiceoverClip != null)
        {
            VOManager.Instance.Play(voiceoverClip);
        }
    }
}

