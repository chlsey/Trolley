using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorController door;   // Assign in Inspector
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        triggered = true;

        if (door != null)
            door.OpenDoor();
    }
}