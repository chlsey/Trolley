using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelTwelveIntroPlayer : MonoBehaviour
{

    public Urinal urinal1;
    public Urinal urinal2;
    public AudioClip bgMusicUrinal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // urinal1.enabled = false;
        // urinal2.enabled = false;
        VOManager.Instance.StopBackgroundMusic();
        VOManager.Instance.StartBackgroundMusic(bgMusicUrinal);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Well, it’s about time we give you a little bathroom break.", 140f, 4800f),

            new VOManager.SubtitleLine("You’ve been working so hard for us.", 5200f, 5000f),

            new VOManager.SubtitleLine("Welp, I’ll give you some space...", 10000f, 4000f),

        });

        StartCoroutine(WaitTenSeconds());




    }

    public IEnumerator WaitTenSeconds()
    {
        yield return new WaitForSeconds(10);
        // urinal1.enabled = true;
        // urinal2.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
