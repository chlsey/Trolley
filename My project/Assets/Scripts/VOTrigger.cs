using UnityEngine;
using System.Collections.Generic;

public class VOTrigger : MonoBehaviour
{
    // INSTRUCTIONS FOR SUBTITLES:
    // Find VOTrigger component in inspector
    // Add a subtitle line for each subtitle line you want to add.
    // For each subtitle, 
    // add the text, 
    // add the time at which the subtitle starts after audio is triggered, 
    // and add the time for which it lasts.
    [System.Serializable]
    public struct TriggerSubtitleLine
    {
        [TextArea(2, 5)]
        public string text;

        [Tooltip("Time in milliseconds from trigger start. Must be >= 0.")]
        public float timeStartMilliseconds;

        [Tooltip("Duration in milliseconds. Must be > 0.")]
        public float durationMilliseconds;
    }

    public string trolleyTag = "Trolley";
    
    public AudioClip voiceoverClip;
    
    [Header("Subtitle")]
    public bool showSubtitle = true;

    [Tooltip("Subtitle lines to show for this trigger. Each line uses its own start time (x) and duration (y), in milliseconds.")]
    public List<TriggerSubtitleLine> subtitleLines = new List<TriggerSubtitleLine>();

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(trolleyTag)) return;

        triggered = true;

        var voManager = VOManager.Instance;

        if (voiceoverClip != null)
        {
            if (voManager != null)
                voManager.Play(voiceoverClip);
            else
                Debug.LogWarning("VOTrigger: VOManager.Instance is null, cannot play voice clip.", this);
        }

        if (showSubtitle)
        {
            if (voManager == null)
            {
                Debug.LogWarning("VOTrigger: VOManager.Instance is null, cannot show subtitle.", this);
                return;
            }

            if (subtitleLines == null || subtitleLines.Count == 0)
            {
                Debug.LogWarning("VOTrigger: showSubtitle is enabled but no subtitle lines are configured.", this);
                return;
            }

            List<VOManager.SubtitleLine> queuedSubtitleLines = new List<VOManager.SubtitleLine>();

            for (int index = 0; index < subtitleLines.Count; index++)
            {
                TriggerSubtitleLine line = subtitleLines[index];

                if (string.IsNullOrWhiteSpace(line.text))
                    continue;

                if (line.timeStartMilliseconds < 0f)
                {
                    Debug.LogWarning($"VOTrigger: subtitle line {index} must have timeStartMilliseconds >= 0.", this);
                    return;
                }

                if (line.durationMilliseconds <= 0f)
                {
                    Debug.LogWarning($"VOTrigger: subtitle line {index} must have durationMilliseconds > 0.", this);
                    return;
                }

                queuedSubtitleLines.Add(new VOManager.SubtitleLine(
                    line.text,
                    line.timeStartMilliseconds,
                    line.durationMilliseconds
                ));
            }

            if (queuedSubtitleLines.Count == 0)
            {
                Debug.LogWarning("VOTrigger: all configured subtitle lines are empty.", this);
                return;
            }

            voManager.ShowSubtitle(queuedSubtitleLines);
        }
    }
}
