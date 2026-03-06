using UnityEngine;
using System;

public class FacePlayerModel : MonoBehaviour
{
    public Transform player;
    public AudioSource audio;
    public AudioClip Slap;
    private bool slapped = false;
    private DateTime slappedTime;

    void Update()
    {
        if (!player) return;
        transform.LookAt(player);
    }
    public void SlapSound()
    {
        audio.PlayOneShot(Slap);
    }
}