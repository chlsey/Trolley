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
        Debug.Log($"{nameof(LeverNoRating)} enabled, nearLever set back to false");
    }

    // Update is called once per frame
    void Update()
    {
        gotInput = Input.GetKeyDown(KeyCode.E) ||
           (Gamepad.current != null && Gamepad.current.buttonWest.wasReleasedThisFrame);

        if (!nearLever || !gotInput || leverFlipped)
        {
            return;
        }

        leverFlipped = true;
        enabled = false;

        if (trolleyMovement && !catapultProjectile)
        {
            Debug.Log($"SwitchTrack called by {nameof(LeverNoRating)} on {gameObject.name}");
            trolleyMovement.SwitchTrack();
            if (redGreenLight != null)
            {
                redGreenLight.Toggle();
            }
            else
            {
                Debug.LogWarning($"{nameof(LeverNoRating)} on {gameObject.name} is missing {nameof(redGreenLight)}. Track switched without updating the light.", this);
            }

            if (greenRedLight != null)
            {
                greenRedLight.Toggle();
            }
            else
            {
                Debug.LogWarning($"{nameof(LeverNoRating)} on {gameObject.name} is missing {nameof(greenRedLight)}. Track switched without updating the light.", this);
            }

            anim.SetTrigger("Switch");
            if (armAnim != null && armAnim != anim)
            {
                armAnim.SetTrigger("Switch");
            }
            audioSource.PlayOneShot(switchSound);
            Debug.Log("TrackSwitched");
            Debug.Log("Track Switched");
            return;
        }

        if (catapultProjectile)
        {
            catapultProjectile.Launch();
            anim.SetTrigger("Switch");
            if (armAnim != null && armAnim != anim)
            {
                armAnim.SetTrigger("Switch");
            }
            audioSource.PlayOneShot(switchSound);
            return;
        }

        if (!trolleyMovement)
        {
            Debug.LogWarning($"{nameof(LeverNoRating)} on {gameObject.name} has no {nameof(trolleyMovement)} reference.", this);
            anim.SetTrigger("Switch");
            if (armAnim != null && armAnim != anim)
            {
                armAnim.SetTrigger("Switch");
            }
            audioSource.PlayOneShot(switchSound);
            Debug.Log("lever flipped in lever script");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        nearLever = true;
        Debug.Log($"nearLever set true by {nameof(LeverNoRating)} on {gameObject.name}");
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

        if (redGreenLight == null || greenRedLight == null)
        {
            Debug.LogWarning($"{nameof(LeverNoRating)} on {gameObject.name} could not find both track lights. Level wiring is incomplete.", this);
        }
    }
    
}
