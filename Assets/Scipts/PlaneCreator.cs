using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCreator : MonoBehaviour
{
    public int width;
    public int height;
    public float xOffset;
    public float yOffset;
    public float planeWidth;
    public float mapHeight;
    public float perlinScale;

    MeshFilter meshFilter;
    Transform tf;
    MeshCollider meshCollider;

    public void Start()
    {
        tf = gameObject.GetComponent<Transform>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        meshCollider = gameObject.AddComponent<MeshCollider>();

        meshFilter = gameObject.AddComponent<MeshFilter>();

        meshFilter.mesh = updateMesh();
    }

    public void Update()
    {
        xOffset += 0.1f;
        meshFilter.mesh = updateMesh();
    }

    public Mesh updateMesh()
    {
        Mesh mesh = new Mesh();

        checkVariables();
        updatePosition();

        mesh.vertices = getVertices();
        mesh.triangles = getTris();
        mesh.RecalculateNormals();
        mesh.uv = getUVs();
        meshCollider.sharedMesh = mesh;
        return mesh;
    }

    private void updatePosition()
    {
        float x = -((width - 1) * planeWidth) / 2f;
        float y = -((height - 1) * planeWidth) / 2f;
        tf.position = new Vector3(x, 0, y);
    }

    private void checkVariables()
    {
        if(width < 2)
        {
            width = 2;
        }
        if(height < 2)
        {
            height = 2;
        }
        if(planeWidth < 0.1)
        {
            planeWidth = 0.1f;
        }
    }

    private float getHeight(float xPos, float yPos)
    {
        float height = Mathf.PerlinNoise(xPos / perlinScale, yPos / perlinScale);
        return height * mapHeight;
    }

    private Vector3[] getVertices()
    {
        Vector3[] vertices = new Vector3[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = x * planeWidth;
                float yPos = y * planeWidth;
                float mapHeight = getHeight(xPos + xOffset, yPos + yOffset);
                vertices[x * height + y] = new Vector3(xPos, mapHeight, yPos);
            }
        }
        return vertices;
    }

    private int[] getTris()
    {
        int[] tris = new int[6 * (width - 1) * (height - 1)];
        int indexTris = 0;
        int vertexCount = 0;
        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                tris[indexTris] = vertexCount;
                tris[indexTris + 1] = vertexCount + 1;
                tris[indexTris + 2] = vertexCount + height;
                tris[indexTris + 3] = vertexCount + height;
                tris[indexTris + 4] = vertexCount + 1;
                tris[indexTris + 5] = vertexCount + 1 + height;
                indexTris += 6;
                vertexCount += 1;
            }
            vertexCount += 1;
        }
        return tris;
    }

    private Vector2[] getUVs()
    {
        Vector2[] uv = new Vector2[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                uv[x * height + y] = new Vector2(x / (float)width, y / (float)height);
            }
        }
        return uv;
    }
}
