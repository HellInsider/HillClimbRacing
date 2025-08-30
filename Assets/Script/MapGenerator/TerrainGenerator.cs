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
    public float smoothing = 0.2f;
    public int subdivisions = 5;
    public int seed;
    public float undergroundDepth = 5f;
    public float coinSpacing = 10f;
    public float fuelSpacing = 25f;
    public float addCoinSpacing;
    public float addFuelSpacing;
    public GameObject coinPrefab;
    public GameObject fuelPrefab;
    public Transform player;
    public GameObject groundPrefab;
    public EnvironmentObjectSettings[] environmentObjectsSettings;
    public Texture2D Texture;
    public Material undergroundMeshMaterial;
    public LineRenderer lineRenderer;

    public readonly List<GameObject> chunks = new List<GameObject>();
    private float lastX = 0;
    private float lastY = 0;
    private float lastCoinX = 0f;
    private float lastFuelX = 0f;
    private LevelMenager levelManager;
    [System.Obsolete]
    void Start()
    {
        seed = System.DateTime.Now.Millisecond;
        Random.InitState(seed);
        levelManager = FindObjectOfType<LevelMenager>();
        GenerateInitialChunks();
    }
    public void NewStart()
    {
        seed = System.DateTime.Now.Millisecond;
        Random.InitState(seed);
        levelManager = FindObjectOfType<LevelMenager>();
        GenerateInitialChunks();
    }
    void Update()
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
        LineRenderer line = chunk.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = chunk.AddComponent<LineRenderer>();
        }
        EdgeCollider2D collider = chunk.GetComponent<EdgeCollider2D>();
        if (collider == null)
        {
            collider = chunk.AddComponent<EdgeCollider2D>();
        }
        chunk.AddComponent<ColliderLine>();
        //LineRenderer line = chunk.GetComponent<LineRenderer>() ?? chunk.AddComponent<LineRenderer>();
        //EdgeCollider2D collider = chunk.GetComponent<EdgeCollider2D>() ?? chunk.AddComponent<EdgeCollider2D>();
        //chunk.AddComponent<ColliderLine>();

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

            foreach (var envObj in environmentObjectsSettings)
            {
                if (envObj.prefab != null && Random.value < envObj.spawnChance)
                {
                    GameObject obj = levelManager?.GetPooledObject() ?? Instantiate(envObj.prefab);
                    obj.transform.position = new Vector3(x, y + envObj.yOffset, 0);
                    obj.transform.parent = chunk.transform;

                    if (!obj.activeSelf)
                    {
                        obj.SetActive(true);
                        var renderer = obj.GetComponent<SpriteRenderer>();
                        var prefabRenderer = envObj.prefab.GetComponent<SpriteRenderer>();
                        if (renderer != null && prefabRenderer != null)
                        {
                            renderer.sprite = prefabRenderer.sprite;
                        }
                    }
                }
            }

            // Монеты
            if (coinPrefab != null && x >= lastCoinX + coinSpacing)
            {
                GameObject coin = Instantiate(coinPrefab, new Vector3(x, y + 1f, 0), Quaternion.identity, chunk.transform);
                lastCoinX = x;
                coinSpacing += addCoinSpacing;
            }

            // Топливо
            if (fuelPrefab != null && x >= lastFuelX + fuelSpacing)
            {
                GameObject fuel = Instantiate(fuelPrefab, new Vector3(x, y + 1f, 0), Quaternion.identity, chunk.transform);
                lastFuelX = x;
                fuelSpacing += addFuelSpacing;
            }
        }

        List<Vector3> smoothPoints = InterpolateCatmullRom(controlPoints);

        line.positionCount = smoothPoints.Count;
        line.SetPositions(smoothPoints.ToArray());

        // === Второй LineRenderer (Underground) ===
        // === Подземный Mesh ===
        GameObject undergroundMeshObj = new GameObject("UndergroundMesh");
        undergroundMeshObj.transform.parent = chunk.transform;

        MeshFilter meshFilter = undergroundMeshObj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = undergroundMeshObj.AddComponent<MeshRenderer>();
        meshRenderer.material = undergroundMeshMaterial;

        Mesh mesh = new Mesh();

        int pointCount = smoothPoints.Count;
        Vector3[] vertices = new Vector3[pointCount * 2];
        int[] triangles = new int[(pointCount - 1) * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        // Верхняя линия
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 top = smoothPoints[i];
            Vector3 bottom = new Vector3(top.x, top.y - undergroundDepth, top.z);

            vertices[i] = top;
            vertices[i + pointCount] = bottom;

            // UV по X и Y нормализуем
            float u = i / (float)(pointCount - 1);
            uvs[i] = new Vector2(u, 1);
            uvs[i + pointCount] = new Vector2(u, 0);

            // Треугольники
            if (i < pointCount - 1)
            {
                int t = i * 6;

                int topLeft = i;
                int topRight = i + 1;
                int bottomLeft = i + pointCount;
                int bottomRight = i + 1 + pointCount;

                triangles[t] = topLeft;
                triangles[t + 1] = bottomLeft;
                triangles[t + 2] = topRight;

                triangles[t + 3] = topRight;
                triangles[t + 4] = bottomLeft;
                triangles[t + 5] = bottomRight;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

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
