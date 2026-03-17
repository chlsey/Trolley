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

        // Pages
        var mainPage = root.Q("MainPage");
        var settingsPage = root.Q("SettingsPage");

        // Always show main page first, hide settings
        mainPage.style.display = DisplayStyle.Flex;
        settingsPage.style.display = DisplayStyle.None;

        // Main page buttons (same names as MainMenu.uxml)
        var playButton = root.Q("Play_Button") as Button;
        var settingsButton = root.Q("Settings_Button") as Button;
        var exitButton = root.Q("Exit_Button") as Button;

        // Settings page elements
        var volumeSlider = root.Q("Volume_Slider") as Slider;
        var mouseSensSlider = root.Q("Mouse_Slider") as Slider;
        var controllerSensSlider = root.Q("Controller_Slider") as Slider;
        var backButton = root.Q("Back_Button") as Button;

        // Load current settings into sliders
        SettingsData settings = SettingsManager.Instance.GetSettingsData();
        if (volumeSlider != null) volumeSlider.value = settings.generalAudioMultiplier;
        if (mouseSensSlider != null) mouseSensSlider.value = settings.mouseSens;
        if (controllerSensSlider != null) controllerSensSlider.value = settings.controllerSens;

        // Wire main page buttons
        if (playButton != null)
        {
            playButton.UnregisterCallback<ClickEvent>(OnClickContinue);
            playButton.RegisterCallback<ClickEvent>(OnClickContinue);
        }
        if (settingsButton != null)
        {
            settingsButton.UnregisterCallback<ClickEvent>(OnClickSettings);
            settingsButton.RegisterCallback<ClickEvent>(OnClickSettings);
        }
        if (exitButton != null)
        {
            exitButton.UnregisterCallback<ClickEvent>(OnClickExit);
            exitButton.RegisterCallback<ClickEvent>(OnClickExit);
        }

        // Wire settings page
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
        if (backButton != null)
        {
            backButton.UnregisterCallback<ClickEvent>(OnClickBack);
            backButton.RegisterCallback<ClickEvent>(OnClickBack);
        }
    }

    private void OnClickContinue(ClickEvent evt)
    {
        Resume();
    }

    private void OnClickSettings(ClickEvent evt)
    {
        var uiDoc = pauseMenuUI.GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;
        root.Q("MainPage").style.display = DisplayStyle.None;
        root.Q("SettingsPage").style.display = DisplayStyle.Flex;
    }

    private void OnClickBack(ClickEvent evt)
    {
        var uiDoc = pauseMenuUI.GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;
        root.Q("SettingsPage").style.display = DisplayStyle.None;
        root.Q("MainPage").style.display = DisplayStyle.Flex;
    }

    private void OnClickExit(ClickEvent evt)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ChangeVolume(ChangeEvent<float> evt)
    {
        SettingsData settings = SettingsManager.Instance.GetSettingsData();
        settings.generalAudioMultiplier = evt.newValue;
        SettingsManager.Instance.SaveSettings(settings);
    }

    private void ChangeMouseSens(ChangeEvent<float> evt)
    {
        SettingsData settings = SettingsManager.Instance.GetSettingsData();
        settings.mouseSens = evt.newValue;
        SettingsManager.Instance.SaveSettings(settings);
    }

    private void ChangeControllerSens(ChangeEvent<float> evt)
    {
        SettingsData settings = SettingsManager.Instance.GetSettingsData();
        settings.controllerSens = evt.newValue;
        SettingsManager.Instance.SaveSettings(settings);
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
