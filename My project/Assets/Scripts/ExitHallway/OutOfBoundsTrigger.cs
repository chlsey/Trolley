using UnityEngine;

public class OutOfBoundsTrigger : MonoBehaviour
{
    private void OnTriggerEnter() {
        NextScene nextScene = FindObjectOfType<NextScene>();
        if (nextScene != null)
        {
            Debug.Log("next scene loading");
            nextScene.TriggerSceneChange();
        }
    }
}
