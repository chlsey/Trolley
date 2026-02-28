using UnityEngine;

public class LevelRatingSetup : MonoBehaviour
{
    public RatingMode ratingMode = RatingMode.Default;

    private void OnEnable()
    {
        Rating.CurrentMode = ratingMode;
        Debug.Log($"[LevelRatingSetup] Rating mode set to {ratingMode}");
    }
}
