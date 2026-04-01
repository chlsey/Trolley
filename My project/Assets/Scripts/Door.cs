using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    public string sceneToLoad;
    private bool nearDoor;
    private bool isLocked;
    public GameObject ePrompt;

    void Awake()
    {
        SetPromptVisible(false);
    }

    void Start()
    {
        nearDoor = false;
        SetPromptVisible(false);
    }
    void Update()
    {
        if(!isLocked && nearDoor && (Input.GetKeyDown(KeyCode.E) ||
           (Gamepad.current != null && Gamepad.current.buttonWest.wasReleasedThisFrame)))
        {
            VOManager.Instance.StopBackgroundMusic();
            SceneManager.LoadScene(sceneToLoad);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        nearDoor = true;
        if (!isLocked)
        {
            SetPromptVisible(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        nearDoor = false;
        SetPromptVisible(false);
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        SetPromptVisible(!isLocked && nearDoor);
    }

    private void SetPromptVisible(bool visible)
    {
        if (ePrompt != null)
        {
            ePrompt.SetActive(visible);
        }
    }
    
}
