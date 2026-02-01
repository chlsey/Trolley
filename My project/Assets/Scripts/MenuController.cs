using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.MessageBox;

public class MenuController : MonoBehaviour
{
    private Button playButton;
    private Button settingsButton;
    private Button exitButton;


    //Add logic that interacts with the UI controls in the `OnEnable` methods
    private void OnEnable()
    {

        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        playButton = uiDocument.rootVisualElement.Q("Play_Button") as Button;
        settingsButton = uiDocument.rootVisualElement.Q("Settings_Button") as Button;
        exitButton = uiDocument.rootVisualElement.Q("Exit_Button") as Button;

        playButton.RegisterCallback<ClickEvent>(ClickPlay);
        exitButton.RegisterCallback<ClickEvent>(ClickExit);
    }

    private void ClickPlay(ClickEvent evt)
    {
        SceneManager.LoadScene(1);
    }

    private void ClickExit(ClickEvent evt)
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

}

