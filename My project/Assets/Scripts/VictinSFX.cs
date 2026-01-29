using UnityEngine;
using TMPro;

public class VictinSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip splat;
    public LayerMask layerMask;
    public static int killCount = 0;
    private bool triggered = false;
    public TextMeshPro text;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = "KILLS: 0";
    }
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if ((layerMask.value & (1 << other.gameObject.layer)) > 0 )
        {
            triggered = true;
            audioSource.PlayOneShot(splat);
            killCount ++;
            text.text = "KILLS: " + killCount;
            Debug.Log(killCount);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
