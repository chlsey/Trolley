using UnityEngine;

public class VolunteerLevelDisableLever : MonoBehaviour
{
    public Lever lever;

    void OnTriggerEnter (Collider other)
    {
        lever.enabled = false;
    }
}
