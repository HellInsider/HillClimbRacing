using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentObjectSettings", menuName = "Terrain/Environment Object Settings")]
public class EnvironmentObjectSettings : ScriptableObject
{
    public GameObject prefab;
    [Range(0f, 1f)] public float spawnChance = 0.1f;
    public float yOffset = 0f;
}
