using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] TrolleyMovement TrolleyMovement;
    private bool playerNear;
    public Animator animator;

    // Update is called once per frame

    void Update()
    {
        if (playerNear == true && Input.GetKeyDown(KeyCode.E))
        {
                Debug.Log("lever!");
                TrolleyMovement.SwitchTrack();
                animator.SetTrigger("Lever");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        playerNear = false;
    }
}
