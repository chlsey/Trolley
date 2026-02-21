using UnityEngine;

// IsCat is set by the preceding level (e.g. LevelTwoATrackA) via GameState.
// IsCat = false  → cat appears on Track A
// IsCat = true → cat appears on Track B
public class Level3Setup : MonoBehaviour, IStageSetReceiver
{
    [Header("Cat Placement (assign in Inspector)")]
    public GameObject catOnTrackA;
    public GameObject catOnTrackB;

    public void Initialize(GameState state)
    {
        bool isCat = state.IsCat;

        if (catOnTrackA != null)
            catOnTrackA.SetActive(!isCat);

        if (catOnTrackB != null)
            catOnTrackB.SetActive(isCat);
    }
}
