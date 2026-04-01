using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EndingDoorOrchestrator : MonoBehaviour
{
    [Header("Curtains")]
    public Transform doorACurtain;
    public Transform doorBCurtain;
    public float curtainOpenDistance = 10f;
    public float curtainOpenDuration = 1f;

    [Header("VO Clips")]
    public AudioClip segway;
    public AudioClip tracksIntro;
    public AudioClip coasterIntro;
    public AudioClip ending;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ThreeDoorsCoroutine());
    }

     private IEnumerator ThreeDoorsCoroutine()
    {
        VOManager.Instance.PlayLine(segway);
        yield return new WaitForSeconds(segway.length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
