using UnityEngine;
using TMPro;

public class KillCounter : MonoBehaviour
{
    public TextMeshPro text;  // Assign in Inspector
    public static int killCount = 0;

    void Start()
    {
        text.text = "KILLS: 0";
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
