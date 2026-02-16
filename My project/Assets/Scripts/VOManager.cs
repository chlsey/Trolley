using UnityEngine;
using System.Collections;

public class VOManager : MonoBehaviour
{
    public static VOManager Instance;
    public AudioSource audioSource;

    // Chelsey: added new audio sources for different audio types so they can overlap

    [Header("Audio")]
    public AudioSource voiceSource;
    public AudioSource audienceSource;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    // end of new things


    private void Awake()
    {
        Instance = this;
    }

    public void Play(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(clip);
    }

    // Chelsey: added new player functions. these were all the intro level voice functions i used to better separate everything
    // im planning for each level to just call on VOManager to play whatever lines they need, this way the lines can also carry 
    // over to the next level
    // pls edit as needed!

    /*
    Play a line of narration, does not wait for narrator 
    to finish before continuing with the next sequence.
    */
    public void PlayLine(AudioClip clip)
    {
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    /*
    Play a line of narration, will wait for the narrator to finish before returning control.
    */
    public IEnumerator PlayLineWait(AudioClip clip)
    {
        // voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();

        yield return new WaitForSeconds(clip.length);
    }

    /*
    Play audience noises (returns immediately, 
    so sound will play over whatever comes after function call)
    */
    public void PlayAudience(AudioClip clip)
    {
        audienceSource.Stop();
        audienceSource.clip = clip;
        audienceSource.Play();
    }

    /*
    Play stage sfx noises (returns immediately, 
    so sound will play over whatever comes after function call)
    */
    public void PlaySoundFX(AudioClip clip)
    {
        sfxSource.Stop();
        sfxSource.clip = clip;
        sfxSource.Play();
    }


    /*
    Start background music playing on musicSource.
    */
    public void StartBackgroundMusic(AudioClip clip)
    {
        if (musicSource.isPlaying && musicSource.clip == clip)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    /*
    Stop the current background music playing on musicSource.
    */
    public void StopBackgroundMusic()
    {
        musicSource.loop = false;
        musicSource.Stop();
        musicSource.clip = null;
    }
    
}
