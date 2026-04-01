using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class IntroOrchestrator : MonoBehaviour
{
    [Header("Curtains")]
    public Transform leftCurtain;
    public Transform rightCurtain;
    public float curtainOpenDistance = 10f;
    public float curtainOpenDuration = 1f;


    [Header("Curtain Lights")]
    public Light curtainLight1;     
    public Light curtainLight2;    
    public Light curtainLight3;  

    public float gyrateSpeed = 2f;
    public float gyrateAngle = 30f;
    public float lightsExitDuration = 2f;

    private Coroutine lightRoutine;

    [Header("Extra Lights")]
    public Light leverLight;
    public Light trolleyLight;
    public Light oneLight;
    public Light fiveLight;


    [Header("VO Clips")]
    public AudioClip introClipPart1;      
    public AudioClip introClipPart2;  
    public AudioClip trolleyClip;  


    [Header("Audience")]
    public AudioClip applauseClip;     

    [Header("Extra SFX")]
    public AudioClip lightOnClip;    
    public AudioClip introBGM; 
    public AudioClip clock;
    public bool deathCoroutinePlaying;

    public LeverNoRating lever;
    void Start()
    {   
        leverLight.intensity = 0;
        deathCoroutinePlaying = false;
        StartCoroutine(PlayIntroSequence());
    }

    // Subtitles EXAMPLE for intro orchestration
        // Add a subtitles line for each, where the parameters are:
        // (Subtitle Sentence, start time in milliseconds after audio is triggered, duration in milliseconds)
        // VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        // {
        //     new VOManager.SubtitleLine("Welcome… welcome… welcome…", 1514f, 2421f),
        // });
    private IEnumerator PlayIntroSequence()
    {

        LightManager.Instance.TurnOffRGBLights();
        VOManager.Instance.ShowPrompt(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Use <sprite index=8> to move,  <sprite index=9> to look", 0, 3000),
            
        });
        
        yield return new WaitForSeconds(3f);

        VOManager.Instance.PlayLine(introClipPart1);
        // Production assistant lines
        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Alright, let's do a final check real quick.", 1000f, 2500f),
            new VOManager.SubtitleLine("Is that lever set up?", 3000f, 5000f),
        });

        Debug.Log("PlayIntroSequence started");

        // 0:06 lever check
        VOManager.Instance.ShowPrompt(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Press <sprite index=1> to flip the lever", 4500, 6500)
            
        });
        yield return new WaitUntil(() => lever.leverFlipped == true);
        Debug.Log("lever flipped, continuing");


        VOManager.Instance.PlayLine(introClipPart2);
            VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Sweettt", 0f, 1500f),
            new VOManager.SubtitleLine("Don’t forget buddy, you can only flip it once per round yeah?", 1500f, 5000f),
            new VOManager.SubtitleLine("Alright, we're gonna be on air in", 5000, 6500f),
            new VOManager.SubtitleLine("3", 6500f, 7000f),
            new VOManager.SubtitleLine("2", 7000f, 7500f),
            new VOManager.SubtitleLine("1", 7500f, 8000f),
        });
        // 0:09 drum rolls spinning lights on
        yield return new WaitForSeconds(9);
        VOManager.Instance.PlaySoundFX(lightOnClip);
        CurtainLightsOn();
        lightRoutine = StartCoroutine(GyrateCurtainLights());
        LightsOff();

        // narrator lines
        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Welcome… welcome… welcome…", 3280f, 3150f),

            new VOManager.SubtitleLine("To the Trolley Show!", 6200f, 3450f),

            new VOManager.SubtitleLine("I'll be your host for the show, and tonight", 9900f, 4421f),

        });


        // 0:17 The trolley showwwww
        yield return new WaitForSeconds(7.6f);
        // curtain opens
        yield return CurtainController.Instance.OpenIntroCurtains();
        VOManager.Instance.PlayAudience(applauseClip);
        CurtainLightsOff();
        VOManager.Instance.PlaySoundFX(lightOnClip);
        LightManager.Instance.TurnOnRGBLights();
        LightManager.Instance.TurnOnArchLights();
        LightManager.Instance.TurnOnWalkwayLights();
        // theme starts playing
        VOManager.Instance.StartBackgroundMusic(introBGM);
            

        // 0:22 a trolley is speeding down the tracks
        yield return new WaitForSeconds(4.5f);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("A trolley is speeding down the tracks.", 0f, 2500f),

            new VOManager.SubtitleLine("Straight ahead… are five people.", 2500f, 4100f),

            new VOManager.SubtitleLine("But! You have a lever.", 6700f, 2800f),

            new VOManager.SubtitleLine("Pull it, and the trolley switches tracks, where only one would be the victim.", 9600f, 8000f),

            // // new VOManager.SubtitleLine("That’s right. For the next ten seconds…", 19000f, 3000f),

            // // new VOManager.SubtitleLine("You are judge, jury, and, really, executioner.", 21700f, 5500f),

            // new VOManager.SubtitleLine("So show us.", 27000f, 2500f),
            
            new VOManager.SubtitleLine("So show us.", 17400f, 2300f),

            new VOManager.SubtitleLine("What choice will you make?", 19600f, 3000f),
        });

        yield return new WaitForSeconds(.5f);

        VOManager.Instance.PlaySoundFX(lightOnClip);
        trolleyLight.intensity = 10000f;
        VOManager.Instance.PlaySoundFX(trolleyClip);

        
        // 0:26 FIVE people
        yield return new WaitForSeconds(3.5f);
        VOManager.Instance.PlaySoundFX(lightOnClip);
        fiveLight.intensity = 1000f;



        
        // 0:30 you have a LEVER
        yield return new WaitForSeconds(4.5f);
        VOManager.Instance.PlaySoundFX(lightOnClip);

        leverLight.intensity = 1000f;

        
        // 0:37 only one would be the victim
        yield return new WaitForSeconds(6.8f);
        fiveLight.intensity = 0f;
        oneLight.intensity = 1000f;

        leverLight.intensity = 0;
        VOManager.Instance.PlaySoundFX(lightOnClip);
        
        // ends at 0:47
        yield return new WaitForSeconds(8f);
        StartMainScene();

    }

    private IEnumerator GyrateCurtainLights()
    {
        float time = 0f;

        Quaternion baseRot1 = curtainLight1.transform.rotation;
        Quaternion baseRot2 = curtainLight2.transform.rotation;
        Quaternion baseRot3 = curtainLight3.transform.rotation;

        while (true)
        {
            time += Time.deltaTime * gyrateSpeed;

            float offset1 = Mathf.Sin(time) * gyrateAngle;
            float offset2 = Mathf.Sin(time + 2f) * gyrateAngle;
            float offset3 = Mathf.Sin(time + 4f) * gyrateAngle;

            curtainLight1.transform.rotation =
                baseRot1 * Quaternion.Euler(0, offset1, 0);

            curtainLight2.transform.rotation =
                baseRot2 * Quaternion.Euler(0, offset2, 0);

            curtainLight3.transform.rotation =
                baseRot3 * Quaternion.Euler(0, offset3, 0);

            yield return null;
        }
    }


    private void LightsOff()
    {
        leverLight.intensity = 0f;
        trolleyLight.intensity = 0f;
        oneLight.intensity = 0f;
        fiveLight.intensity = 0f;
    }
    private void CurtainLightsOff()
    {
        curtainLight1.intensity = 0f;
        curtainLight2.intensity = 0f;
        curtainLight3.intensity = 0f;
    }
    private void CurtainLightsOn()
    {
        curtainLight1.intensity = 1000f;
        curtainLight2.intensity = 1000f;
        curtainLight3.intensity = 1000f;
    }

    private void MoveLightsAsideAndDim()
    {
        Quaternion targetRot1 = Quaternion.Euler(0, -90f, 0);
        Quaternion targetRot2 = Quaternion.Euler(0, 90f, 0);
        Quaternion targetRot3 = Quaternion.Euler(0, 180f, 0);

        float startIntensity1 = curtainLight1.intensity;
        float startIntensity2 = curtainLight2.intensity;
        float startIntensity3 = curtainLight3.intensity;

        float elapsed = 0f;

        while (elapsed < lightsExitDuration)
        {
            float t = elapsed / lightsExitDuration;

            curtainLight1.transform.rotation =
                Quaternion.Slerp(curtainLight1.transform.rotation, targetRot1, t);
            curtainLight2.transform.rotation =
                Quaternion.Slerp(curtainLight2.transform.rotation, targetRot2, t);
            curtainLight3.transform.rotation =
                Quaternion.Slerp(curtainLight3.transform.rotation, targetRot3, t);

            curtainLight1.intensity = Mathf.Lerp(startIntensity1, 0f, t);
            curtainLight2.intensity = Mathf.Lerp(startIntensity2, 0f, t);
            curtainLight3.intensity = Mathf.Lerp(startIntensity3, 0f, t);

            elapsed += Time.deltaTime;
            // yield return null;
        }

        curtainLight1.intensity = 0f;
        curtainLight2.intensity = 0f;
        curtainLight3.intensity = 0f;
    }

    private IEnumerator OpenCurtains()
    {
        Vector3 leftStart = leftCurtain.position;
        Vector3 rightStart = rightCurtain.position;

        Vector3 leftEnd = leftStart + Vector3.left * curtainOpenDistance;
        Vector3 rightEnd = rightStart + Vector3.right * curtainOpenDistance;

        float elapsed = 0f;

        while (elapsed < curtainOpenDuration)
        {
            float t = elapsed / curtainOpenDuration;
            leftCurtain.position = Vector3.Lerp(leftStart, leftEnd, t);
            rightCurtain.position = Vector3.Lerp(rightStart, rightEnd, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        leftCurtain.position = leftEnd;
        rightCurtain.position = rightEnd;
    }

    private void StartMainScene()
    {
        VOManager.Instance.PlaySoundFX(clock);

        FindObjectOfType<LevelNameDisplay>().ShowLevelName("Level 1: 5 vs 1");

        Debug.Log("Intro complete → transition to main scene");
        LevelDirector.Active.EndLevel("track_a");
    }

}
