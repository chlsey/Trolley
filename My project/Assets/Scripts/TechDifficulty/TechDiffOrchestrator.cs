using UnityEngine;
using System.Collections;
using UnityEngine.Video;

public class TechDiffOrchestrator : MonoBehaviour
{
    [Header("VO Clips")]
    public AudioClip techDiffIntroClip1;  
    public AudioClip techDiffIntroClip2;  
    public AudioClip powerDownSFX;
    public AudioClip whirSFX;

    [Header("TV Screen")]
    public Transform projector;
    public float projDistance = 6;
    public float projTime = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PlayTechDiffLevel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PlayTechDiffLevel()
    {
        VOManager.Instance.PlayLine(techDiffIntroClip1);
        yield return new WaitForSeconds(3);

        VOManager.Instance.PlaySoundFX(powerDownSFX);

        StartCoroutine(LightManager.Instance.TurnOffAllLights(2));

        yield return new WaitForSeconds(6);

        VOManager.Instance.PlayLine(techDiffIntroClip2);

        VOManager.Instance.PlaySoundFX(whirSFX);

        StartCoroutine(LowerProjector());

        yield return new WaitForSeconds(6);



        // trolli commercial should play




    }

    private IEnumerator LowerProjector()
    {
        Vector3 start = transform.position;
        Vector3 end = start - Vector3.forward * projDistance;
        float elapsed = 0f;

        while (elapsed < projTime)
        {
            float t = elapsed / projDistance;
            projector.position = Vector3.Lerp(start, end, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        projector.position = end;
    }
}
