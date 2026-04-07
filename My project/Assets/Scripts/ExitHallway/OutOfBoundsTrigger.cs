using UnityEngine;
using System.Collections;

public class OutOfBoundsTrigger : MonoBehaviour
{
    private void OnTriggerEnter()
    {
        StartCoroutine(WaitForAudioThenTransition());
    }

    private IEnumerator WaitForAudioThenTransition()
    {
        // Wait until VOManager's voice line finishes playing
        if (VOManager.Instance != null && VOManager.Instance.voiceSource != null)
        {
            yield return new WaitWhile(() => VOManager.Instance.voiceSource.isPlaying);
        }

        NextScene nextScene = FindObjectOfType<NextScene>();
        if (nextScene != null)
        {
            StopAllCoroutines();
            Debug.Log("next scene loading");
            nextScene.TriggerSceneChange();
        }
    }
}
