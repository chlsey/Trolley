using UnityEngine;
using System.Collections;

public class IntroVoiceController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource voiceSource;
    public AudioSource audienceSource;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public static IntroVoiceController Instance;
    

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlayLine(AudioClip clip)
    {
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public IEnumerator PlayLineWait(AudioClip clip)
    {
        // voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();

        yield return new WaitForSeconds(clip.length);
    }

    public void PlayAudience(AudioClip clip)
    {
        audienceSource.Stop();
        audienceSource.clip = clip;
        audienceSource.Play();
    }

    public void PlaySoundFX(AudioClip clip)
    {
        sfxSource.Stop();
        sfxSource.clip = clip;
        sfxSource.Play();
    }


    public void StartBackgroundMusic(AudioClip clip)
    {
        if (musicSource.isPlaying && musicSource.clip == clip)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        musicSource.loop = false;
        musicSource.Stop();
        musicSource.clip = null;
    }

}
