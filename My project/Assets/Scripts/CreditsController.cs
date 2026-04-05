using UnityEngine;
using UnityEngine.SceneManagement;
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

    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(creditsDuration);
        SceneManager.LoadScene("MenuScene");
    }
}
