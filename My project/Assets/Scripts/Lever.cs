using UnityEngine;
using UnityEngine.InputSystem;

public class Lever : MonoBehaviour
{
    public TrolleyMovement trolleyMovement;
    public CatapultProjectile catapultProjectile;
    public RedGreenLight redGreenLight;
    public bool isClusterTruckLevel;
    public NextScene nextScene;
    public GreenRedLight greenRedLight;
    public Animator anim;

    public Animator armAnim;
    public AudioSource audioSource;
    public AudioClip switchSound;
    public VOManager voManager;
    public Rating rating;
    public bool leverFlipped = false;

    private bool nearLever;
    private bool gotInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nearLever = false;

    }

    // Update is called once per frame
    void Update()
    {
        gotInput = Input.GetKeyDown(KeyCode.E) ||
           (Gamepad.current != null && Gamepad.current.buttonWest.wasReleasedThisFrame);
        if (!leverFlipped && nearLever && gotInput && trolleyMovement && !catapultProjectile)
        {
            leverFlipped = true;
            trolleyMovement.SwitchTrack();
            redGreenLight.Toggle();
            greenRedLight.Toggle();
            anim.SetTrigger("Switch");
            armAnim.SetTrigger("Switch");
            audioSource.PlayOneShot(switchSound);
            Debug.Log("TrackSwitched");
            Debug.Log("Track Switched");
            // rating.ChangeRating(-0.5f);
            
            if (isClusterTruckLevel)
            {
                nextScene.TriggerSceneChange();
            }
        }
        if (!leverFlipped && nearLever && gotInput && catapultProjectile)
        {
            catapultProjectile.Launch();
            anim.SetTrigger("Switch");
            armAnim.SetTrigger("Switch");
            audioSource.PlayOneShot(switchSound);
            leverFlipped = true;
            // enabled = false;
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
        if (rating == null)
            rating = FindFirstObjectByType<Rating>();

        // Reset track lights to defaults: green on Track A, red on Track B
        if (greenRedLight != null)
            greenRedLight.SetGreen();
        if (redGreenLight != null)
            redGreenLight.SetRed();

        Debug.Log($"Lever.Awake reset lights - greenRedLight: {greenRedLight}, redGreenLight: {redGreenLight}");
    }
    
}
