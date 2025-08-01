using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BiomInfo;
public enum EnvironmentType { City, Desert }
public class LevelMenager : MonoBehaviour
{
    public int coinVal;
    public float recordTrack;
    public TerrainGenerator terrain;
    private EnvironmentType currentEnvironment;
    private float lastTransitionX;
    private readonly Queue<GameObject> objectPool = new Queue<GameObject>();
    private const int POOL_SIZE = 20;
    [SerializeField] public TerrainGenerator terrainGenerator;
    [SerializeField] public EnvironmentSettings citySettings;
    [SerializeField] public EnvironmentSettings desertSettings;
    [SerializeField] public Transform player;
    [SerializeField] public float transitionDistance = 100f;
    void Start()
    {
        if (terrainGenerator == null || player == null)
        {
            Debug.LogError("TerrainGenerator or Player not assigned in LevelManager!");
            enabled = false;
            return;
        }
        InitializeObjectPool();
        SetEnvironment(EnvironmentType.City);
        lastTransitionX = player.position.x;
    }

    void Update()
    {
        if (Mathf.Abs(player.position.x - lastTransitionX) > transitionDistance)
        {
            SwitchEnvironment();
            lastTransitionX = player.position.x;
        }
    }

    void InitializeObjectPool()
    {
        for (int i = 0; i < POOL_SIZE; i++)
        {
            GameObject obj = new GameObject("PooledObject");
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        if (objectPool.Count > 0)
        {
            GameObject obj = objectPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    public void ReturnPooledObject(GameObject obj)
    {
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }

    void SetEnvironment(EnvironmentType type)
    {
        currentEnvironment = type;
        EnvironmentSettings settings = type == EnvironmentType.City ? citySettings : desertSettings;
        terrainGenerator.perlinNoiseFrequency = settings.perlinNoiseFrequency;
        terrainGenerator.heightVariation = settings.heightVariation;
        terrainGenerator.mountainThreshold = settings.mountainThreshold;
       // terrainGenerator.objectSpawnChance = settings.objectSpawnChance;
        terrainGenerator.smoothing = settings.smoothing;
        terrainGenerator.Texture = settings.terrainTexture;
       // terrainGenerator.environmentObjects = settings.environmentObjects;
    }
    private void SwitchEnvironment()
    {
        SetEnvironment(currentEnvironment == EnvironmentType.City ? EnvironmentType.Desert : EnvironmentType.City);
       /* foreach (GameObject chunk in terrainGenerator.chunks)
        {
            foreach (Transform child in chunk.transform)
            {
                if (child.gameObject.activeSelf && terrainGenerator.environmentObjects.Contains(child.gameObject))
                {
                    ReturnPooledObject(child.gameObject);
                }
            }
        }*/
        terrainGenerator.chunks.Clear();
        terrainGenerator.GenerateInitialChunks();

    }
}
