using UnityEngine;

// IsCat is set by the preceding level (e.g. LevelTwoATrackA) via GameState.
// IsCat = false  → cat appears on Track A, dog appears on Track B
// IsCat = true   → cat appears on Track B, dog appears on Track A
public class Level3Setup : MonoBehaviour, IStageSetReceiver
{
    [Header("Cat Placement (assign in Inspector)")]
    public GameObject catOnTrackA;
    public GameObject catOnTrackB;

    [Header("Dog Placement (assign in Inspector)")]
    public GameObject dogOnTrackA;
    public GameObject dogOnTrackB;

    public void Initialize(GameState state)
    {
        bool isCat = state.IsCat;

        // Cat: active on the track determined by IsCat
        if (catOnTrackA != null)
            catOnTrackA.SetActive(!isCat);

        if (catOnTrackB != null)
            catOnTrackB.SetActive(isCat);

        // Dog: active on the opposite track from cat
        if (dogOnTrackA != null)
            dogOnTrackA.SetActive(isCat);

        if (dogOnTrackB != null)
            dogOnTrackB.SetActive(!isCat);
    }
}
