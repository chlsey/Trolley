using UnityEngine;
using UnityEngine.Playables;

public class ClusterGameManager : MonoBehaviour
{
    public static ClusterGameManager Instance;
    public bool cutsceneHasPlayed = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}