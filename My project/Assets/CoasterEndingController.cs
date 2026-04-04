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
                // new VOManager.SubtitleLine("Apologies for the sudden change in... dimentionality", 0f, 5000f)
            });

            yield return new WaitForSeconds(intro.length);

            // VOManager.Instance.PlayLine(fadeout);

        }
    }

    private IEnumerator RidingCoroutine()
    {
        if (VOManager.Instance != null)
        {
            VOManager.Instance.PlayLine(nauseaClip);
            VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
            {
                // new VOManager.SubtitleLine("Apologies for the sudden change in... dimentionality", 0f, 5000f)
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
