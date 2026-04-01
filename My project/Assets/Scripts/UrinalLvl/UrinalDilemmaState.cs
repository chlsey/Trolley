using UnityEngine;

public class UrinalDilemmaState : MonoBehaviour
{
    public Door exitDoor;
    public Urinal[] urinals;
    public StallDoor[] stalls;

    public bool HasUrinalSelection { get; private set; }

    private Urinal selectedUrinal;

    void Start()
    {
        if (exitDoor != null)
        {
            exitDoor.SetLocked(true);
        }

        HideAllOptionPrompts();
    }

    public bool TryCommitUrinal(Urinal source)
    {
        if (HasUrinalSelection)
        {
            return false;
        }

        selectedUrinal = source;
        HasUrinalSelection = true;
        HideAllOptionPrompts();
        SetAllStallInteractionsEnabled(false);
        return true;
    }

    public void UnlockExit()
    {
        if (exitDoor != null)
        {
            exitDoor.SetLocked(false);
        }
    }

    private void HideAllOptionPrompts()
    {
        foreach (Urinal urinal in urinals)
        {
            if (urinal != null)
            {
                urinal.HidePrompt();
            }
        }

        foreach (StallDoor stall in stalls)
        {
            if (stall != null)
            {
                stall.HidePrompt();
            }
        }
    }

    private void SetAllStallInteractionsEnabled(bool enabled)
    {
        foreach (StallDoor stall in stalls)
        {
            if (stall != null)
            {
                stall.SetInteractionEnabled(enabled);
            }
        }
    }
}
