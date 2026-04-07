using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TrackEndingController : MonoBehaviour
{
    public AudioClip end;
    public AudioClip fadeout;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TrackEndingCoroutine());
    }

    private IEnumerator TrackEndingCoroutine()
    {
        if (VOManager.Instance != null)
        {
            VOManager.Instance.PlayLine(end);
            VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
            {
                new VOManager.SubtitleLine("Well, I sensed this choice coming...", 0f, 2400f),
                new VOManager.SubtitleLine("But really, why, contestant?", 3500f, 3200f),
                new VOManager.SubtitleLine("Has our time together been so unbearable that you choose to be run over?", 7200f, 5100f),
                new VOManager.SubtitleLine("I even let you have a bathroom break.", 12800f, 2600f),
                new VOManager.SubtitleLine("Or are you sacrificing yourself for five strangers?", 16400f, 4100f),
                new VOManager.SubtitleLine("I hate to tell you this, but they're not even real.", 20900f, 3100f),
                new VOManager.SubtitleLine("It's all a game.", 24300f, 1200f),
                new VOManager.SubtitleLine("You're the only real one here.", 26600f, 2700f),
            });

            yield return new WaitForSeconds(end.length);

            VOManager.Instance.PlayLine(fadeout);
            VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
            {
                new VOManager.SubtitleLine("Just you and me.", 0f, 2200f),
            });

        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
