using UnityEngine;
using System.Collections.Generic;

public class BoundaryClipTrigger : MonoBehaviour
{
    private bool triggered = false;
    public AudioClip triggeredClip;

    public List<VOTrigger.TriggerSubtitleLine> subtitleLines = new List<VOTrigger.TriggerSubtitleLine>();

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        triggered = true;
        StartCoroutine(VOManager.Instance.PlayLineWait(triggeredClip));

        // Show subtitles — calling ShowSubtitle automatically clears any previous subtitle
        if (subtitleLines == null || subtitleLines.Count == 0) return;

        List<VOManager.SubtitleLine> lines = new List<VOManager.SubtitleLine>();
        for (int i = 0; i < subtitleLines.Count; i++)
        {
            var line = subtitleLines[i];
            if (string.IsNullOrWhiteSpace(line.text)) continue;

            lines.Add(new VOManager.SubtitleLine(
                line.text,
                line.timeStartMilliseconds,
                line.durationMilliseconds
            ));
        }

        if (lines.Count > 0)
            VOManager.Instance.ShowSubtitle(lines);
    }
}
