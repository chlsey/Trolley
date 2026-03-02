using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "IntroAudioHook", menuName = "Trolley/Level Node Hooks/introaudio hook")]
public class IntroAudioHook : LevelNodeHook
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private float durationSeconds;

    public override IEnumerator Execute(LevelNode node, GameState state)
    {
        if (VOManager.Instance != null && clip != null)
        {
            VOManager.Instance.PlayLine(clip);
        }

        yield return new WaitForSeconds(durationSeconds);
    }
}
