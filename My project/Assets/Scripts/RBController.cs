using UnityEngine;

public class RBController : MonoBehaviour
{
 private Rigidbody rb;
 public MeshRenderer meshRenderer;
 public LayerMask layerMask;
 public AudioSource audioSource;
 public AudioClip glassBreaking;

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        DisableRigidbody();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            
            EnableRigidbody();
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) > 0 )
        {
            audioSource.PlayOneShot(glassBreaking);
            meshRenderer.enabled = false;
            Debug.Log("hitGround");
        }
    }
    public void EnableRigidbody()
    {
        if (rb != null)
        {
            rb.isKinematic = false; 
            rb.useGravity = true;   
        }
    }

  
    public void DisableRigidbody()
    {
        if (rb != null)
        {
            rb.isKinematic = true;  
            rb.useGravity = false;  
        }
    }
}

