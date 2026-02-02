using UnityEngine;
using TMPro;

public class VictinSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip splat;
    public LayerMask layerMask;
    public Animator anim;
    // public static int killCount = 0;
    private bool triggered = false;
    // public TextMeshPro text;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.speed = Random.Range(0.8f, 1.5f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if ((layerMask.value & (1 << other.gameObject.layer)) > 0 )
        {
            triggered = true;
            anim.SetTrigger("dead");
            KillCounter.AddKill();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
