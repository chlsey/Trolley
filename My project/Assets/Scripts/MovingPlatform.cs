using System;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform platform;
    public Rigidbody platformRb;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Debug.Log("Player detected");
            other.transform.root.SetParent(platform);
            // PlayerMovement pm = other.GetComponentInParent<PlayerMovement>();
            // if (pm != null)
            // {
            //     pm.platformRb = platformRb;
            //     Debug.Log("Player linked to platform velocity");
            // }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            transform.DetachChildren();
            // PlayerMovement pm = other.GetComponentInParent<PlayerMovement>();
            // if (pm != null)
            // {
            //     pm.platformRb = null;
            //     Debug.Log("Player linked to platform velocity");
            // }
        }

    }
}
