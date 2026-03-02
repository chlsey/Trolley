using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextScene : MonoBehaviour
{
    public string sceneToLoad;
    public RBController rBController;
    public Health health;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        TriggerSceneChange();
    }

    public void TriggerSceneChange()
    {
        if (triggered) return; 
        triggered = true;

        rBController.EnableRigidbody();
        StartCoroutine(LoadNextScene());
    }

    public IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1);
        health.TriggerFadeToBlack();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneToLoad);
    }
}