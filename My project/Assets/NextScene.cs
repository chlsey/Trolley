using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextScene : MonoBehaviour
{
    public string sceneToLoad;
    public RBController rBController;
    public Health health;

    public void triggerSceneLoad()
    {
        StartCoroutine(LoadNextScene());
    }

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

    private IEnumerator LoadNextScene()
    {
        health.TriggerFadeToBlack();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneToLoad);
    }
    
}
