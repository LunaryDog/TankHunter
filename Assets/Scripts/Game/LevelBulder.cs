using UnityEngine;

public enum LevelBulderEvens
{
    LOAD
}
public class LevelBulder : MonoBehaviour {
    Observer observer;
    public TextAsset levelFile;
  
    public LevelParser jsonParser;
 
   
    public GameObject playerPrefab;
    public GameObject spawnerPrefab;
    public GameObject tilePrefab;
    public Tilelist list;

    private GameObject levelParent;

    static float heightWorld;
    public static float HeightWorld
    {
        get { return heightWorld; }
    }

    static float widhtWorld;
    public static float WidhtWorld
    {
        get { return widhtWorld; }
    }

    void Awake()
    {
        observer = Observer.Instance;
        
        LevelData levelData = jsonParser.Parse(levelFile.text); 
        levelParent = new GameObject("Level");
        heightWorld = levelData.levelProperties.heightLevel * levelData.levelProperties.tileSize;
        widhtWorld = levelData.levelProperties.widthLevel * levelData.levelProperties.tileSize;
        foreach (Layer layer in levelData.layers)
        {
            if (!layer.visibleLayer)
            {
                continue;
            }
            GameObject layerGameObject = new GameObject(layer.nameLayer);
            layerGameObject.transform.SetParent(levelParent.transform);
            int tileIndex = 0;
            if (layer.typeLayer == "Tiles")
            {
                for (int rowIndex = 0; rowIndex < levelData.levelProperties.heightLevel; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < levelData.levelProperties.widthLevel; columnIndex++)
                    {
                        TileData tileData = layer.dataLayer[tileIndex++];                      
                        int spriteIndex = tileData.tileIndex;
                      //  Debug.Log(spriteIndex);
                        if (spriteIndex == 0)
                        {
                           continue;
                        }

                        float offset_x = -1f * widhtWorld / 2 + levelData.levelProperties.tileSize/2;
                        float offset_y = 1f * heightWorld / 2 - levelData.levelProperties.tileSize/2;                    
                        CreateTileObject(new Vector3(offset_x +(columnIndex * levelData.levelProperties.tileSize), offset_y +(-rowIndex * levelData.levelProperties.tileSize)), levelData, spriteIndex, layerGameObject, tileData);

                    }
                }
            }
            else if (layer.typeLayer == "Objects")
            {
                foreach (TileData tileData in layer.dataLayer)
                {
                    CreateTileObject(new Vector3(tileData.x, -tileData.y + levelData.levelProperties.tileSize), levelData, tileData.tileIndex, layerGameObject, tileData );
                }
            }
        }       
    }
    private void Start()
    {
        observer.SendMessage(LevelBulderEvens.LOAD);
    }
  
    void CreateTileObject(Vector3 position, LevelData levelData, int spriteIndex, GameObject parent, TileData tileData)
    {
        GameObject tile = CreateTile(spriteIndex);
      
        tile.transform.SetParent(parent.transform);
        tile.transform.Translate(position);
        if (!tileData.visibleTile)
        {
            tile.GetComponent<SpriteRenderer>().enabled = false;
        }
    }


    public GameObject CreateTile(int id)
    {
        GameObject prefub = list.tiles[id-1].gameObject;
        GameObject tile = Instantiate(prefub);  
        return tile;
    }
    

}
