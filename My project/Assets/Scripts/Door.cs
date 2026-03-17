using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    public string sceneToLoad;
    private bool nearDoor;
    public GameObject ePrompt;


    void Start()
    {
        nearDoor = false;
        ePrompt.SetActive(false);
    }
    void Update()
    {
        if(nearDoor && (Input.GetKeyDown(KeyCode.E) ||
           (Gamepad.current != null && Gamepad.current.buttonWest.wasReleasedThisFrame)))
        {
            VOManager.Instance.StopBackgroundMusic();
            SceneManager.LoadScene(sceneToLoad);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        nearDoor = true;
        ePrompt.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        nearDoor = false;
        ePrompt.SetActive(false);
    }
    
}