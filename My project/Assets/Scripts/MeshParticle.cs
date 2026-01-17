using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshParticle : MonoBehaviour
{
    private const int MAX_QUAD_AMT = 15000;
    private Mesh mesh;

    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    private int quadIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = new Mesh();
        vertices = new Vector3[4 * MAX_QUAD_AMT];
        uv = new Vector2[4 * MAX_QUAD_AMT];
        triangles = new int[6 * MAX_QUAD_AMT];

        AddQuad(new Vector3(0,0));
        AddQuad(new Vector3(0,2));

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;


    }

    private void AddQuad(Vector3 position)
    {
        if (quadIndex > MAX_QUAD_AMT) return; // mesh is full
        int vIndex = quadIndex * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        Vector3 quadSize = new Vector3(1,1);
        float rotation = 0f;
        vertices[vIndex0] = position + Quaternion.Euler(0,0, rotation - 180) * quadSize;
        vertices[vIndex1] = position + Quaternion.Euler(0,0, rotation - 270) * quadSize;
        vertices[vIndex2] = position + Quaternion.Euler(0,0, rotation - 0) * quadSize;
        vertices[vIndex3] = position + Quaternion.Euler(0,0, rotation - 90) * quadSize;
    
        int tIndex = quadIndex * 6;

        triangles[tIndex + 0] = vIndex0;
        triangles[tIndex + 1] = vIndex1;
        triangles[tIndex + 2] = vIndex2;

        triangles[tIndex + 3] = vIndex0;
        triangles[tIndex + 4] = vIndex2;
        triangles[tIndex + 5] = vIndex3;

        quadIndex++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
