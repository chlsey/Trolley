using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector director;
    public GameObject cutsceneCamera;
    public GameObject playerCamera;
    public MonoBehaviour playerMovement;
    public Health healthScript;

    void Start()
    {
        if (ClusterGameManager.Instance.cutsceneHasPlayed)
        {
            cutsceneCamera.SetActive(false);
            playerCamera.SetActive(true);
            playerMovement.enabled = true;
            healthScript.skipFade = false;
            return;
        }
        ClusterGameManager.Instance.cutsceneHasPlayed = true;
        healthScript.skipFade = true;
        Time.timeScale = 0f;

        director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;

        cutsceneCamera.SetActive(true);
        playerCamera.SetActive(false);

        playerMovement.enabled = false;

        director.Play();
        director.stopped += OnCutsceneEnd;
    }

    void OnCutsceneEnd(PlayableDirector pd)
    {
        Time.timeScale = 1f;

        cutsceneCamera.SetActive(false);
        playerCamera.SetActive(true);
        playerMovement.enabled = true;
    }
}