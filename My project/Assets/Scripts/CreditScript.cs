using UnityEngine;
using UnityEngine.UI;

public class CreditScript : MonoBehaviour
{
    public float scrollSpeed = 40;

    public GameObject title;
    public GameObject subtitle;
    public GameObject credits;

    private RectTransform rectTransform;
    private RectTransform rectTransform2;
    private RectTransform rectTransform3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = title.GetComponent<RectTransform>();
        rectTransform2 = subtitle.GetComponent<RectTransform>();
        rectTransform3 = credits.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        rectTransform2.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        rectTransform3.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
    }
}
