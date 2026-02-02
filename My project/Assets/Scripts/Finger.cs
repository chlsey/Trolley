using UnityEngine;

public class Finger : MonoBehaviour
{
    public Animator animator;
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
    
        nearLever = true;
        
    }
    private void OnTriggerExit(Collider other)
    {
        
        nearLever = false;
        
    }
}
