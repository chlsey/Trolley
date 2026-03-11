using UnityEngine;

public class TriggerDeatj : MonoBehaviour
{
    public Health health;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            health.HandleDeathSequenceTrigger();
        }
    }
}
