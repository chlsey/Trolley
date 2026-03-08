using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SettingsManager : MonoBehaviour
{
    private const string MouseSensKey = "settings.mouse_sens";
    private const string ControllerSensKey = "settings.controller_sens";
    private const string GeneralAudioKey = "settings.general_audio";

    public struct SettingsData
    {
        public float mouseSens;
        public float controllerSens;
        public float generalAudio;
    }

    public float mouseSens = 1.0f;
    public float controllerSens = 1.0f;
    public float generalAudio = 1.0f;

    private void Awake()
    {
        LoadSettings();
    }

    public SettingsData LoadSettings()
    {
        mouseSens = PlayerPrefs.GetFloat(MouseSensKey, mouseSens);
        controllerSens = PlayerPrefs.GetFloat(ControllerSensKey, controllerSens);
        generalAudio = PlayerPrefs.GetFloat(GeneralAudioKey, generalAudio);

        return new SettingsData
        {
            mouseSens = mouseSens,
            controllerSens = controllerSens,
            generalAudio = generalAudio
        };
    }

    public void SaveSettings(SettingsData settings)
    {
        mouseSens = settings.mouseSens;
        controllerSens = settings.controllerSens;
        generalAudio = settings.generalAudio;

        PlayerPrefs.SetFloat(MouseSensKey, mouseSens);
        PlayerPrefs.SetFloat(ControllerSensKey, controllerSens);
        PlayerPrefs.SetFloat(GeneralAudioKey, generalAudio);
        PlayerPrefs.Save();
    }
}
