using UnityEngine;
using System.Collections.Generic;

public class CrossyOrchestrator : MonoBehaviour
{
    public AudioClip intro;
    public AudioSource audioSource;

    void Start()
    {
        if (VOManager.Instance != null)
        {
            VOManager.Instance.PlayLine(intro);
            VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
            {
                new VOManager.SubtitleLine("Apologies for the sudden change in... dimentionality", 0f, 5000f),
                new VOManager.SubtitleLine("Welcome! To King Street!", 5200f, 8000f),
                new VOManager.SubtitleLine("Where rules are made up, and traffic laws are... mostly optional", 8300f, 12500f),
                new VOManager.SubtitleLine("All you need to do is reach the lever across the street to win!", 13000f, 18600f),
                new VOManager.SubtitleLine("Simple!", 18800f, 19500f),
                new VOManager.SubtitleLine("Let's give a round of applause to our contestant!", 20000f, 25000f),
                new VOManager.SubtitleLine("", 25000f, 25000f)
            });
        }
        else
        {
            audioSource.clip = intro;
            audioSource.Play();
        }
    }

    void Update()
    {

    }
}
