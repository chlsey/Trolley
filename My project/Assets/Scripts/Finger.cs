using UnityEngine;

public class Finger : MonoBehaviour
{
    public Animator animator;
     public string leverTag = "Lever";
    private bool nearLever = false;
    
  
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nearLever && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("Switch");
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    
        if (other.CompareTag(leverTag))
        {
            nearLever = true;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag(leverTag))
        {
            nearLever = false;
        }
        
    }
}
