using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.MessageBox;

public class MenuController : MonoBehaviour
{
    private Button _button;
    

    //Add logic that interacts with the UI controls in the `OnEnable` methods
    private void OnEnable()
    {

        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        _button = uiDocument.rootVisualElement.Q("button") as Button;

        _button.RegisterCallback<ClickEvent>(ClickPlay);
        _button.RegisterCallback<MouseEnterEvent>(MouseHovering);
        _button.RegisterCallback<MouseOutEvent>(MouseLeft);
    }

    private void ClickPlay(ClickEvent evt)
    {
        SceneManager.LoadScene(1);
    }
    private void MouseHovering(MouseEnterEvent evt)
    {
        _button.style.backgroundColor = new Color(0f, 1f, 1f, 0.5f);
    }

    private void MouseLeft(MouseOutEvent evt)
    {
        _button.style.backgroundColor = new Color(1f, 1f, 1f, 0f);
    }
}

