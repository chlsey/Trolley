using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class CreditsController : MonoBehaviour
{
    public float creditsDuration = 30f;

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        StartCoroutine(ReturnToMenu());
    }

    void Update()
    {
        bool skip = Input.GetKeyDown(KeyCode.Tab) ||
            (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame);

        if (skip)
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(creditsDuration);
        SceneManager.LoadScene("MenuScene");
    }
}
