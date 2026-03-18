using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    private static PauseMenuController instance;

    // Assign the child GameObject that has the UIDocument component
    public GameObject pauseMenuUI;

    private void Awake()
    {
        // Singleton — survive scene loads, destroy duplicates
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    private void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        pauseMenuUI.SetActive(true);
        WireButtons();
    }

    private void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        pauseMenuUI.SetActive(false);
    }

    private void WireButtons()
    {
        var uiDoc = pauseMenuUI.GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        var volumeSlider = root.Q("Volume_Slider") as Slider; 
        var mouseSensSlider = root.Q("Mouse_Slider") as Slider;
        var controllerSensSlider = root.Q("Controller_Slider") as Slider;
        var unpauseButton = root.Q("Unpause_Button") as Button;
        var exitButton = root.Q("Exit_Button") as Button;

        //This was the simplest way I could get the sliders to keep their positions when closing and reopening the menu
        SettingsData SettingsStruct = SettingsManager.Instance.GetSettingsData();
        volumeSlider.value = SettingsStruct.generalAudioMultiplier;
        mouseSensSlider.value = SettingsStruct.mouseSens;
        controllerSensSlider.value = SettingsStruct.controllerSens;

        // Unregister first to avoid stacking duplicate callbacks
        if (volumeSlider != null)
        {
            volumeSlider.UnregisterCallback<ChangeEvent<float>>(ChangeVolume);
            volumeSlider.RegisterCallback<ChangeEvent<float>>(ChangeVolume);
        }
        if (mouseSensSlider != null)
        {
            mouseSensSlider.UnregisterCallback<ChangeEvent<float>>(ChangeMouseSens);
            mouseSensSlider.RegisterCallback<ChangeEvent<float>>(ChangeMouseSens);
        }
        if (controllerSensSlider != null)
        {
            controllerSensSlider.UnregisterCallback<ChangeEvent<float>>(ChangeControllerSens);
            controllerSensSlider.RegisterCallback<ChangeEvent<float>>(ChangeControllerSens);
        }
        if (unpauseButton != null)
        {
            unpauseButton.UnregisterCallback<ClickEvent>(OnClickContinue);
            unpauseButton.RegisterCallback<ClickEvent>(OnClickContinue);
        }
        if (exitButton != null)
        {
            exitButton.UnregisterCallback<ClickEvent>(OnClickExit);
            exitButton.RegisterCallback<ClickEvent>(OnClickExit);
        }
    }

    private void ChangeVolume(ChangeEvent<float> evt)
    {
        SettingsData SettingsStruct = SettingsManager.Instance.GetSettingsData();
        SettingsStruct.generalAudioMultiplier = evt.newValue;
        SettingsManager.Instance.SaveSettings(SettingsStruct);
        
    }

    private void ChangeMouseSens(ChangeEvent<float> evt)
    {
        SettingsData SettingsStruct = SettingsManager.Instance.GetSettingsData();
        SettingsStruct.mouseSens = evt.newValue;
        SettingsManager.Instance.SaveSettings(SettingsStruct);
    }

    private void ChangeControllerSens(ChangeEvent<float> evt)
    {
        SettingsData SettingsStruct = SettingsManager.Instance.GetSettingsData();
        SettingsStruct.controllerSens = evt.newValue;
        SettingsManager.Instance.SaveSettings(SettingsStruct);
    }

    private void OnClickContinue(ClickEvent evt)
    {
        Resume();
    }

    private void OnClickExit(ClickEvent evt)
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        IsPaused = false;
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (IsPaused)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            IsPaused = false;
        }
    }
}
