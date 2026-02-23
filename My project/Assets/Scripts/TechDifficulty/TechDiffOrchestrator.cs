using UnityEngine;
using System.Collections;
using UnityEngine.Video;

public class TechDiffOrchestrator : MonoBehaviour
{
    [Header("VO Clips")]
    public AudioClip techDiffIntroClip1;  
    public AudioClip techDiffIntroClip2;  
    public AudioClip droppedClip;  
    public AudioClip powerDownSFX;
    public AudioClip whirSFX;

    [Header("TV Screen")]
    public Transform projector;
    private float projDistance = 10;
    private float projTime = 6;

    public Transform cat;
    private float catDistance = 8;
    private float catTime = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PlayTechDiffLevel());
    }

    // Update is called once per frame
    void Update()
    {
       if (RBController.Instance.dropped)
        {
            
        }
    }

    private IEnumerator PlayTechDiffLevel()
    {
        // VOManager.Instance.PlayLine(techDiffIntroClip1);
        // yield return new WaitForSeconds(3);

        VOManager.Instance.StopBackgroundMusic();

        VOManager.Instance.PlaySoundFX(powerDownSFX);

        StartCoroutine(LightManager.Instance.TurnOffAllLights(2));

        yield return new WaitForSeconds(6);

        VOManager.Instance.PlayLine(techDiffIntroClip2);

        yield return new WaitForSeconds(10);

        VOManager.Instance.PlaySoundFX(whirSFX);

        StartCoroutine(LowerProjector());

        VideoPlayer videoPlayer = projector.GetComponent<VideoPlayer>();

        yield return new WaitForSeconds(5);

        videoPlayer.isLooping = true;

        videoPlayer.Play();

        yield return new WaitForSeconds(8);

        cat.gameObject.SetActive(true);

        StartCoroutine(LowerCat());

        // trolli commercial should play




    }

    private IEnumerator LowerProjector()
    {
        Vector3 start = projector.position;
        Vector3 end = start - Vector3.up * projDistance;
        float elapsed = 0f;

        while (elapsed < projTime)
        {
            float t = elapsed / projTime;
            projector.position = Vector3.Lerp(start, end, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        projector.position = end;
    }

    private IEnumerator LowerCat()
    {
        Vector3 start = cat.position;
        Vector3 end = start - Vector3.up * catDistance;
        float elapsed = 0f;

        while (elapsed < catTime)
        {
            float t = elapsed / catTime;
            cat.position = Vector3.Lerp(start, end, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cat.position = end;
    }
}
