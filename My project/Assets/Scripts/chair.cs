using UnityEngine;

public class chair : MonoBehaviour
{
    public GameObject chairObject;
    public LayerMask layerMask;
    public AudioSource audioSource;
    public AudioClip chairBreak;
    public GameObject prefabToSpawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

         
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) > 0 )
        {
            Instantiate(prefabToSpawn,transform.position,transform.rotation);
            audioSource.PlayOneShot(chairBreak);
            Destroy(gameObject);
            Debug.Log("ChairDestroyed");
        }
    }
}
