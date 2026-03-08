using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Serialization;

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
    
    [Header("Subtitles")]
    public TextMeshProUGUI subtitleText;

    [System.Serializable]
    public struct SubtitleLine
    {
        // Subtitle text to display.
        public string text;
        // When to show this line, in milliseconds from the sequence start.
        public float timeStartMilliseconds;
        // How long this line should stay visible, in milliseconds.
        public float durationMilliseconds;

        public SubtitleLine(string text, float timeStartMilliseconds, float durationMilliseconds)
        {
            this.text = text;
            this.timeStartMilliseconds = timeStartMilliseconds;
            this.durationMilliseconds = durationMilliseconds;
        }
    }

    private Coroutine subtitleCoroutine;
    private float baseAudioSourceVolume;
    private float baseVoiceSourceVolume;
    private float baseAudienceSourceVolume;
    private float baseMusicSourceVolume;
    private float baseSfxSourceVolume;




    private void Awake()
    {
        Instance = this;
        CacheBaseVolumes();
    }

    private void OnEnable()
    {
        SettingsManager.Instance.SettingsChanged += ApplySettings;
    }

    private void Start()
    {
        SettingsData settings = SettingsManager.Instance.GetSettingsData();
        float generalAudioMultiplier = settings.generalAudioMultiplier;

        if (audioSource != null)
            audioSource.volume = baseAudioSourceVolume * generalAudioMultiplier;

        if (voiceSource != null)
            voiceSource.volume = baseVoiceSourceVolume * generalAudioMultiplier;

        if (audienceSource != null)
            audienceSource.volume = baseAudienceSourceVolume * generalAudioMultiplier;

        if (musicSource != null)
            musicSource.volume = baseMusicSourceVolume * generalAudioMultiplier;

        if (sfxSource != null)
            sfxSource.volume = baseSfxSourceVolume * generalAudioMultiplier;
    }

    private void OnDisable()
    {
        SettingsManager.Instance.SettingsChanged -= ApplySettings;
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

    // USAGE INSTRUCTIONS!
    // Plays a timed subtitle sequence
    // One or more subtitle lines, each with start time and duration in milliseconds.
    // Usage example:
    // VOManager.Instance.ShowSubtitle(new List&lt;VOManager.SubtitleLine&gt;
    // {
    //     new VOManager.SubtitleLine("Line 1", 0f, 1400f),
    //     new VOManager.SubtitleLine("Line 2", 1600f, 1000f),
    // });
    //
    // Notes:
    // - Calling ShowSubtitle clears any active subtitle sequence.
    // - Lines are sorted by start time before being played.
    // - If lines overlap, the next line replaces the current line at its start time.
    public void ShowSubtitle(List<SubtitleLine> subtitleLines)
    {
        if (subtitleText == null)
        {
            Debug.LogWarning("VOManager: subtitleText is not assigned.", this);
            return;
        }

        if (subtitleLines == null || subtitleLines.Count == 0)
        {
            Debug.LogWarning("VOManager: ShowSubtitle requires at least one subtitle line.", this);
            return;
        }

        for (int index = 0; index < subtitleLines.Count; index++)
        {
            if (subtitleLines[index].timeStartMilliseconds < 0f)
            {
                Debug.LogWarning($"VOManager: subtitle line {index} must have timeStartMilliseconds >= 0.", this);
                return;
            }

            if (subtitleLines[index].durationMilliseconds <= 0f)
            {
                Debug.LogWarning($"VOManager: subtitle line {index} must have durationMilliseconds > 0.", this);
                return;
            }
        }

        ClearSubtitle();

        var queuedLines = new List<SubtitleLine>(subtitleLines);
        subtitleCoroutine = StartCoroutine(RunSubtitleSequence(queuedLines));
    }

    public void ClearSubtitle()
    {
        if (subtitleCoroutine != null)
        {
            StopCoroutine(subtitleCoroutine);
            subtitleCoroutine = null;
        }

        if (subtitleText != null)
            subtitleText.text = string.Empty;
    }

    private IEnumerator RunSubtitleSequence(List<SubtitleLine> subtitleLines)
    {
        subtitleLines.Sort((left, right) => left.timeStartMilliseconds.CompareTo(right.timeStartMilliseconds));

        float elapsedMilliseconds = 0f;

        for (int index = 0; index < subtitleLines.Count; index++)
        {
            SubtitleLine line = subtitleLines[index];
            float nextLineStartMilliseconds = index + 1 < subtitleLines.Count
                ? subtitleLines[index + 1].timeStartMilliseconds
                : float.PositiveInfinity;

            if (line.timeStartMilliseconds > elapsedMilliseconds)
            {
                float waitToStartMilliseconds = line.timeStartMilliseconds - elapsedMilliseconds;
                yield return new WaitForSeconds(waitToStartMilliseconds / 1000f);
                elapsedMilliseconds = line.timeStartMilliseconds;
            }

            subtitleText.text = line.text ?? string.Empty;

            float lineEndMilliseconds = line.timeStartMilliseconds + line.durationMilliseconds;
            float hideAtMilliseconds = Mathf.Min(lineEndMilliseconds, nextLineStartMilliseconds);

            if (hideAtMilliseconds > elapsedMilliseconds)
            {
                float visibleForMilliseconds = hideAtMilliseconds - elapsedMilliseconds;
                yield return new WaitForSeconds(visibleForMilliseconds / 1000f);
                elapsedMilliseconds = hideAtMilliseconds;
            }

            if (lineEndMilliseconds <= nextLineStartMilliseconds && subtitleText != null)
                subtitleText.text = string.Empty;
        }

        if (subtitleText != null)
            subtitleText.text = string.Empty;

        subtitleCoroutine = null;
    }

    private void CacheBaseVolumes()
    {
        baseAudioSourceVolume = audioSource != null ? audioSource.volume : 0f;
        baseVoiceSourceVolume = voiceSource != null ? voiceSource.volume : 0f;
        baseAudienceSourceVolume = audienceSource != null ? audienceSource.volume : 0f;
        baseMusicSourceVolume = musicSource != null ? musicSource.volume : 0f;
        baseSfxSourceVolume = sfxSource != null ? sfxSource.volume : 0f;
    }

    private void ApplySettings(SettingsData settings)
    {
        if (audioSource != null)
            audioSource.volume = baseAudioSourceVolume * settings.generalAudioMultiplier;

        if (voiceSource != null)
            voiceSource.volume = baseVoiceSourceVolume * settings.generalAudioMultiplier;

        if (audienceSource != null)
            audienceSource.volume = baseAudienceSourceVolume * settings.generalAudioMultiplier;

        if (musicSource != null)
            musicSource.volume = baseMusicSourceVolume * settings.generalAudioMultiplier;

        if (sfxSource != null)
            sfxSource.volume = baseSfxSourceVolume * settings.generalAudioMultiplier;
    }

}
