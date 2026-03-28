using UnityEngine;
using TMPro;

public class KillCounter : MonoBehaviour
{
    public TextMeshPro text;  // Assign in Inspector
    public static int killCount = 0;

    // Resets every time Unity enters Play mode (handles editor Play button)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetOnPlay()
    {
        killCount = 0;
    }

    public static void ResetKillCount()
    {
        killCount = 0;
    }

    void Start()
    {
        text.text = "KILLS: " + killCount;
    }

    public static void AddKill()
    {
        killCount++;
        var instance = FindObjectOfType<KillCounter>();
        if (instance != null)
            instance.text.text = "KILLS: " + killCount;

        Debug.Log(killCount);
    }
}
