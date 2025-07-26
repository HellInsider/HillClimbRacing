using System.Collections.Generic;
using UnityEngine;

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
        public float objectSpawnChance;
        public float smoothing;
        public Texture2D terrainTexture;
        public GameObject[] environmentObjects;
        
    }
   

   
}
