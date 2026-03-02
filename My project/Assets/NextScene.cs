using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextScene : MonoBehaviour
{
    public string sceneToLoad;
    public RBController rBController;
    public Health health;

    private void OnTriggerEnter(Collider other)
    {
        rBController.EnableRigidbody();
        StartCoroutine(LoadNextScene());
    }
     private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1);
        health.TriggerFadeToBlack();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneToLoad);
    }
}
