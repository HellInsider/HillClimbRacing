using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private EnvironmentSettings citySettings;
    [SerializeField] private EnvironmentSettings desertSettings;
    [SerializeField] private Transform player;
    [SerializeField] private float transitionDistance = 100f;
    [SerializeField] private TextMeshProUGUI TotalRoad;
    [SerializeField] Car car;
    private float oldEngine;
    private float oldExpenditure;

    void Start()
    {
        oldEngine = car._engineForce;
        oldExpenditure = car._Expenditure;
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
        TotalRoad.text = "total: " + ((int)recordTrack);
        if (player == null)
        {
            return;
        }
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
        GameObject newObj = new GameObject("PooledObject_Overflow");
        newObj.SetActive(true);
        return newObj;
    }

    public void ReturnPooledObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(null);
        objectPool.Enqueue(obj);
    }

    void SetEnvironment(EnvironmentType type)
    {
        currentEnvironment = type;
        EnvironmentSettings settings = type == EnvironmentType.City ? citySettings : desertSettings;
        if (terrainGenerator != null)
        {
            terrainGenerator.perlinNoiseFrequency = settings.perlinNoiseFrequency;
            terrainGenerator.heightVariation = settings.heightVariation;
            terrainGenerator.mountainThreshold = settings.mountainThreshold;
            terrainGenerator.smoothing = settings.smoothing;
            terrainGenerator.Texture = settings.terrainTexture;
            terrainGenerator.environmentObjectsSettings = settings.environmentObjects;
            terrainGenerator.coinSpacing = settings.coinSpacing;
            terrainGenerator.fuelSpacing = settings.fuelSpacing;
        }
    }

    private void SwitchEnvironment()
    {
        
        if (terrainGenerator == null)
        {
            return;
        }
        EnvironmentType newEnvironment = currentEnvironment == EnvironmentType.City ? EnvironmentType.Desert : EnvironmentType.City;
        EnvironmentSettings newSettings = newEnvironment == EnvironmentType.City ? citySettings : desertSettings; 
        ApplyBiomePenalties(newSettings);
        SetEnvironment(currentEnvironment == EnvironmentType.City ? EnvironmentType.Desert : EnvironmentType.City);
        foreach (GameObject chunk in terrainGenerator.chunks)
        {
            if (chunk != null)
            {
                foreach (Transform child in chunk.transform)
                {
                    if (child.gameObject.activeSelf && Array.Exists(terrainGenerator.environmentObjectsSettings,
                        setting => setting.prefab == child.gameObject))
                    {
                        ReturnPooledObject(child.gameObject);
                    }
                }
            }
        }
        terrainGenerator.chunks.Clear();
        terrainGenerator.GenerateInitialChunks();
    }
    private void ApplyBiomePenalties(EnvironmentSettings settings)
    {
        car._engineForce = oldEngine;
        car._Expenditure = oldExpenditure;
        if (car != null)
        {
            car._engineForce -= settings.speedPenalty;
            car._Expenditure += settings.fuelPenalty;
        }
    }
}
