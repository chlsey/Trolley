using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EndingDoor : MonoBehaviour
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
        if (nearDoor)
        {
            bool canEnter = EndingDoorOrchestrator.Instance != null && EndingDoorOrchestrator.Instance.canEnter;
            ePrompt.SetActive(canEnter);
        }

        if (EndingDoorOrchestrator.Instance != null && EndingDoorOrchestrator.Instance.canEnter && nearDoor && 
            (Input.GetKeyDown(KeyCode.E) ||
            (Gamepad.current != null && Gamepad.current.buttonWest.wasReleasedThisFrame)))
        {
            VOManager.Instance.StopBackgroundMusic();
            SceneManager.LoadScene(sceneToLoad);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        nearDoor = true;

        bool canEnter = EndingDoorOrchestrator.Instance != null && EndingDoorOrchestrator.Instance.canEnter;
        ePrompt.SetActive(canEnter);
    }
    private void OnTriggerExit(Collider other)
    {
        nearDoor = false;
        ePrompt.SetActive(false);
    }
    
}