using UnityEngine;

public class CoasterEndingBannerController : MonoBehaviour
{
    public GameObject onStageBanner;
    public GameObject offStageBanner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onStageBanner.SetActive(false);
        
    }

    void OnTriggerEnter()
    {
        onStageBanner.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
