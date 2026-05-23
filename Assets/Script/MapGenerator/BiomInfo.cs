using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BiomInfo", menuName = "Scriptable Objects/BiomInfo")]
public class BiomInfo : ScriptableObject
{
    [SerializeField] EnvironmentType DropDown;
    [System.Serializable]
    public struct EnvironmentSettings
    {
        public float perlinNoiseFrequency;
        public float heightVariation;
        public float mountainThreshold;
        public float smoothing;
        public Texture2D terrainTexture;
        public EnvironmentObjectSettings[] environmentObjects;
        public float coinSpacing;
        public float fuelSpacing;
        public float speedPenalty; 
        public float fuelPenalty;
        public float addCoinSpacing;
        public float addFuelSpacing;
        public Texture2D image;
    }
    public EnvironmentSettings primarySettings;
    public EnvironmentSettings citySettings;
    public EnvironmentSettings desertSettings;
}
