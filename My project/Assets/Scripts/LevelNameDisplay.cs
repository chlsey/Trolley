using UnityEngine;
using TMPro;

public class LevelNameDisplay : MonoBehaviour
{
    public TextMeshPro text;  // Assign in Inspector

    public void ShowLevelName(string levelName)
    {
        if (text == null)
        {
            Debug.LogError("LevelNameDisplay: Text reference is missing!");
            return;
        }

        text.text = levelName;
        Debug.Log("Displaying level name: " + levelName);
    }
}
