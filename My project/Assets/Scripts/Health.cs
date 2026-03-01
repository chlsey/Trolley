using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public RawImage uiImage;
    public LayerMask layerMask;
    public AudioListener audioListener;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {

        if ((layerMask.value & (1 << other.gameObject.layer)) > 0 )
        {
            audioSource.PlayOneShot(audioClip);
            uiImage.enabled = true;
            Debug.Log("Dead");
            StartCoroutine(DisableAudioListener());
        }
    }
    private System.Collections.IEnumerator DisableAudioListener()
    {
        yield return new WaitForSeconds(0.3f);
        audioListener.enabled = false;
    }

}
