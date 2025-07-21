using System.Collections.Generic;
using System.Linq;
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
    public float widthUnderground = 12;
    public float offsetUnderground = -1;
    public Transform player;
    public GameObject groundPrefab;
    public GameObject[] environmentObjects;
    public Texture2D Texture;
    public Texture2D undergroundTexture;
    public LineRenderer lineRenderer;

    public readonly List<GameObject> chunks = new List<GameObject>();
    private float lastX = 0;
    private float lastY = 0;

    /*void Start()
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
                obj.transform.parent = chunk.transform;
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
    }*/
    private LevelMenager levelManager;

    private void Start()
    {
        seed = System.DateTime.Now.Millisecond;
        Random.InitState(seed);
        levelManager = FindObjectOfType<LevelMenager>();
        GenerateInitialChunks();
    }

    private void Update()
    {
        if (player == null) return;

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

    public void GenerateInitialChunks()
    {
        for (int i = 0; i < maxChunks; i++)
        {
            GenerateChunk();
        }
    }

    private void GenerateChunk()
    {
        GameObject chunk = groundPrefab != null ? Instantiate(groundPrefab) : new GameObject("Chunk");
        chunk.transform.position = new Vector3(lastX, 0, 0);

        LineRenderer line = chunk.GetComponent<LineRenderer>() ?? chunk.AddComponent<LineRenderer>();
        EdgeCollider2D collider = chunk.GetComponent<EdgeCollider2D>() ?? chunk.AddComponent<EdgeCollider2D>();
        chunk.AddComponent<ColliderLine>();

        line.useWorldSpace = true;
        line.loop = false;
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        Material lineMaterial = new Material(Shader.Find("Unlit/Texture"));
        if (Texture != null)
        {
            lineMaterial.mainTexture = Texture;
        }
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
                GameObject obj = levelManager?.GetPooledObject() ?? Instantiate(environmentObjects[index]);
                if (obj != null)
                {
                    obj.transform.position = new Vector3(x, y, 0);
                    obj.transform.parent = chunk.transform;
                    if (!obj.activeSelf)
                    {
                        obj.SetActive(true);
                        GameObject prefab = environmentObjects[index];
                        var renderer = obj.GetComponent<SpriteRenderer>();
                        var prefabRenderer = prefab.GetComponent<SpriteRenderer>();
                        if (renderer != null && prefabRenderer != null)
                        {
                            renderer.sprite = prefabRenderer.sprite;
                        }
                    }
                }
            }
        }

       List<Vector3> smoothPoints = InterpolateCatmullRom(controlPoints);

        line.positionCount = smoothPoints.Count;
        line.SetPositions(smoothPoints.ToArray());

        // === Второй LineRenderer (Underground) ===
        GameObject undergroundObj = new GameObject("UndergroundLine");
        undergroundObj.transform.parent = chunk.transform;

        LineRenderer undergroundLine = undergroundObj.AddComponent<LineRenderer>();
        undergroundLine.useWorldSpace = true;
        undergroundLine.loop = false;

        undergroundLine.startWidth = widthUnderground;
        undergroundLine.endWidth = widthUnderground;
        
        Material undergroundMat = new Material(Shader.Find("Unlit/Texture"));
        if (undergroundTexture != null)
        {
            undergroundMat.mainTexture = undergroundTexture;
            undergroundMat.mainTexture.wrapMode = TextureWrapMode.Repeat;
        }
        undergroundLine.material = undergroundMat;

        float undergroundOffsetY = offsetUnderground;
        Vector3[] undergroundPoints = smoothPoints
            .Select(p => new Vector3(p.x, p.y + undergroundOffsetY, p.z + 1))
            .ToArray();

        undergroundLine.positionCount = undergroundPoints.Length;
        undergroundLine.SetPositions(undergroundPoints);

        if (undergroundTexture != null)
        {
            float length = (undergroundPoints[undergroundPoints.Length - 1] - undergroundPoints[0]).magnitude;
            undergroundLine.material.mainTextureScale = new Vector2(length / undergroundTexture.width, 1);
        }

        if (Texture != null)
        {
            float length = (smoothPoints[smoothPoints.Count - 1] - smoothPoints[0]).magnitude;
            line.material.mainTextureScale = new Vector2(length / Texture.width, 1);
        }

        chunks.Add(chunk);
        lastX = controlPoints[controlPoints.Count - 1].x;
        lastY = controlPoints[controlPoints.Count - 1].y;
    }

    private float GenerateTerrainHeight(float x, float prevY)
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
