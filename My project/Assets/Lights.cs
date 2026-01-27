using UnityEngine;

public class Lights : MonoBehaviour
{
    public Light stageLights;
    public float minTime = 0.1f;
    public float maxTime = 0.5f;

    void Start()
    {
        StartCoroutine(Flash());
    }

    System.Collections.IEnumerator Flash()
    {
        while (true)
        {
            stageLights.color = Random.ColorHSV(
                0f, 1f,   // Hue
                0.8f, 1f, // Sat
                0.8f, 1f  // Val
            );

            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        }
    }
}
