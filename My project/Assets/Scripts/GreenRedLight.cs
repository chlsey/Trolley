using UnityEngine;

public class GreenRedLight : MonoBehaviour
{
    public Light light;

    public Color redColor = Color.red;
    public Color greenColor = Color.green;

    private bool isRed = true;

    void Start()
    {
        SetGreen();
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
