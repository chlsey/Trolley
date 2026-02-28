using System.Collections;
using UnityEngine;

public class LevelFourChoiceB : MonoBehaviour
{
    public EndTrigger endTrigger;
    public Lever lever;
    public AudioClip levelFourEndingTwo;
    void OnTriggerEnter(Collider other)
    {
        if(lever.leverFlipped == false)
        {
            StopAllCoroutines();
            Debug.Log("lvl 4 track b ending!");
            StartCoroutine(EndSequence());
        }
    }

    private IEnumerator EndSequence()
    {
        VOManager.Instance.PlayLine(levelFourEndingTwo);

        StartCoroutine(CurtainController.Instance.CloseCurtains());

        yield return new WaitForSeconds(15f);  
        endTrigger.TriggerEnd(); 

    }
}
