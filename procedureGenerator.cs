using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int chunkSize = 20;
    public int maxChunks = 3;
    public float perlinNoiseFrequency = 5f;
    public float heightVariation = 3f;
    public float mountainThreshold = 0.7f;
    public float objectSpawnChance = 0.1f;
    public float smoothing = 0.2f;
    public int subdivisions = 5;
    public int seed;
    public Transform player;
    public GameObject groundPrefab;
    public GameObject[] environmentObjects;
    public Texture2D yourTexture;

    private readonly List<GameObject> chunks = new List<GameObject>();
    private float lastX = 0;
    private float lastY = 0;

    void Start()
    {
        seed = System.DateTime.Now.Millisecond;
        Random.InitState(seed);
        GenerateInitialChunks();

    }

    void Update()
    {
        if (player.position.x > lastX - (chunkSize * 2))

        {
            GenerateChunk();
        }
        if (chunks.Count > 0 && player.position.x - chunks[0].transform.position.x > chunkSize * 2)
        {
            Destroy(chunks[0]);
            chunks.RemoveAt(0);
        }
    }

    void GenerateInitialChunks()
    {
        for (int i = 0; i < maxChunks; i++)
        {
            GenerateChunk();
        }
    }

    void GenerateChunk()
    {
        GameObject chunk = groundPrefab != null ? Instantiate(groundPrefab) : new GameObject("Chunk");
        chunk.transform.position = new Vector3(lastX, 0, 0);
        LineRenderer line = chunk.GetComponent<LineRenderer>() ?? chunk.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.loop = false;
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        Material lineMaterial = new Material(Shader.Find("Unlit/Texture"));
        lineMaterial.mainTexture = yourTexture;
        line.material = lineMaterial;

        List<Vector3> controlPoints = new List<Vector3>();
        if (chunks.Count > 0)
        {
            controlPoints.Add(new Vector3(lastX - 0.2f, lastY - 0.2f, 0));
        }

        float prevY = lastY;
        for (int i = 0; i < chunkSize; i++)
        {
            float x = lastX + i;
            float y = GenerateTerrainHeight(x, prevY);
            prevY = y;
            if (i == 0) y = lastY;
            controlPoints.Add(new Vector3(x, y, 0));

            if (Random.value < objectSpawnChance && environmentObjects.Length > 0)
            {
                int index = Random.Range(0, environmentObjects.Length);
                GameObject obj = Instantiate(environmentObjects[index]);
                obj.transform.position = new Vector3(x, y, 0);
            }
        }

        List<Vector3> smoothPoints = InterpolateCatmullRom(controlPoints);

        line.positionCount = smoothPoints.Count;
        line.SetPositions(smoothPoints.ToArray());
        float length = (smoothPoints[smoothPoints.Count - 1] - smoothPoints[0]).magnitude;
        line.material.mainTextureScale = new Vector2(length / yourTexture.width, 1);

        chunks.Add(chunk);
        lastX = controlPoints[controlPoints.Count - 1].x;
        lastY = controlPoints[controlPoints.Count - 1].y;
    }

    float GenerateTerrainHeight(float x, float prevY)
    {
        float lowFreqNoise = Mathf.PerlinNoise(x * (perlinNoiseFrequency * 0.005f) + seed * 0.01f, 0f) * 2f - 1f;
        float midFreqNoise = Mathf.PerlinNoise(x * (perlinNoiseFrequency * 0.01f) + seed * 0.01f, 0f) * 2f - 1f;
        float highFreqNoise = Mathf.PerlinNoise(x * (perlinNoiseFrequency * 0.05f) + seed * 0.01f, 0f) * 2f - 1f;
        float targetY = (lowFreqNoise * 2f + midFreqNoise + highFreqNoise * 0.1f) * heightVariation;

        if (midFreqNoise > mountainThreshold) targetY += highFreqNoise * heightVariation * 2f;
        if (midFreqNoise < -0.5f) targetY *= 0.5f;
        else if (midFreqNoise > 0.5f) targetY *= 1.5f;

        return Mathf.Lerp(prevY, targetY, Mathf.Clamp(smoothing * 0.3f, 0.01f, 0.5f));
    }

    List<Vector3> InterpolateCatmullRom(List<Vector3> controlPoints)
    {
        List<Vector3> smoothedPoints = new List<Vector3>();

        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            Vector3 p0 = i == 0 ? controlPoints[i] : controlPoints[i - 1];
            Vector3 p1 = controlPoints[i];
            Vector3 p2 = controlPoints[i + 1];
            Vector3 p3 = i + 2 < controlPoints.Count ? controlPoints[i + 2] : controlPoints[i + 1];

            for (int j = 0; j < subdivisions; j++)
            {
                float t = j / (float)subdivisions;
                Vector3 interpolatedPoint = CatmullRom(p0, p1, p2, p3, t);
                smoothedPoints.Add(interpolatedPoint);
            }
        }

        smoothedPoints.Add(controlPoints[controlPoints.Count - 1]);
        return smoothedPoints;
    }
    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3

        );
    }
}
