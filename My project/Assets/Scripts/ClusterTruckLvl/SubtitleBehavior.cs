using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleBehavior: PlayableBehaviour
{
    public string subtitleText;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI text = playerData as TextMeshProUGUI;
        text.text = subtitleText;
    }
}
