using System.Collections;
using UnityEngine;

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

        // StartCoroutine(CurtainController.Instance.CloseCurtains());
        yield return new WaitForSeconds(levelFourEndingTwo.length); 

        VOManager.Instance.PlayLine(platformerTransitionClip);
        yield return new WaitForSeconds(platformerTransitionClip.length);

        // scene change to platformer level

        FindObjectOfType<NextScene>().TriggerSceneChange();

        endTrigger.TriggerEnd(); 

    }
}
