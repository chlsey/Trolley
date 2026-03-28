using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

// This is created so we can control subtitles using the timeline
[TrackBindingType(typeof(TextMeshProUGUI))]
[TrackClipType(typeof(SubtitleClip))]
public class SubtitleTrack : TrackAsset
{
}
