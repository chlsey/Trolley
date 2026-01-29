using UnityEngine;

public class RedGreenLight : MonoBehaviour
{
    public Light light;

    public Color redColor = Color.red;
    public Color greenColor = Color.green;

    private bool isRed = true;

    void Start()
    {
        SetRed();
    }

    public void SetRed()
    {
        light.color = redColor;
        isRed = true;
    }

    public void SetGreen()
    {
        light.color = greenColor;
        isRed = false;
    }

    public void Toggle()
    {
        if (isRed)
            SetGreen();
        else
            SetRed();
    }
}
