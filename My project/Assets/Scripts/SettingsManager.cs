using System;
using UnityEngine;


public struct SettingsData
{
    public float mouseSens;
    public float controllerSens;
    public float generalAudioMultiplier;
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



    private const string GeneralAudioKey = "settings.general_audio";


    // DEFAULT SETTINGS
    // -------------------------------------------------------------------------------------
    [SerializeField] private float mouseSens = 0.1f;
    [SerializeField] private float controllerSens = 150.0f;
    [SerializeField] private float generalAudioMultiplier = 1.0f;
    // -------------------------------------------------------------------------------------

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

    public void LoadSettings()
    {
        mouseSens = PlayerPrefs.GetFloat(MouseSensKey, mouseSens);
        controllerSens = PlayerPrefs.GetFloat(ControllerSensKey, controllerSens);
        generalAudioMultiplier = PlayerPrefs.GetFloat(GeneralAudioKey, generalAudioMultiplier);

        SettingsChanged?.Invoke(GetSettingsData());
    }

    // Saved in Unity Cache PlayerPrefs
    public void SaveSettings(SettingsData settings)
    {
        mouseSens = settings.mouseSens;
        controllerSens = settings.controllerSens;
        generalAudioMultiplier = settings.generalAudioMultiplier;

        PlayerPrefs.SetFloat(MouseSensKey, mouseSens);
        PlayerPrefs.SetFloat(ControllerSensKey, controllerSens);
        PlayerPrefs.SetFloat(GeneralAudioKey, generalAudioMultiplier);
        PlayerPrefs.Save();

        SettingsChanged?.Invoke(GetSettingsData());
    }

    public SettingsData GetSettingsData()
    {
        return new SettingsData
        {
            mouseSens = mouseSens,
            controllerSens = controllerSens,
            generalAudioMultiplier = generalAudioMultiplier
        };
    }
}
