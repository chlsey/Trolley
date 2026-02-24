using UnityEngine;

public class RBController : MonoBehaviour
{
 private Rigidbody rb;
 public MeshRenderer meshRenderer;
 public LayerMask layerMask;
 public AudioSource audioSource;
 public AudioClip glassBreaking;
 public TechDiffOrchestrator techDiffOrchestrator;
 

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        DisableRigidbody();
    }

    void Update()
    {
        techDiffOrchestrator = FindObjectOfType<TechDiffOrchestrator>();
        if(techDiffOrchestrator.pressL && Input.GetKeyDown(KeyCode.L))
        {

            // destroy mesh collider immediately when key is pressed
            if (meshRenderer != null)
            {
                MeshCollider mc = meshRenderer.GetComponent<MeshCollider>();
                if (mc != null)
                    Destroy(mc);
            }
            else
            {
                // catch the next update check as a warning rather than throw an error
                Debug.LogWarning("meshRenderer is null");
            }

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

