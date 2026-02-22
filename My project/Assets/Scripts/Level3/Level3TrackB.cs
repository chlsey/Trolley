using UnityEngine;
using System.Collections;

public class Level3TrackB : MonoBehaviour, IStageSetReceiver
{
    public EndTrigger endTrigger;

    [Header("VO Clips")]
    public AudioClip killCatClip;
    public AudioClip catEscapesClip;
    public AudioClip killDogClip;
    public AudioClip nextLevelIntroClip;

    [Header("Cat")]
    public GameObject catObject; // the cat on Track B — assign in Inspector

    private bool _isCat; // false = cat on Track A, true = cat on Track B
    private Vector3 _catStartPos;
    private Quaternion _catStartRot;

    public void Initialize(GameState state)
    {
        _isCat = state.IsCat;
    }

    private void Start()
    {
        if (catObject != null)
        {
            _catStartPos = catObject.transform.position;
            _catStartRot = catObject.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        yield return CurtainController.Instance.CloseCurtains();

        // IsCat = false → cat was on Track A (dog on B) → trolley hit Track B → dog killed
        // IsCat = true  → cat was on Track B → trolley hit Track B → cat killed
        if (_isCat)
        {
            VOManager.Instance.PlayLine(killCatClip);
            yield return new WaitForSeconds(killCatClip.length);

            // Respawn the cat at its starting position so it can "escape"
            if (catObject != null)
            {
                catObject.SetActive(false);
                catObject.transform.position = _catStartPos;
                catObject.transform.rotation = _catStartRot;
                catObject.SetActive(true);
            }

            VOManager.Instance.PlayLine(catEscapesClip);
            yield return new WaitForSeconds(catEscapesClip.length);
        }
        else
        {
            VOManager.Instance.PlayLine(killDogClip);
            yield return new WaitForSeconds(killDogClip.length);
        }

        if (nextLevelIntroClip != null)
        {
            VOManager.Instance.PlayLine(nextLevelIntroClip);
            yield return new WaitForSeconds(nextLevelIntroClip.length);
        }

        yield return CurtainController.Instance.OpenCurtains();

        endTrigger.TriggerEnd();
    }
}