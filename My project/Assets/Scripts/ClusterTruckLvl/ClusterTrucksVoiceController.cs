using System.Collections;
using UnityEngine;

public class ClusterTrucksVoiceController : MonoBehaviour
{
    public AudioClip introPt1;
    public AudioClip introPt2;
    public AudioClip introPt3;
    public AudioClip fail1;
    public AudioClip fail2;
    public AudioClip fail3;
    public AudioClip catMeow;
    public AudioClip themeSong;
    public Health healthScript;
    bool failedSeqStarted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        failedSeqStarted = false;
        StartCoroutine(PlayClusterTruckIntro());
    }

    void Update()
    {
        if(failedSeqStarted == false && healthScript.isDeathCoroutinePlaying == true)
        {
            Debug.Log("player died, playing random Voiceline");
            StopCoroutine(PlayClusterTruckIntro());
            StartCoroutine(PlayDeathVoiceLine());
            failedSeqStarted = true;
        }
    }
    private IEnumerator PlayClusterTruckIntro()
    {
        VOManager.Instance.PlayLine(introPt1);
        yield return new WaitForSeconds(2);
        VOManager.Instance.PlaySoundFX(catMeow);
        yield return new WaitForSeconds(2);
        VOManager.Instance.PlayLine(introPt2);
        yield return new WaitForSeconds(8);
        VOManager.Instance.PlayLine(introPt3);
        yield return new WaitForSeconds(8);
        VOManager.Instance.StartBackgroundMusic(themeSong);
    }

    private IEnumerator PlayDeathVoiceLine()
    {
        Debug.Log("player died, playing random Voiceline");
        // will randomly choose a death clip to play out of 3
        VOManager.Instance.StopAllCoroutines();
        int randomInt = Random.Range(1, 4);
        switch (randomInt)
        {
            case 1:
                VOManager.Instance.PlayLine(fail1);
                break;
            case 2:
                VOManager.Instance.PlayLine(fail2);
                break;         
            case 3:
                VOManager.Instance.PlayLine(fail3);
                break;

            default:
                VOManager.Instance.PlayLine(fail1);
                break;
        }

        yield return new WaitForSeconds(3);
        failedSeqStarted = false;
    }
}
