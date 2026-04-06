using System;
using UnityEngine;
using UnityEngine.Audio;


public struct SettingsData
{
    public float mouseSens;
    public float controllerSens;
    public float generalAudioMultiplier;
    public float voAudioMultiplier;
    public float musicAudioMultiplier;
    public float soundEffectsAudioMultiplier;
}


// SettingsManager is a persistant manager for player settings. Is not destroyed on scene deload
// Values are intended to be audjusted in settings menu.
// Exposes an event `SettingsChanged` that dependents subscribe to with a handler function.
// Event passes a struct of settings that handlers use to update local values. 

// FOR ANYONE DOING THE SETTINGS MENU
// PLEASE LOOK AT EXAMPLE BELOW, DEMONSTRATION OF HOW TO GET CURRENT SETTINGS, AUDJUST SETTINGS, AND SAVE AUDJUSTED SETTINGS.
// (Wrap this in a function and call it)
// vvvvvvvvvvvvvvvvvvvvvvvvv
//
// SettingsData SettingsStruct = SettingsManager.Instance.GetSettingsData();
// SettingsStruct.generalAudioMultiplier = 1.0f;
// SettingsStruct.mouseSens = 0.1f;
// SettingsManager.Instance.SaveSettings(SettingsStruct);


// THIS MAKES SURE LOADING SETTINGS RUNS FIRST BEFORE ANY OTHER SCRIPT IN SCENE
[DefaultExecutionOrder(-1)]
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // create event
    // anything that needs settings subscribes to this event for updates. (See VOManager.cs, PlayerCamera.cs)
    public event Action<SettingsData> SettingsChanged;

    private const string MouseSensKey = "settings.mouse_sens";
    private const string ControllerSensKey = "settings.controller_sens";

    // GENERAL AUDIO OVERHAUL
    // WE ARE NOW USING GLOBAL MIXERS TO PROPERLY ADJUST VOLUME/OTHER SETTINGS BEFORE PLAYING THEM
    // Basically:
    // Audio Clip/Source -> wired to a Audio Mixer Group -> Audio Mixer Group applies volume multipler before sound is played at runtime
    // SettingsManager -> volume change -> onChange -> apply to Audio Mixer Grouup
    // 
    // Internally in Audio:
    // We have AudioMixer -> AudioMixerGroups -> Audio Sources
    //
    // Audio Mixer Groups have a hierachy as well and are routed as such:
    //
    // -> Master
    //  -> VO
    //  -> Music 
    //  -> SoundEffects
    //
    // Please assign audio to THEIR CORRECT AudioMixerGroup (Don't assign to master volume)
    // For example, any audio in SoundEffects at run time -> applied SoundEffects set volume multiplier -> applied Master set volume multiplier -> audio is played

    [SerializeField] private AudioMixer generalAudioMixer;
    private const string MasterVolumeParameter = "MasterVolume";
    private const string VOVolumeParameter = "VOVolume";
    private const string MusicVolumeParameter = "MusicVolume";
    private const string SoundEffectsVolumeParameter = "SoundEffectsVolume";
    private const float MutedVolumeDb = -80.0f;


    private const string GeneralAudioKey = "settings.general_audio";
    private const string VOAudioKey = "settings.vo_audio";
    private const string MusicAudioKey = "settings.music_audio";
    private const string SoundEffectsAudioKey = "settings.sound_effects_audio";


    // DEFAULT SETTINGS
    // -------------------------------------------------------------------------------------
    [SerializeField] private float mouseSens = 0.1f;
    [SerializeField] private float controllerSens = 150.0f;
    [SerializeField] private float generalAudioMultiplier = 1.0f;
    [SerializeField] private float voAudioMultiplier = 1.0f;
    [SerializeField] private float musicAudioMultiplier = 1.0f;
    [SerializeField] private float soundEffectsAudioMultiplier = 1.0f;
    // -------------------------------------------------------------------------------------
    private bool warnedAboutMissingMasterVolumeParameter;
    private bool warnedAboutMissingVOVolumeParameter;
    private bool warnedAboutMissingMusicVolumeParameter;
    private bool warnedAboutMissingSoundEffectsVolumeParameter;

    private void Awake()
    {
        // set up singleton
        // now any class can get SettingsManager.Instance.MouseSensKey, ... etc
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // MAKES SETTINGSMANAGER PERSIST THROUGH SCENES, YOU DO NOT NEED TO SPAWN A SETTINGS MANAGER EVERY SCENE
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    private void Start()
    {
        if (generalAudioMixer != null)
        {
            if (generalAudioMultiplier <= 0.0f)
            {
                if (!generalAudioMixer.SetFloat(MasterVolumeParameter, MutedVolumeDb) && !warnedAboutMissingMasterVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{MasterVolumeParameter}'.", this);
                    warnedAboutMissingMasterVolumeParameter = true;
                }
            }
            else
            {
                if (!generalAudioMixer.SetFloat(MasterVolumeParameter, Mathf.Log10(generalAudioMultiplier) * 20.0f) && !warnedAboutMissingMasterVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{MasterVolumeParameter}'.", this);
                    warnedAboutMissingMasterVolumeParameter = true;
                }
            }

            if (voAudioMultiplier <= 0.0f)
            {
                if (!generalAudioMixer.SetFloat(VOVolumeParameter, MutedVolumeDb) && !warnedAboutMissingVOVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{VOVolumeParameter}'.", this);
                    warnedAboutMissingVOVolumeParameter = true;
                }
            }
            else
            {
                if (!generalAudioMixer.SetFloat(VOVolumeParameter, Mathf.Log10(voAudioMultiplier) * 20.0f) && !warnedAboutMissingVOVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{VOVolumeParameter}'.", this);
                    warnedAboutMissingVOVolumeParameter = true;
                }
            }

            if (musicAudioMultiplier <= 0.0f)
            {
                if (!generalAudioMixer.SetFloat(MusicVolumeParameter, MutedVolumeDb) && !warnedAboutMissingMusicVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{MusicVolumeParameter}'.", this);
                    warnedAboutMissingMusicVolumeParameter = true;
                }
            }
            else
            {
                if (!generalAudioMixer.SetFloat(MusicVolumeParameter, Mathf.Log10(musicAudioMultiplier) * 20.0f) && !warnedAboutMissingMusicVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{MusicVolumeParameter}'.", this);
                    warnedAboutMissingMusicVolumeParameter = true;
                }
            }

            if (soundEffectsAudioMultiplier <= 0.0f)
            {
                if (!generalAudioMixer.SetFloat(SoundEffectsVolumeParameter, MutedVolumeDb) && !warnedAboutMissingSoundEffectsVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{SoundEffectsVolumeParameter}'.", this);
                    warnedAboutMissingSoundEffectsVolumeParameter = true;
                }
            }
            else
            {
                if (!generalAudioMixer.SetFloat(SoundEffectsVolumeParameter, Mathf.Log10(soundEffectsAudioMultiplier) * 20.0f) && !warnedAboutMissingSoundEffectsVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{SoundEffectsVolumeParameter}'.", this);
                    warnedAboutMissingSoundEffectsVolumeParameter = true;
                }
            }
        }
    }

    public void LoadSettings()
    {
        mouseSens = PlayerPrefs.GetFloat(MouseSensKey, mouseSens);
        controllerSens = PlayerPrefs.GetFloat(ControllerSensKey, controllerSens);
        generalAudioMultiplier = Mathf.Clamp01(PlayerPrefs.GetFloat(GeneralAudioKey, generalAudioMultiplier));
        voAudioMultiplier = Mathf.Clamp01(PlayerPrefs.GetFloat(VOAudioKey, voAudioMultiplier));
        musicAudioMultiplier = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicAudioKey, musicAudioMultiplier));
        soundEffectsAudioMultiplier = Mathf.Clamp01(PlayerPrefs.GetFloat(SoundEffectsAudioKey, soundEffectsAudioMultiplier));

        SettingsChanged?.Invoke(GetSettingsData());
    }

    // Saved in Unity Cache PlayerPrefs
    public void SaveSettings(SettingsData settings)
    {
        mouseSens = settings.mouseSens;
        controllerSens = settings.controllerSens;
        generalAudioMultiplier = Mathf.Clamp01(settings.generalAudioMultiplier);
        voAudioMultiplier = Mathf.Clamp01(settings.voAudioMultiplier);
        musicAudioMultiplier = Mathf.Clamp01(settings.musicAudioMultiplier);
        soundEffectsAudioMultiplier = Mathf.Clamp01(settings.soundEffectsAudioMultiplier);

        PlayerPrefs.SetFloat(MouseSensKey, mouseSens);
        PlayerPrefs.SetFloat(ControllerSensKey, controllerSens);
        PlayerPrefs.SetFloat(GeneralAudioKey, generalAudioMultiplier);
        PlayerPrefs.SetFloat(VOAudioKey, voAudioMultiplier);
        PlayerPrefs.SetFloat(MusicAudioKey, musicAudioMultiplier);
        PlayerPrefs.SetFloat(SoundEffectsAudioKey, soundEffectsAudioMultiplier);
        PlayerPrefs.Save();
        
        if (generalAudioMixer != null)
        {
            if (generalAudioMultiplier <= 0.0f)
            {
                if (!generalAudioMixer.SetFloat(MasterVolumeParameter, MutedVolumeDb) && !warnedAboutMissingMasterVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{MasterVolumeParameter}'.", this);
                    warnedAboutMissingMasterVolumeParameter = true;
                }
            }
            else
            {
                if (!generalAudioMixer.SetFloat(MasterVolumeParameter, Mathf.Log10(generalAudioMultiplier) * 20.0f) && !warnedAboutMissingMasterVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{MasterVolumeParameter}'.", this);
                    warnedAboutMissingMasterVolumeParameter = true;
                }
            }

            if (voAudioMultiplier <= 0.0f)
            {
                if (!generalAudioMixer.SetFloat(VOVolumeParameter, MutedVolumeDb) && !warnedAboutMissingVOVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{VOVolumeParameter}'.", this);
                    warnedAboutMissingVOVolumeParameter = true;
                }
            }
            else
            {
                if (!generalAudioMixer.SetFloat(VOVolumeParameter, Mathf.Log10(voAudioMultiplier) * 20.0f) && !warnedAboutMissingVOVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{VOVolumeParameter}'.", this);
                    warnedAboutMissingVOVolumeParameter = true;
                }
            }

            if (musicAudioMultiplier <= 0.0f)
            {
                if (!generalAudioMixer.SetFloat(MusicVolumeParameter, MutedVolumeDb) && !warnedAboutMissingMusicVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{MusicVolumeParameter}'.", this);
                    warnedAboutMissingMusicVolumeParameter = true;
                }
            }
            else
            {
                if (!generalAudioMixer.SetFloat(MusicVolumeParameter, Mathf.Log10(musicAudioMultiplier) * 20.0f) && !warnedAboutMissingMusicVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{MusicVolumeParameter}'.", this);
                    warnedAboutMissingMusicVolumeParameter = true;
                }
            }

            if (soundEffectsAudioMultiplier <= 0.0f)
            {
                if (!generalAudioMixer.SetFloat(SoundEffectsVolumeParameter, MutedVolumeDb) && !warnedAboutMissingSoundEffectsVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{SoundEffectsVolumeParameter}'.", this);
                    warnedAboutMissingSoundEffectsVolumeParameter = true;
                }
            }
            else
            {
                if (!generalAudioMixer.SetFloat(SoundEffectsVolumeParameter, Mathf.Log10(soundEffectsAudioMultiplier) * 20.0f) && !warnedAboutMissingSoundEffectsVolumeParameter)
                {
                    Debug.LogWarning($"SettingsManager: missing mixer parameter '{SoundEffectsVolumeParameter}'.", this);
                    warnedAboutMissingSoundEffectsVolumeParameter = true;
                }
            }
        }

        SettingsChanged?.Invoke(GetSettingsData());
    }

    public SettingsData GetSettingsData()
    {
        return new SettingsData
        {
            mouseSens = mouseSens,
            controllerSens = controllerSens,
            generalAudioMultiplier = generalAudioMultiplier,
            voAudioMultiplier = voAudioMultiplier,
            musicAudioMultiplier = musicAudioMultiplier,
            soundEffectsAudioMultiplier = soundEffectsAudioMultiplier
        };
    }
}
