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

        var playButton = root.Q("Play_Button") as Button;
        var exitButton = root.Q("Exit_Button") as Button;

        // Unregister first to avoid stacking duplicate callbacks
        if (playButton != null)
        {
            playButton.UnregisterCallback<ClickEvent>(OnClickContinue);
            playButton.RegisterCallback<ClickEvent>(OnClickContinue);
        }
        if (exitButton != null)
        {
            exitButton.UnregisterCallback<ClickEvent>(OnClickExit);
            exitButton.RegisterCallback<ClickEvent>(OnClickExit);
        }
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
