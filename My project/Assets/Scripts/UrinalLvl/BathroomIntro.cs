using UnityEngine;
using System.Collections.Generic;

// Attach to a GameObject in the SeperateBathroom scene.
// Plays the Level 12 intro audio and subtitles when the scene starts.
public class BathroomIntro : MonoBehaviour
{
    public AudioClip bgMusic;
    public AudioClip introVoiceLine;

    void Start()
    {
        VOManager.Instance.StopBackgroundMusic();
        VOManager.Instance.StartBackgroundMusic(bgMusic);

        if (introVoiceLine != null)
            VOManager.Instance.PlayLine(introVoiceLine);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Well, it's about time we give you a little bathroom break.", 140f, 4800f),
            new VOManager.SubtitleLine("You've been working so hard for us.", 5200f, 5000f),
            new VOManager.SubtitleLine("Welp, I'll give you some space...", 10000f, 4000f),
        });
    }
}
