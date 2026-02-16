using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CurtainController : MonoBehaviour
{

    public static CurtainController Instance;

    [Header("Curtains")]
    public Transform rightCurtain;
    public Transform leftCurtain;
    public float curtainOpenDistance = 4f;
    public float curtainOpenDuration = 1f;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    public IEnumerator OpenCurtains()
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


    public IEnumerator CloseCurtains()
    {
        Vector3 leftStart = leftCurtain.position;
        Vector3 rightStart = rightCurtain.position;

        Vector3 leftEnd = leftStart - Vector3.left * curtainOpenDistance;
        Vector3 rightEnd = rightStart - Vector3.right * curtainOpenDistance;

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
}
