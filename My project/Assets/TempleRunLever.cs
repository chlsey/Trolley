using UnityEngine;

public class TempleRunLever : MonoBehaviour
{
    public TrolleyMovement trolleyMovement;

    public Animator anim;

    public AudioSource audioSource;
    public AudioClip switchSound;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            trolleyMovement.SwitchTrack();
            
            anim.SetTrigger("Switch");
            
            audioSource.PlayOneShot(switchSound);
            Debug.Log("TrackSwitched");
            enabled = false;
        }
    }
    
}
