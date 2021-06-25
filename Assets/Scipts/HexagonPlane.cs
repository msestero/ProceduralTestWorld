using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonPlane : MonoBehaviour
{
    public int edgeVertexCount;
    public float xOffset;
    public float yOffset;
    public float edgeLength;
    public float mapHeight;
    public float perlinScale;

    MeshFilter meshFilter;
    Transform tf;
    MeshCollider meshCollider;

    private float angleDst;

    public void Start()
    {
        angleDst = Mathf.Sin(Mathf.PI / 3f);
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
        yOffset += 0.1f;
        meshFilter.mesh = updateMesh();
    }

    public Mesh updateMesh()
    {
        Mesh mesh = new Mesh();

        checkVariables();

        mesh.vertices = getVertices();
        mesh.triangles = getTris();
        mesh.RecalculateNormals();
        mesh.uv = getUVs(mesh.vertices);
        meshCollider.sharedMesh = mesh;
        return mesh;
    }

    private void checkVariables()
    {
        if (edgeVertexCount < 2)
        {
            edgeVertexCount = 2;
        }

        if (edgeLength < 0.1f)
        {
            edgeLength = 0.1f;
        }
    }

    private float getHeight(float xPos, float yPos)
    {
        float height = Mathf.PerlinNoise((xPos + xOffset) / perlinScale, (yPos + yOffset) / perlinScale);
        return height * mapHeight;
    }

    private Vector3[] getVertices()
    {
        int numVertices = (edgeVertexCount - 1) * (3 * edgeVertexCount - 2) + (2 * edgeVertexCount - 1);
        Vector3[] vertices = new Vector3[numVertices];
        int vertexIndex = 0;
        int lineLength = edgeVertexCount;
        float xPos;
        float yPos;
        for (int y = 0; y < edgeVertexCount - 1; y++)
        {
            xPos = -(float)(edgeVertexCount - 1 + y) * (float)edgeLength / 2f;
            yPos = -angleDst * (float)(edgeVertexCount - 1 - y) * (float)edgeLength;
            for (int x = 0; x < lineLength; x++)
            {
                vertices[vertexIndex] = new Vector3(xPos, getHeight(xPos, yPos), yPos);
                vertices[numVertices - vertexIndex - 1] = new Vector3(-xPos, getHeight(-xPos, -yPos), -yPos);
                xPos += edgeLength;
                vertexIndex++;
            }
            lineLength++;
        }
        xPos = -(float)(lineLength - 1) * (float)edgeLength / 2f;
        for (int x = 0; x < lineLength; x++)
        {
            vertices[vertexIndex] = new Vector3(xPos, getHeight(xPos, 0), 0);
            xPos += edgeLength;
            vertexIndex++;
        }
        return vertices;
    }

    private int[] getTris()
    {
        int numVertices = (edgeVertexCount - 1) * (3 * edgeVertexCount - 2) + (2 * edgeVertexCount - 1);
        int numTriangles = 2 * (numVertices - (edgeVertexCount + 1)) + (edgeVertexCount + 1);
        int[] tris = new int[3 * numTriangles];
        int vertexIndex = 0;
        int triIndex = 0;
        int lineLength = edgeVertexCount;
        for (int y = 0; y < edgeVertexCount - 1; y++)
        {
            for (int x = 0; x < lineLength; x++)
            {
                tris[triIndex] = vertexIndex;
                tris[triIndex + 1] = vertexIndex + lineLength;
                tris[triIndex + 2] = vertexIndex + lineLength + 1;
                if (x != lineLength - 1)
                {
                    tris[triIndex + 3] = vertexIndex;
                    tris[triIndex + 4] = vertexIndex + lineLength + 1;
                    tris[triIndex + 5] = vertexIndex + 1;
                }

                tris[3 * numTriangles - triIndex - 1] = numVertices - vertexIndex - 1;
                tris[3 * numTriangles - triIndex - 2] = numVertices - vertexIndex - 1 - lineLength - 1;
                tris[3 * numTriangles - triIndex - 3] = numVertices - vertexIndex - 1 - lineLength;
                if (x != lineLength - 1)
                {
                    tris[3 * numTriangles - triIndex - 4] = numVertices - vertexIndex - 1;
                    tris[3 * numTriangles - triIndex - 5] = numVertices - vertexIndex - 1 - 1;
                    tris[3 * numTriangles - triIndex - 6] = numVertices - vertexIndex - 1 - lineLength - 1;
                }

                triIndex += 6;
                vertexIndex++;
            }
            lineLength++;
        }

        return tris;
    }

    private Vector2[] getUVs(Vector3[] vertices)
    {
        int numVertices = (edgeVertexCount - 1) * (3 * edgeVertexCount - 2) + (2 * edgeVertexCount - 1);
        Vector2[] uv = new Vector2[numVertices];
        for(int i = 0; i < numVertices; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        return uv;
    }
}
