using UnityEngine;
using UnityEngine.SceneManagement;

public class SubwaySurferLvlController : MonoBehaviour
{

    bool gameHasEnded = false;

    public float restartDelay = 1f;
    public AudioClip subwaySurfereTheme;

    void Awake()
    {
        VOManager.Instance.StartBackgroundMusic(subwaySurfereTheme);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameHasEnded == false)
        {
            gameHasEnded = true;
            Debug.Log("GAME OVER");
            Invoke("Restart", restartDelay);
        }

    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}