using UnityEngine;

public class FacePlayerModel : MonoBehaviour
{
    public Transform player;
    public AudioSource audio;
    public AudioClip Slap;

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