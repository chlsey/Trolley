using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance;

    [Header("Overarching Lights")]
    public Light[] archLights;
    
    [Header("Walkway Lights")]
    public Light[] walkLights;

    [Header("Stage Main Lights")]
    public Light mainLight1;
    public Light mainLight2;
    public Light mainLight3;


    [Header("Track Lights")]
    public Light trackALight;
    public Light trackBLight;



    private void Awake()
    {
        Instance = this;
    }

    public void TurnOffArchLights()
    {
        foreach (Light light in archLights)
        {
            if (light != null)
                light.enabled = false;
        }
        
    }

    public void TurnOnArchLights()
    {
        foreach (Light light in archLights)
        {
            if (light != null)
                light.enabled = true;
        }
        
    }

    public void TurnOffWalkwayLights()
    {
        foreach (Light light in walkLights)
        {
            if (light != null)
                light.enabled = false;
        }
        
    }

    public void TurnOnWalkwayLights()
    {
        foreach (Light light in walkLights)
        {
            if (light != null)
                light.enabled = true;
        }
        
    }

    public IEnumerator TurnOffAllLights(float seconds)
    {
        List<Light> allLights = new List<Light>();

        allLights.AddRange(archLights);
        allLights.AddRange(walkLights);

        allLights.Add(mainLight1);
        allLights.Add(mainLight2);
        allLights.Add(mainLight3);

        allLights.Add(trackALight);
        allLights.Add(trackBLight);

        // store starting intensities
        Dictionary<Light, float> startIntensities = new Dictionary<Light, float>();

        foreach (Light light in allLights)
        {
            if (light != null)
                startIntensities[light] = light.intensity;
        }

        float elapsed = 0f;

        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / seconds);

            foreach (Light light in startIntensities.Keys)
            {
                light.intensity = Mathf.Lerp(startIntensities[light], 0f, t);
            }

            yield return null;
        }

        foreach (Light light in startIntensities.Keys)
        {
            light.intensity = 0f;
            light.enabled = false;
        }
    }
}
