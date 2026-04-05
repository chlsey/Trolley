using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextScene : MonoBehaviour
{
    public string sceneToLoad;
    public RBController rBController;
    public Health health;
    private bool triggered = false;

    public void triggerSceneLoad()
    {
        StartCoroutine(LoadNextScene());
    }

   private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        TriggerSceneChange();
    }

    public void TriggerSceneChange()
    {
        if (triggered) return; 
        triggered = true;
        
        if(rBController) rBController.EnableRigidbody();
        
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        health.TriggerFadeToBlack();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneToLoad);
    }
    
}
