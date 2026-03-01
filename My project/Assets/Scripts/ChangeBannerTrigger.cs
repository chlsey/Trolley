using UnityEngine;

public class ChangeBannerTrigger : MonoBehaviour
{
    private bool triggered = false;

    public Renderer targetRenderer;   // The object whose material changes
    public Material newMaterial;      // The material to switch to

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // if (!other.CompareTag("Player")) return;

        triggered = true;

        if (targetRenderer != null && newMaterial != null)
        {
            targetRenderer.material = newMaterial;
        }
    }
}