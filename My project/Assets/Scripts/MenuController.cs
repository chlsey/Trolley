using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    // Stores the player's pet choice so GameOrchestrator can read it
    public static bool IsCat { get; private set; }

    private VisualElement mainMenu;
    private VisualElement petSelect;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // Main menu
        mainMenu = root.Q("MainMenu");
        var playButton = root.Q("Play_Button") as Button;
        var exitButton = root.Q("Exit_Button") as Button;

        playButton.RegisterCallback<ClickEvent>(ClickPlay);
        exitButton.RegisterCallback<ClickEvent>(ClickExit);
        

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

    private void ClickExit(ClickEvent evt)
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}

