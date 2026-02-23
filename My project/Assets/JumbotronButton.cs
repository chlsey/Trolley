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
    public Animator armAnim;
    public GameObject qPrompt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nearRightButton = false;
        nearLeftButton = false;
        qPrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (nearRightButton && Input.GetKeyDown(KeyCode.Q))
        {
            armAnim.SetTrigger("PressButton");
            anim.SetTrigger("Pressed");
            jumbotronCameraAngleSwitcher.CycleMaterial();
            audioSource.PlayOneShot(clickSound);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(RightScreen)
        {
            nearRightButton = true;
        
            qPrompt.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(RightScreen)
        {
            nearRightButton = false;
            
            qPrompt.SetActive(false);
        }
    }
}
