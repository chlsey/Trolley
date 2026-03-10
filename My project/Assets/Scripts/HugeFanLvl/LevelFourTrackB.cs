using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class LevelFourChoiceB : MonoBehaviour
{
    public EndTrigger endTrigger;
    public Lever lever;
    public AudioClip levelFourEndingTwo;
    public AudioClip platformerTransitionClip;

    private bool hasTriggered = false;
    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        hasTriggered = true;
        
        if(lever.leverFlipped == false)
        {
            StopAllCoroutines();
            Debug.Log("lvl 4 track b ending!");
            StartCoroutine(EndSequence());
        }
    }

    private IEnumerator EndSequence()
    {
        VOManager.Instance.StopBackgroundMusic();
        VOManager.Instance.PlayLine(levelFourEndingTwo);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Getting comfortable standing around, I see?", 240f, 3500f),

        });

        // StartCoroutine(CurtainController.Instance.CloseCurtains());
        yield return new WaitForSeconds(levelFourEndingTwo.length); 

        VOManager.Instance.PlayLine(platformerTransitionClip);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Anyhow, this is getting boring.", 140f, 3800f),

            new VOManager.SubtitleLine("How about a change in scenery?", 4000f, 3000f),

            new VOManager.SubtitleLine("I must warn you,", 7000f, 1500f),

            new VOManager.SubtitleLine("The lever won't be so close this time...", 8500f, 2500f),

            new VOManager.SubtitleLine("and the trolley won't be so far away.", 11000f, 3000f),
        });

        yield return new WaitForSeconds(platformerTransitionClip.length);

        // scene change to platformer level

        FindObjectOfType<NextScene>().TriggerSceneChange();

        endTrigger.TriggerEnd(); 

    }
}
