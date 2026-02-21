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
    public AudioClip introClip;      
    public AudioClip transitionClip;  
    public AudioClip trolleyClip;  


    [Header("Audience")]
    public AudioClip applauseClip;     

    [Header("Extra SFX")]
    public AudioClip lightOnClip;    
    public AudioClip introBGM; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }


    private IEnumerator PlayIntroSequence()
    {
        Debug.Log("PlayIntroSequence started");
        LightsOff();

        LightManager.Instance.TurnOffArchLights();
        LightManager.Instance.TurnOffWalkwayLights();

        // Subtitles EXAMPLE for intro orchestration
        // Add a subtitles line for each, where the parameters are:
        // (Subtitle Sentence, start time in milliseconds after audio is triggered, duration in milliseconds)
        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Welcome… welcome… welcome…", 814f, 2421f),

            new VOManager.SubtitleLine("to The Trolley Show!", 3795f, 2500f),

            new VOManager.SubtitleLine("I'll be your host for the show,", 7462f, 1500f),

            new VOManager.SubtitleLine("and tonight, you get to be the most powerful person in the room.", 9000f, 4500f),

            new VOManager.SubtitleLine("No pressure.", 14114f, 532f),

            new VOManager.SubtitleLine("Here on The Trolley Show, we take life’s toughest philosophical questions…", 16233f, 6000f),

            new VOManager.SubtitleLine("…and we solve them…", 22500f, 1500f),

            new VOManager.SubtitleLine("in the most exciting way.", 24000f, 2000f),
        });

        VOManager.Instance.PlayLine(introClip);

        yield return new WaitForSeconds(5);

        VOManager.Instance.PlayAudience(lightOnClip);

        yield return new WaitForSeconds(1);

        LightsOn();

        Debug.Log("lights on");

        lightRoutine = StartCoroutine(GyrateCurtainLights());

        yield return new WaitForSeconds(introClip.length - 8);

        VOManager.Instance.PlayAudience(applauseClip);

        StopCoroutine(lightRoutine);
        MoveLightsAsideAndDim();

        // open curtains
        // yield return StartCoroutine(OpenCurtains());
        yield return CurtainController.Instance.OpenIntroCurtains();

        LightManager.Instance.TurnOnArchLights();
        LightManager.Instance.TurnOnWalkwayLights();


        VOManager.Instance.StartBackgroundMusic(introBGM);

        // yield return new WaitForSeconds(2);

        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("A trolley is speeding down the tracks.", 640f, 2500f),

            new VOManager.SubtitleLine("Straight ahead… are five people.", 3360f, 4000f),

            new VOManager.SubtitleLine("But! You have a lever.", 7500f, 2800f),

            new VOManager.SubtitleLine("Pull it, and the trolley switches tracks, where only one would be the victim.", 10500f, 8000f),

            new VOManager.SubtitleLine("That’s right. For the next ten seconds…", 19000f, 3000f),

            new VOManager.SubtitleLine("You are judge, jury, and, really, executioner.", 21700f, 5500f),

            new VOManager.SubtitleLine("So show us.", 27000f, 2500f),

            new VOManager.SubtitleLine("What choice will you make?", 30000f, 3000f),
        });
        VOManager.Instance.PlayLine(transitionClip);
        

        yield return new WaitForSeconds(1);

        VOManager.Instance.PlaySoundFX(lightOnClip);

        trolleyLight.intensity = 10000f;

        VOManager.Instance.PlaySoundFX(trolleyClip);

        yield return new WaitForSeconds(4);

        VOManager.Instance.PlaySoundFX(lightOnClip);
        fiveLight.intensity = 100f;

        yield return new WaitForSeconds(5);

        VOManager.Instance.PlaySoundFX(lightOnClip);

        leverLight.intensity = 10000f;

        yield return new WaitForSeconds(4);

        fiveLight.intensity = 0f;
        oneLight.intensity = 100f;

        leverLight.intensity = 0;
        VOManager.Instance.PlaySoundFX(lightOnClip);

        yield return new WaitForSeconds(10);

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
        curtainLight1.intensity = 0f;
        curtainLight2.intensity = 0f;
        curtainLight3.intensity = 0f;
        leverLight.intensity = 0f;
        trolleyLight.intensity = 0f;
        oneLight.intensity = 0f;
        fiveLight.intensity = 0f;
    }

        private void LightsOn()
    {
        curtainLight1.intensity = 100f;
        curtainLight2.intensity = 100f;
        curtainLight3.intensity = 100f;
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
        Debug.Log("Intro complete → transition to main scene");
        LevelDirector.Active.EndLevel("track_a");
    }

}
