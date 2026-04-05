using UnityEngine;
using System.Collections.Generic;

public class RepeatingClipTrigger : MonoBehaviour
{
    [System.Serializable]
    public class TriggerEntry
    {
        public AudioClip clip;
        public List<VOTrigger.TriggerSubtitleLine> subtitleLines = new List<VOTrigger.TriggerSubtitleLine>();
    }

    // Entry 0 = first exit, Entry 1 = first re-enter, Entry 2 = second exit, etc.
    public List<TriggerEntry> entries = new List<TriggerEntry>();
    public float cooldown = 2f;

    private int triggerCount = 0;
    private bool playerInside = false;
    private float lastTriggerTime = -999f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null) return;
        if (playerInside) return;

        playerInside = true;

        if (Time.time - lastTriggerTime < cooldown) return;
        lastTriggerTime = Time.time;
        PlayEntry();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null) return;
        if (!playerInside) return;

        playerInside = false;

        if (Time.time - lastTriggerTime < cooldown) return;
        lastTriggerTime = Time.time;
        PlayEntry();
    }

    private void PlayEntry()
    {
        if (triggerCount >= entries.Count) return;

        var entry = entries[triggerCount];
        triggerCount++;

        if (entry.clip == null || VOManager.Instance == null) return;

        VOManager.Instance.PlayLine(entry.clip);

        if (entry.subtitleLines != null && entry.subtitleLines.Count > 0)
        {
            List<VOManager.SubtitleLine> lines = new List<VOManager.SubtitleLine>();
            for (int i = 0; i < entry.subtitleLines.Count; i++)
            {
                var line = entry.subtitleLines[i];
                if (!string.IsNullOrWhiteSpace(line.text))
                {
                    lines.Add(new VOManager.SubtitleLine(
                        line.text,
                        line.timeStartMilliseconds,
                        line.durationMilliseconds
                    ));
                }
            }
            if (lines.Count > 0)
                VOManager.Instance.ShowSubtitle(lines);
        }
    }
}
