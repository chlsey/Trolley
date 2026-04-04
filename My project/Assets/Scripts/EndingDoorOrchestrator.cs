using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EndingDoorOrchestrator : MonoBehaviour
{
    public static EndingDoorOrchestrator Instance { get; private set; }

    [Header("Curtains")]
    public Transform doorACurtain;
    public Transform doorBCurtain;
    public float curtainOpenDistance = 10f;
    public float curtainOpenDuration = 1f;
    public Transform glassPanel;

    [Header("VO Clips")]
    public AudioClip segway;
    public AudioClip tracksIntro;
    public AudioClip coasterIntro;
    public AudioClip ending;

    [Header("Lights")]
    public Light leftL;
    public Light rightL;
    public Light leftDoorKey;
    public Light rightDoorKey;
    [Header("SFX")]
    public AudioClip lightOn;

    public bool canEnter = false;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;                  
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftDoorKey.intensity = 0;
        rightDoorKey.intensity = 0;
        leftL.intensity = 0;
        rightL.intensity = 0;
        StartCoroutine(ThreeDoorsCoroutine());
    }

     private IEnumerator ThreeDoorsCoroutine()
    {
        VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
        {
            new VOManager.SubtitleLine("Ahh… feels great to have full camera control again.", 0f, 4150f),

            new VOManager.SubtitleLine("Now then, welcome to our final segment of tonight’s show…", 4200f, 6000f),

            new VOManager.SubtitleLine("Yes, contestant.", 10000f, 2021f),

            new VOManager.SubtitleLine("I know you’ve been DYING to tie yourself to the tracks.", 12100f, 4421f),

            new VOManager.SubtitleLine("Or... if survival is more your style...", 17100f, 4421f),

            new VOManager.SubtitleLine("May I interest you in the Trolley Getaway Experience?", 21421f, 4121f),
            
            new VOManager.SubtitleLine("A thrilling ride that sacrifices five others on your behalf!", 26821f, 5121f),

            new VOManager.SubtitleLine("So, what will be your choice, my dear contestant?", 33921f, 6021f),

        });

        VOManager.Instance.PlayLine(segway);
        yield return new WaitForSeconds(segway.length);
        VOManager.Instance.PlaySoundFX(lightOn);
        leftL.intensity = 200;
        rightL.intensity = 200;
        VOManager.Instance.PlayLine(tracksIntro);
        
        yield return new WaitForSeconds(6);
        yield return StartCoroutine(OpenCurtain(doorACurtain));
        yield return new WaitForSeconds(tracksIntro.length - 6);

        VOManager.Instance.PlayLine(coasterIntro);
        yield return new WaitForSeconds(4);
        yield return StartCoroutine(OpenCurtain(doorBCurtain));
        yield return new WaitForSeconds(coasterIntro.length - 4);

        curtainOpenDistance = -5f;
        VOManager.Instance.PlayLine(ending);
        yield return new WaitForSeconds(2);

        yield return StartCoroutine(OpenCurtain(glassPanel));
        canEnter = true;
        leftL.intensity = 0;
        rightL.intensity = 0;
        leftDoorKey.intensity = 25;
        rightDoorKey.intensity = 25;
        // yield return new WaitForSeconds(ending.length - 2);
    }

    private IEnumerator OpenCurtain(Transform curtain)
    {
        if (curtain == null) yield break;

        Vector3 startPos = curtain.position;
        Vector3 targetPos = startPos + Vector3.up * curtainOpenDistance;
        float elapsed = 0f;

        while (elapsed < curtainOpenDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / curtainOpenDuration);
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            curtain.position = Vector3.Lerp(startPos, targetPos, smooth);
            yield return null;
        }

        curtain.position = targetPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
