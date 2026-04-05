using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CoasterEndingController : MonoBehaviour
{
    public AudioClip intro;
    public AudioClip nauseaClip;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CoasterCoroutine());
    }

    private IEnumerator CoasterCoroutine()
    {
        if (VOManager.Instance != null)
        {
            VOManager.Instance.PlayLine(intro);
            VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
            {
                new VOManager.SubtitleLine("Yes! The Trolley Getaway Experience!", 0f, 3800f),
                new VOManager.SubtitleLine("Hop on for our final journey together as host and contestant.", 4000f, 8300f),
            });

            yield return new WaitForSeconds(intro.length);

            // VOManager.Instance.PlayLine(fadeout);

        }
    }

    public void StartRidingCoroutine()
    {
        StartCoroutine(RidingCoroutine());
    }

    private IEnumerator RidingCoroutine()
    {
        yield return new WaitForSeconds(4f);

        if (VOManager.Instance != null)
        {
            VOManager.Instance.PlayLine(nauseaClip);
            VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
            {
                new VOManager.SubtitleLine("Is this not the best, most nauseating thing you’ve ever experienced?", 0f, 5200f),
                new VOManager.SubtitleLine("Haha! I sure won’t forget it anytime soon!", 5300f, 4000f),
            });

            yield return new WaitForSeconds(nauseaClip.length);

            // VOManager.Instance.PlayLine(fadeout);

        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
