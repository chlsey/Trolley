using UnityEngine;

// Attach to Level 7 prefab to reduce victim SFX volume (1 million people level).
public class QuietVictims : MonoBehaviour
{
    [Range(0f, 1f)]
    public float volumeMultiplier = 0.15f;

    void Start()
    {
        var victims = GetComponentsInChildren<VictimSFX>(true);
        foreach (var victim in victims)
        {
            if (victim.audioSource != null)
                victim.audioSource.volume *= volumeMultiplier;
        }
    }
}
