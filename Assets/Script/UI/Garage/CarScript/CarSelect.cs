using UnityEngine;

[CreateAssetMenu(fileName = "New Car", menuName = "Scriptable Objects/Car")]
public class CarSelect : ScriptableObject  
{
    public int carIndex;
    public string carName;
    public string carDescription;
    public Sprite carImage;
    public GameObject carPrefab;
}