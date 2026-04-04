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
                // new VOManager.SubtitleLine("Apologies for the sudden change in... dimentionality", 0f, 5000f)
            });

            yield return new WaitForSeconds(end.length);

            VOManager.Instance.PlayLine(fadeout);

        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
