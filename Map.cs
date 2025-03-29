using UnityEngine;

[CreateAssetMenu (fileName = "New Map", menuName = "Scriptable objects/Map")]
public class Map : ScriptableObject
{
    public int mapIndex;
    public string mapName;
    public string mapDescription;
    public Sprite mapImage;
    public Object sceneToLoad;

}

    

