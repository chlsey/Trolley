using UnityEngine;

public class JumbotronCameraAngleSwitcher : MonoBehaviour
{
    public Material[] materials;   
    private int currentIndex = 0;
    private Renderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<Renderer>();

        if (materials.Length > 0)
        {
            meshRenderer.material = materials[0];
        }
    }

    void Update()
    {

    }

    public void CycleMaterial()
    {
        currentIndex++;
        if (currentIndex >= materials.Length)
        {
            currentIndex = 0;
        }

        meshRenderer.material = materials[currentIndex];
    }
}

