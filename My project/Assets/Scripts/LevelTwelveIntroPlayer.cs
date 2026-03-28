using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelTwelveIntroPlayer : MonoBehaviour
{

    public Urinal urinal1;
    public Urinal urinal2;
    public AudioClip bgMusicUrinal;

    void Start()
    {
        VOManager.Instance.StopBackgroundMusic();
        VOManager.Instance.StartBackgroundMusic(bgMusicUrinal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
