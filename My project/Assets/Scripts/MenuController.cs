using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    // Stores the player's pet choice so GameOrchestrator can read it
    public static bool IsCat { get; private set; }

    private UIDocument uiDocument;
    private VisualElement root;
    private VisualElement mainMenu;
    private Button playButton;
    private Button settingsButton;
    private Button exitButton;
    private VisualElement settingsMenu;
    private Slider volumeSlider;
    private Slider mouseSensSlider;
    private Slider controllerSensSlider;
    private Button returnButton;
    private VisualElement petSelect;

    private void OnEnable()
    {
        // Restore cursor so menu buttons are clickable after returning from gameplay
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        // Ensure time is running (in case a pause menu or death screen froze it)
        Time.timeScale = 1f;

        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Main menu
        mainMenu = root.Q("Main_Menu");
        settingsMenu = root.Q("Settings_Menu");

        playButton = root.Q("Play_Button") as Button;
        settingsButton = root.Q("Settings_Button") as Button;
        exitButton = root.Q("Exit_Button") as Button;

        volumeSlider = root.Q("Volume_Slider") as Slider;
        mouseSensSlider = root.Q("Mouse_Slider") as Slider;
        controllerSensSlider = root.Q("Controller_Slider") as Slider;
        returnButton = root.Q("Return_Button") as Button;

        //set slider value to the current settings
        SettingsData SettingsStruct = SettingsManager.Instance.GetSettingsData();
        volumeSlider.value = SettingsStruct.generalAudioMultiplier;
        mouseSensSlider.value = SettingsStruct.mouseSens;
        controllerSensSlider.value = SettingsStruct.controllerSens;

        if (volumeSlider != null)
        {
            playButton.UnregisterCallback<ClickEvent>(ClickPlay);
            playButton.RegisterCallback<ClickEvent>(ClickPlay);
        }
        if (volumeSlider != null)
        {
            settingsButton.UnregisterCallback<ClickEvent>(ClickSettings);
            settingsButton.RegisterCallback<ClickEvent>(ClickSettings);
        }
        if (volumeSlider != null)
        {
            exitButton.UnregisterCallback<ClickEvent>(ClickExit);
            exitButton.RegisterCallback<ClickEvent>(ClickExit);
        }
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
        if (returnButton != null)
        {
            returnButton.UnregisterCallback<ClickEvent>(OnClickReturn);
            returnButton.RegisterCallback<ClickEvent>(OnClickReturn);
        }

        // // Pet selection screen
        // petSelect = root.Q("PetSelect");
        // var catButton = root.Q("Cat_Button") as Button;
        // var dogButton = root.Q("Dog_Button") as Button;

        // if (catButton != null)
        //     catButton.RegisterCallback<ClickEvent>(ClickCat);
        // if (dogButton != null)
        //     dogButton.RegisterCallback<ClickEvent>(ClickDog);

        // // Hide pet select at start
        // if (petSelect != null)
        //     petSelect.style.display = DisplayStyle.None;
    }

    private void ClickPlay(ClickEvent evt)
    {
        KillCounter.ResetKillCount();
        SceneManager.LoadScene("MainScene");
        // if (mainMenu != null)
        //     mainMenu.style.display = DisplayStyle.None;
        // if (petSelect != null)
        //     petSelect.style.display = DisplayStyle.Flex;
    }

    // private void ClickCat(ClickEvent evt)
    // {
    //     IsCat = true;
    //     SceneManager.LoadScene(1);
    // }

    // private void ClickDog(ClickEvent evt)
    // {
    //     IsCat = false;
    //     SceneManager.LoadScene(1);
    // }

    private void ClickSettings(ClickEvent evt)
    {
        settingsMenu.visible = true;
        settingsMenu.SetEnabled(true);
        mainMenu.visible = false;
        mainMenu.SetEnabled(false);
        Debug.Log("Settings Pressed");
    }

    private void ClickExit(ClickEvent evt)
    {
        Application.Quit();
        Debug.Log("Quit Game");
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
    private void OnClickReturn(ClickEvent evt)
    {
        settingsMenu.visible = false;
        settingsMenu.SetEnabled(false);
        mainMenu.visible = true;
        mainMenu.SetEnabled(true);
        Debug.Log("Return");
    }
}

