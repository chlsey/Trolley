using UnityEngine;

public class SubwayGameManager : MonoBehaviour
{
    public static SubwayGameManager Instance { get; private set; }

    public bool playedIntro = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}