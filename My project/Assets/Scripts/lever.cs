using UnityEngine;

public class lever : MonoBehaviour
{
    public TrolleyMovement trolleyMovement;
    public Animator anim;
    public AudioSource audioSource;
    public AudioClip switchSound;

    private bool nearLever;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nearLever = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (nearLever && Input.GetKeyDown(KeyCode.E))
        {
            trolleyMovement.SwitchTrack();
            anim.SetTrigger("Switch");
            audioSource.PlayOneShot(switchSound);
            Debug.Log("TrackSwitched");
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        nearLever = true;
    }
    private void OnTriggerExit(Collider other)
    {
        nearLever = false;
    }
    
}
