using UnityEngine;

public class TrackTeleportTrigger : MonoBehaviour
{
    public LayerMask layerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        
        
        PlayerMovement pm = other.GetComponentInParent<PlayerMovement>();

        pm.TeleportToTrack();
        Debug.Log("teleport");
        
    }
    
}
