using UnityEngine;

public class TriggerEyeOpen : MonoBehaviour
{
    public Health health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health.TriggerFadeFromBlack();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
