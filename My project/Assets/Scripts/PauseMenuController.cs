using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    private sealed class NavigationTargets
    {
        public VisualElement Up;
        public VisualElement Down;
        public VisualElement Left;
        public VisualElement Right;
        public bool HandleVertical;
        public bool HandleHorizontal;
    }

    private static PauseMenuController instance;

    // Assign the child GameObject that has the UIDocument component.
    public GameObject pauseMenuUI;

    private UIDocument uiDocument;
    private VisualElement root;
    private Slider volumeSlider;
    private Slider mouseSensSlider;
    private Slider controllerSensSlider;
    private Button unpauseButton;
    private Button exitButton;
    private int lastResumeFrame = -1;

    private void Awake()
    {
        // Singleton - survive scene loads, destroy duplicates.
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
        bool escapePressed = Input.GetKeyDown(KeyCode.Escape);
        bool startPressed = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;

        if (!IsPaused)
        {
            if (lastResumeFrame == Time.frameCount)
            {
                return;
            }

            if (escapePressed || startPressed)
            {
                Pause();
            }

            return;
        }

        if (startPressed)
        {
            Resume();
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
        FocusElement(volumeSlider);
    }

    private void Resume()
    {
        if (!IsPaused)
        {
            return;
        }

        IsPaused = false;
        lastResumeFrame = Time.frameCount;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        pauseMenuUI.SetActive(false);
    }

    private void WireButtons()
    {
        uiDocument = pauseMenuUI.GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        volumeSlider = root.Q<Slider>("Volume_Slider");
        mouseSensSlider = root.Q<Slider>("Mouse_Slider");
        controllerSensSlider = root.Q<Slider>("Controller_Slider");
        unpauseButton = root.Q<Button>("Unpause_Button");
        exitButton = root.Q<Button>("Exit_Button");

        ApplyCurrentSettings();

        root.UnregisterCallback<NavigationCancelEvent>(OnNavigationCancel);
        root.RegisterCallback<NavigationCancelEvent>(OnNavigationCancel);
        root.UnregisterCallback<PointerDownEvent>(OnRootPointerDown);
        root.RegisterCallback<PointerDownEvent>(OnRootPointerDown);

        if (volumeSlider != null)
        {
            volumeSlider.UnregisterCallback<ChangeEvent<float>>(ChangeVolume);
            volumeSlider.RegisterCallback<ChangeEvent<float>>(ChangeVolume);
            PrepareInteractiveElement(volumeSlider, new NavigationTargets
            {
                Up = null,
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
                Up = volumeSlider,
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
                Down = unpauseButton,
                HandleVertical = true
            });
        }

        if (unpauseButton != null)
        {
            unpauseButton.clicked -= OnClickContinue;
            unpauseButton.clicked += OnClickContinue;
            PrepareInteractiveElement(unpauseButton, new NavigationTargets
            {
                Up = controllerSensSlider,
                Down = exitButton,
                HandleVertical = true
            });
        }

        if (exitButton != null)
        {
            exitButton.clicked -= OnClickExit;
            exitButton.clicked += OnClickExit;
            PrepareInteractiveElement(exitButton, new NavigationTargets
            {
                Up = unpauseButton,
                Down = null,
                HandleVertical = true
            });
        }
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

        if (mouseSensSlider != null)
        {
            mouseSensSlider.value = settings.mouseSens;
        }

        if (controllerSensSlider != null)
        {
            controllerSensSlider.value = settings.controllerSens;
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
        if (!IsPaused)
        {
            return;
        }

        evt.PreventDefault();
        evt.StopPropagation();
        Resume();
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

        FocusElement(volumeSlider);
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

    private void OnClickContinue()
    {
        Resume();
    }

    private void OnClickExit()
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
