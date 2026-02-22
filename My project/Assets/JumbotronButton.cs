using UnityEngine;

public class JumbotronButton : MonoBehaviour
{
    public Animator anim;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public JumbotronCameraAngleSwitcher jumbotronCameraAngleSwitcher;
    public bool RightScreen;
    public bool LeftScreen;
    private bool nearRightButton;
    private bool nearLeftButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nearRightButton = false;
        nearLeftButton = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (nearRightButton && Input.GetKeyDown(KeyCode.Q))
        {
            anim.SetTrigger("Pressed");
            jumbotronCameraAngleSwitcher.CycleMaterial();
            audioSource.PlayOneShot(clickSound);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(RightScreen)
        {
            nearRightButton = true;
            Debug.Log("nearRightButton");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(RightScreen)
        {
            nearRightButton = false;
            Debug.Log("goneRightButton");
        }
    }
}
