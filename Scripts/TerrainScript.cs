using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    public int xSize;
    public int ySize;

    public int pixWidth;
    public int pixHeight;

    public float xOrg;
    public float yOrg;

    public float scale;

    private Texture2D noiseTexure;
    private Color[] pixels;

    private Vector3[] vertices;
    private Mesh mesh;

    private void GenerateMesh()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                float xCoord = xOrg + (float)x / xSize * scale;
                float yCoord = yOrg + (float)y / ySize * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                vertices[i] = new Vector3(x, sample, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void CalcNoiseTexture()
    {
        noiseTexure = new Texture2D(pixWidth, pixHeight);
        pixels = new Color[noiseTexure.width * noiseTexure.height];
        GetComponent<Renderer>().material.mainTexture = noiseTexure;

        for (float y = 0.0f; y < noiseTexure.height; y++)
        {
            for (float x = 0.0f; x < noiseTexure.width; x++)
            {
                float xCoord = xOrg + x / noiseTexure.width * scale;
                float yCoord = yOrg + y / noiseTexure.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pixels[(int)y * noiseTexure.width + (int)x] = new Color(sample, sample, sample);
            }
        }

        noiseTexure.SetPixels(pixels);
        noiseTexure.Apply();
    }

    private void Start()
    {
        GenerateMesh();
        CalcNoiseTexture();
    }

    private void Update()
    {
        
    }
}
