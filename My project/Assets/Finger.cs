using UnityEngine;

public class Finger : MonoBehaviour
{
    public Animator animator;
    public string FingerAnimationBoolName = "Finger";
    public KeyCode FingerKeyToHold = KeyCode.F;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(FingerKeyToHold))
            animator.SetBool(FingerAnimationBoolName, true);
        else
            animator.SetBool(FingerAnimationBoolName, false);
    }
}
