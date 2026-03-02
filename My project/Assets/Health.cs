using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Health : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public RawImage uiImage;
    public LayerMask layerMask;
    public AudioListener audioListener;
    public float fadeDuration = 0.5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        uiImage.enabled = true;
        Color c = uiImage.color;
        c.a = 1f;
        uiImage.color = c;
        TriggerFadeFromBlack();
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
            StartCoroutine(FadeToBlack());
            Debug.Log("Dead");
            StartCoroutine(DisableAudioListener());
        }
    }
    public void TriggerFadeToBlack()
    {
        StartCoroutine(FadeToBlack());
    }
    public void TriggerFadeFromBlack()
    {
        StartCoroutine(FadeFromBlack());
    }
    private System.Collections.IEnumerator DisableAudioListener()
    {
        yield return new WaitForSeconds(0.3f);
        audioListener.enabled = false;
    }
    private IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        Color c = uiImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            uiImage.color = c;
            yield return null;
        }
        c.a = 1f;
        uiImage.color = c;
        yield return new WaitForSeconds(0.3f);
        
    }
    private IEnumerator FadeFromBlack()
    {
        float elapsed = 0f;
        Color c = uiImage.color;
        c.a = 1f;
        uiImage.color = c;
        uiImage.enabled = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            uiImage.color = c;
            yield return null;
        }
        c.a = 0f;
        uiImage.color = c;
    }

}
