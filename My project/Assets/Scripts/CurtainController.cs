using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CurtainController : MonoBehaviour
{

    public static CurtainController Instance;

    [Header("Curtains")]
    public Transform rightIntroCurtain;
    public Transform leftIntroCurtain;
    // public Transform rightCurtain;
    // public Transform leftCurtain;
    public float curtainOpenDistance = 4f;
    public float curtainOpenDuration = 1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        
    }

    // // Update is called once per frame
    // public IEnumerator OpenCurtains()
    // {
    //     Vector3 leftStart = leftCurtain.position;
    //     Vector3 rightStart = rightCurtain.position;

    //     Vector3 leftEnd = leftStart + Vector3.left * curtainOpenDistance;
    //     Vector3 rightEnd = rightStart + Vector3.right * curtainOpenDistance;

    //     float elapsed = 0f;

    //     while (elapsed < curtainOpenDuration)
    //     {
    //         float t = elapsed / curtainOpenDuration;
    //         leftCurtain.position = Vector3.Lerp(leftStart, leftEnd, t);
    //         rightCurtain.position = Vector3.Lerp(rightStart, rightEnd, t);

    //         elapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     leftCurtain.position = leftEnd;
    //     rightCurtain.position = rightEnd;
    // }



    // public IEnumerator CloseCurtains()
    // {
    //     Vector3 leftStart = leftCurtain.position;
    //     Vector3 rightStart = rightCurtain.position;

    //     Vector3 leftEnd = leftStart - Vector3.left * curtainOpenDistance;
    //     Vector3 rightEnd = rightStart - Vector3.right * curtainOpenDistance;

    //     float elapsed = 0f;

    //     while (elapsed < curtainOpenDuration)
    //     {
    //         float t = elapsed / curtainOpenDuration;
    //         leftCurtain.position = Vector3.Lerp(leftStart, leftEnd, t);
    //         rightCurtain.position = Vector3.Lerp(rightStart, rightEnd, t);

    //         elapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     leftCurtain.position = leftEnd;
    //     rightCurtain.position = rightEnd;
    // }


    public IEnumerator OpenIntroCurtains()
    {
        Vector3 leftStart = leftIntroCurtain.position;
        Vector3 rightStart = rightIntroCurtain.position;

        Vector3 leftEnd = leftStart + Vector3.left * curtainOpenDistance;
        Vector3 rightEnd = rightStart + Vector3.right * curtainOpenDistance;

        float elapsed = 0f;

        while (elapsed < curtainOpenDuration)
        {
            float t = elapsed / curtainOpenDuration;
            leftIntroCurtain.position = Vector3.Lerp(leftStart, leftEnd, t);
            rightIntroCurtain.position = Vector3.Lerp(rightStart, rightEnd, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        leftIntroCurtain.position = leftEnd;
        rightIntroCurtain.position = rightEnd;

        leftIntroCurtain.gameObject.SetActive(false);
        rightIntroCurtain.gameObject.SetActive(false);

        // leftCurtain.gameObject.SetActive(true);
        // rightCurtain.gameObject.SetActive(true);
    }


}

