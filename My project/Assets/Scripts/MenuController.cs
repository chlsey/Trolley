using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    // Stores the player's pet choice so GameOrchestrator can read it
    public static bool IsCat { get; private set; }

    private sealed class NavigationTargets
    {
        public VisualElement Up;
        public VisualElement Down;
        public VisualElement Left;
        public VisualElement Right;
        public bool HandleVertical;
        public bool HandleHorizontal;
    }

    private UIDocument uiDocument;
    private VisualElement root;
    private VisualElement mainMenu;
    private VisualElement settingsMenu;
    private Button playButton;
    private Button settingsButton;
    private Button exitButton;
    private Slider volumeSlider;
    private Slider voSlider;
    private Slider musicSlider;
    private Slider soundEffectsSlider;
    private Slider mouseSensSlider;
    private Slider controllerSensSlider;
    private Button returnButton;

    private void OnEnable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        Time.timeScale = 1f;

        QueryElements();
        ApplyCurrentSettings();
        WireMenu();
        ShowMainMenu(playButton);
    }

    private void QueryElements()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        mainMenu = root.Q<VisualElement>("Main_Menu");
        settingsMenu = root.Q<VisualElement>("Settings_Menu");

        playButton = root.Q<Button>("Play_Button");
        settingsButton = root.Q<Button>("Settings_Button");
        exitButton = root.Q<Button>("Exit_Button");

        volumeSlider = root.Q<Slider>("Volume_Slider");
        voSlider = root.Q<Slider>("VO_Slider");
        musicSlider = root.Q<Slider>("Music_Slider");
        soundEffectsSlider = root.Q<Slider>("SoundEffects_Slider");
        mouseSensSlider = root.Q<Slider>("Mouse_Slider");
        controllerSensSlider = root.Q<Slider>("Controller_Slider");
        returnButton = root.Q<Button>("Return_Button");
    }

    private void ApplyCurrentSettings()
    {
        if (SettingsManager.Instance == null)
        {
            return;
        }

        SettingsData settings = SettingsManager.Instance.GetSettingsData();

        if (volumeSlider != null)
        {
            volumeSlider.value = settings.generalAudioMultiplier;
        }

        if (voSlider != null)
        {
            voSlider.value = settings.voAudioMultiplier;
        }

        if (musicSlider != null)
        {
            musicSlider.value = settings.musicAudioMultiplier;
        }

        if (soundEffectsSlider != null)
        {
            soundEffectsSlider.value = settings.soundEffectsAudioMultiplier;
        }

        if (mouseSensSlider != null)
        {
            mouseSensSlider.value = settings.mouseSens;
        }

        if (controllerSensSlider != null)
        {
            controllerSensSlider.value = settings.controllerSens;
        }
    }

    private void WireMenu()
    {
        if (root != null)
        {
            root.UnregisterCallback<NavigationCancelEvent>(OnNavigationCancel);
            root.RegisterCallback<NavigationCancelEvent>(OnNavigationCancel);
            root.UnregisterCallback<PointerDownEvent>(OnRootPointerDown);
            root.RegisterCallback<PointerDownEvent>(OnRootPointerDown);
        }

        if (playButton != null)
        {
            playButton.clicked -= ClickPlay;
            playButton.clicked += ClickPlay;
            PrepareInteractiveElement(playButton, new NavigationTargets
            {
                Left = null,
                Right = settingsButton,
                HandleHorizontal = true
            });
        }

        if (settingsButton != null)
        {
            settingsButton.clicked -= ClickSettings;
            settingsButton.clicked += ClickSettings;
            PrepareInteractiveElement(settingsButton, new NavigationTargets
            {
                Left = playButton,
                Right = exitButton,
                HandleHorizontal = true
            });
        }

        if (exitButton != null)
        {
            exitButton.clicked -= ClickExit;
            exitButton.clicked += ClickExit;
            PrepareInteractiveElement(exitButton, new NavigationTargets
            {
                Left = settingsButton,
                Right = null,
                HandleHorizontal = true
            });
        }

        if (volumeSlider != null)
        {
            volumeSlider.UnregisterCallback<ChangeEvent<float>>(ChangeVolume);
            volumeSlider.RegisterCallback<ChangeEvent<float>>(ChangeVolume);
            PrepareInteractiveElement(volumeSlider, new NavigationTargets
            {
                Up = null,
                Down = voSlider,
                HandleVertical = true
            });
        }

        if (voSlider != null)
        {
            voSlider.UnregisterCallback<ChangeEvent<float>>(ChangeVOVolume);
            voSlider.RegisterCallback<ChangeEvent<float>>(ChangeVOVolume);
            PrepareInteractiveElement(voSlider, new NavigationTargets
            {
                Up = volumeSlider,
                Down = musicSlider,
                HandleVertical = true
            });
        }

        if (musicSlider != null)
        {
            musicSlider.UnregisterCallback<ChangeEvent<float>>(ChangeMusicVolume);
            musicSlider.RegisterCallback<ChangeEvent<float>>(ChangeMusicVolume);
            PrepareInteractiveElement(musicSlider, new NavigationTargets
            {
                Up = voSlider,
                Down = soundEffectsSlider,
                HandleVertical = true
            });
        }

        if (soundEffectsSlider != null)
        {
            soundEffectsSlider.UnregisterCallback<ChangeEvent<float>>(ChangeSoundEffectsVolume);
            soundEffectsSlider.RegisterCallback<ChangeEvent<float>>(ChangeSoundEffectsVolume);
            PrepareInteractiveElement(soundEffectsSlider, new NavigationTargets
            {
                Up = musicSlider,
                Down = mouseSensSlider,
                HandleVertical = true
            });
        }

        if (mouseSensSlider != null)
        {
            mouseSensSlider.UnregisterCallback<ChangeEvent<float>>(ChangeMouseSens);
            mouseSensSlider.RegisterCallback<ChangeEvent<float>>(ChangeMouseSens);
            PrepareInteractiveElement(mouseSensSlider, new NavigationTargets
            {
                Up = soundEffectsSlider,
                Down = controllerSensSlider,
                HandleVertical = true
            });
        }

        if (controllerSensSlider != null)
        {
            controllerSensSlider.UnregisterCallback<ChangeEvent<float>>(ChangeControllerSens);
            controllerSensSlider.RegisterCallback<ChangeEvent<float>>(ChangeControllerSens);
            PrepareInteractiveElement(controllerSensSlider, new NavigationTargets
            {
                Up = mouseSensSlider,
                Down = returnButton,
                HandleVertical = true
            });
        }

        if (returnButton != null)
        {
            returnButton.clicked -= OnClickReturn;
            returnButton.clicked += OnClickReturn;
            PrepareInteractiveElement(returnButton, new NavigationTargets
            {
                Up = controllerSensSlider,
                Down = null,
                HandleVertical = true
            });
        }
    }

    private void PrepareInteractiveElement(VisualElement element, NavigationTargets navigationTargets)
    {
        if (element == null)
        {
            return;
        }

        element.focusable = true;
        element.userData = navigationTargets;
        element.UnregisterCallback<PointerEnterEvent>(OnPointerEnterFocus);
        element.RegisterCallback<PointerEnterEvent>(OnPointerEnterFocus);
        element.UnregisterCallback<NavigationMoveEvent>(OnNavigationMove);
        element.RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
    }

    private void OnPointerEnterFocus(PointerEnterEvent evt)
    {
        if (evt.currentTarget is VisualElement element)
        {
            FocusElement(element);
        }
    }

    private void OnNavigationMove(NavigationMoveEvent evt)
    {
        if (evt.currentTarget is not VisualElement current || current.userData is not NavigationTargets targets)
        {
            return;
        }

        VisualElement next = null;

        switch (evt.direction)
        {
            case NavigationMoveEvent.Direction.Up:
                if (!targets.HandleVertical)
                {
                    return;
                }

                next = targets.Up;
                break;
            case NavigationMoveEvent.Direction.Down:
                if (!targets.HandleVertical)
                {
                    return;
                }

                next = targets.Down;
                break;
            case NavigationMoveEvent.Direction.Left:
                if (!targets.HandleHorizontal)
                {
                    return;
                }

                next = targets.Left;
                break;
            case NavigationMoveEvent.Direction.Right:
                if (!targets.HandleHorizontal)
                {
                    return;
                }

                next = targets.Right;
                break;
            default:
                return;
        }

        evt.PreventDefault();
        evt.StopPropagation();

        if (next != null)
        {
            FocusElement(next);
        }
    }

    private void OnNavigationCancel(NavigationCancelEvent evt)
    {
        if (settingsMenu == null || !settingsMenu.visible)
        {
            return;
        }

        evt.PreventDefault();
        evt.StopPropagation();
        OnClickReturn();
    }

    private void OnRootPointerDown(PointerDownEvent evt)
    {
        if (evt.target is not VisualElement element)
        {
            return;
        }

        if (element.focusable || element.GetFirstAncestorOfType<Button>() != null || element.GetFirstAncestorOfType<Slider>() != null)
        {
            return;
        }

        FocusElement(settingsMenu != null && settingsMenu.visible ? volumeSlider : playButton);
    }

    private void FocusElement(VisualElement element)
    {
        if (root == null || element == null)
        {
            return;
        }

        root.schedule.Execute(() =>
        {
            if (element.panel != null && element.enabledInHierarchy && element.visible)
            {
                element.Focus();
            }
        }).ExecuteLater(0);
    }

    private void ShowMainMenu(VisualElement focusTarget)
    {
        if (settingsMenu != null)
        {
            settingsMenu.visible = false;
            settingsMenu.SetEnabled(false);
        }

        if (mainMenu != null)
        {
            mainMenu.visible = true;
            mainMenu.SetEnabled(true);
        }

        FocusElement(focusTarget);
    }

    private void ShowSettingsMenu()
    {
        if (settingsMenu != null)
        {
            settingsMenu.visible = true;
            settingsMenu.SetEnabled(true);
        }

        if (mainMenu != null)
        {
            mainMenu.visible = false;
            mainMenu.SetEnabled(false);
        }

        FocusElement(volumeSlider);
    }

    private void ClickPlay()
    {
        KillCounter.ResetKillCount();
        SceneManager.LoadScene("MainScene");
    }

    private void ClickSettings()
    {
        ShowSettingsMenu();
        Debug.Log("Settings Pressed");
    }

    private void ClickExit()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    private void ChangeVolume(ChangeEvent<float> evt)
    {
        SettingsData settings = SettingsManager.Instance.GetSettingsData();
        settings.generalAudioMultiplier = evt.newValue;
        SettingsManager.Instance.SaveSettings(settings);
    }

    private void ChangeVOVolume(ChangeEvent<float> evt)
    {
        SettingsData settings = SettingsManager.Instance.GetSettingsData();
        settings.voAudioMultiplier = evt.newValue;
        SettingsManager.Instance.SaveSettings(settings);
    }

    private void ChangeMusicVolume(ChangeEvent<float> evt)
    {
        SettingsData settings = SettingsManager.Instance.GetSettingsData();
        settings.musicAudioMultiplier = evt.newValue;
        SettingsManager.Instance.SaveSettings(settings);
    }

    private void ChangeSoundEffectsVolume(ChangeEvent<float> evt)
    {
        SettingsData settings = SettingsManager.Instance.GetSettingsData();
        settings.soundEffectsAudioMultiplier = evt.newValue;
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

    private void OnClickReturn()
    {
        ShowMainMenu(settingsButton);
        Debug.Log("Return");
    }
}
