using UnityEngine;
using UnityEngine.InputSystem;

public class LeverNoRating : MonoBehaviour
{
    public TrolleyMovement trolleyMovement;
    public CatapultProjectile catapultProjectile;
    public RedGreenLight redGreenLight;

    public GreenRedLight greenRedLight;
    public Animator anim;

    public Animator armAnim;
    public AudioSource audioSource;
    public AudioClip switchSound;
    public bool leverFlipped;

    private bool nearLever;
    private bool gotInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leverFlipped = false;
        nearLever = false;
    }

    // Update is called once per frame
    void Update()
    {
        gotInput = Input.GetKeyDown(KeyCode.E) ||
           (Gamepad.current != null && Gamepad.current.buttonWest.isPressed);
        if (nearLever && gotInput && trolleyMovement && !catapultProjectile)
        {
            trolleyMovement.SwitchTrack();
            redGreenLight.Toggle();
            greenRedLight.Toggle();
            anim.SetTrigger("Switch");
            armAnim.SetTrigger("Switch");
            audioSource.PlayOneShot(switchSound);
            Debug.Log("TrackSwitched");
            Debug.Log("Track Switched");
            enabled = false;
            leverFlipped = true;
        }
        if (nearLever && gotInput && catapultProjectile)
        {
            catapultProjectile.Launch();
            anim.SetTrigger("Switch");
            armAnim.SetTrigger("Switch");
            audioSource.PlayOneShot(switchSound);
            enabled = false;
            leverFlipped = true;
        }
        else if (nearLever && gotInput)
        {
            anim.SetTrigger("Switch");
            armAnim.SetTrigger("Switch");
            audioSource.PlayOneShot(switchSound);
            enabled = false;
            Debug.Log("lever flipped in lever script");
            leverFlipped = true;
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

    void Awake()
    {
        trolleyMovement ??= FindFirstObjectByType<TrolleyMovement>();
        if (redGreenLight == null)
            redGreenLight = FindFirstObjectByType<RedGreenLight>();
        if (greenRedLight == null)
            greenRedLight = FindFirstObjectByType<GreenRedLight>();
        if (anim == null)
            anim = GetComponent<Animator>();
        if (armAnim == null)
            armAnim = GetComponentInChildren<Animator>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    
}
