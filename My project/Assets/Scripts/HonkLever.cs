using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HonkLever : MonoBehaviour
{

    public AudioClip honk;
    public AudioClip noWorkClip;

    public Animator anim;

    public Animator armAnim;
    private bool nearLever;
    private bool gotInput;
    private bool clicked = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nearLever = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        nearLever = true;
    }
    private void OnTriggerExit(Collider other)
    {
        nearLever = false;
    }

    // Update is called once per frame
    void Update()
    {
        gotInput = Input.GetKeyDown(KeyCode.E) ||
           (Gamepad.current != null && Gamepad.current.buttonWest.isPressed);
        if (nearLever && gotInput)
        {
            if (clicked)
            {
                VOManager.Instance.PlaySoundFX(honk);
            } else
            {
                clicked = true;
                anim.SetTrigger("Switch");
                armAnim.SetTrigger("Switch");

                VOManager.Instance.PlaySoundFX(honk);

                VOManager.Instance.PlayLine(noWorkClip);

                VOManager.Instance.ShowSubtitle(new List<VOManager.SubtitleLine>
                {
                    new VOManager.SubtitleLine("Whoops! Forgot to tell ya,", 560f, 1500f),

                    new VOManager.SubtitleLine("that one doesn’t work.", 2200f, 1600f),

                });
            }
        
        }
    }

    void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
        if (armAnim == null)
            armAnim = GetComponentInChildren<Animator>();
    }
}