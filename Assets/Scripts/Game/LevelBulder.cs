using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelBulderEvens
{
    LOAD
}
public class LevelBulder : MonoBehaviour {
    Observer observer;
    public TextAsset levelJsonFile;
    //public float tileWidth;
    // public LevelDataJsonParser jsonParser;
    // public TilePropertiesFactory tilePropertiesFactory;
   
    public GameObject playerPrefab;
    public GameObject spawnerPrefab;
    public GameObject tilePrefab;
    public Tilelist list;
 //   private Sprite[] spritesTile;
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
        
        LevelData levelData = Parse(levelJsonFile.text);
        levelParent = new GameObject("Level");
        heightWorld = levelData.levelProperties.levelHeight * levelData.levelProperties.tileLevelНeight;
        widhtWorld = levelData.levelProperties.levelWidth * levelData.levelProperties.tileLevelWidht;
        foreach (Layer layer in levelData.layers)
        {
            if (!layer.visibleLayer)
            {
                continue;
            }
            GameObject layerGameObject = new GameObject(layer.nameLayer);
            layerGameObject.transform.SetParent(levelParent.transform);
            int tileIndex = 0;
            if (layer.typeLayer == "tilelayer")
            {
                for (int rowIndex = 0; rowIndex < layer.heightLayer; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < layer.widthLayer; columnIndex++)
                    {
                        TileData tileData = layer.dataLayer[tileIndex++];                      
                        int spriteIndex = tileData.brushIndex;
                      //  Debug.Log(spriteIndex);
                        if (spriteIndex == 0)
                        {
                           continue;
                        }

                        float offset_x = -1f * widhtWorld / 2 + levelData.levelProperties.tileLevelWidht/2;
                        float offset_y = 1f * heightWorld / 2 - levelData.levelProperties.tileLevelНeight/2;
                        Dictionary<string, string> tileProperties = levelData.tilesets[0].tileproperties.ContainsKey(spriteIndex - 1) ? levelData.tilesets[0].tileproperties[spriteIndex - 1] : new Dictionary<string, string>();
                        CreateTileObject(new Vector3(offset_x +(columnIndex * levelData.levelProperties.tileLevelWidht), offset_y +(-rowIndex * levelData.levelProperties.tileLevelНeight)), levelData, spriteIndex, layerGameObject, tileData, tileProperties);

                    }
                }
            }
            else if (layer.typeLayer == "objectgroup")
            {
                foreach (TileData tileData in layer.dataLayer)
                {
                    CreateTileObject(new Vector3(tileData.x, -tileData.y + levelData.tilesets[0].tileHeight), levelData, tileData.brushIndex, layerGameObject, tileData, tileData.tileProperties);
                }
            }
        }
       
    }

    private void Start()
    {
        observer.SendMessage(LevelBulderEvens.LOAD);
    }
    public LevelData Parse(string levelJson)
    {
        JSONObject jsonObject = new JSONObject(levelJson);
        LevelData levelData = new LevelData();
        List<Layer> layers = ParseLayers(jsonObject.GetField("layers"));
        levelData.layers = layers;

        List<Tileset> tilesets = ParseTilesets(jsonObject.GetField("tilesets"));
        levelData.tilesets = tilesets;

        LevelProperties levelProperties = ParseLevelProperties(jsonObject);
        levelData.levelProperties = levelProperties;

        return levelData;
    }

    LevelProperties ParseLevelProperties (JSONObject levelsPropertiesJson)
    {
        LevelProperties levelProperties = new LevelProperties();
        levelProperties.number = (int)levelsPropertiesJson.GetField("number").n;
        levelProperties.levelWidth = (int)levelsPropertiesJson.GetField("width").n;
        levelProperties.levelHeight = (int)levelsPropertiesJson.GetField("height").n;
        levelProperties.tileLevelНeight = levelsPropertiesJson.GetField("tileheight").n;
        levelProperties.tileLevelWidht = levelsPropertiesJson.GetField("tilewidth").n;
        return levelProperties;
    }

    List<Tileset> ParseTilesets(JSONObject tilesetsJson)
    {
        List<Tileset> tilesetList = new List<Tileset>();
        foreach (JSONObject tilesetJson in tilesetsJson.list)
        {
            Tileset tileset = new Tileset();
            tileset.tileWidth = (int)tilesetJson.GetField("tilewidth").n;
            tileset.tileHeight = (int)tilesetJson.GetField("tileheight").n;
            JSONObject tilepropertiesJson = tilesetJson.GetField("tileproperties");
            if (tilepropertiesJson != null)
            {
                for (int i = 0; i < tilepropertiesJson.list.Count; i++)
                {
                    JSONObject customPropertiesForTileJson = (JSONObject)tilepropertiesJson.list[i];
                    if (customPropertiesForTileJson != null)
                    {
                        int key = int.Parse((string)tilepropertiesJson.keys[i]);

                        if (!tileset.tileproperties.ContainsKey(key))
                        {
                            tileset.tileproperties[key] = new Dictionary<string, string>();
                        }

                        Dictionary<string, string> properties = tileset.tileproperties[key];

                        for (int n = 0; n < customPropertiesForTileJson.list.Count; n++)
                        {
                            string propertyKey = (string)customPropertiesForTileJson.keys[n];
                            string value = customPropertiesForTileJson.list[n].str;
                            properties.Add(propertyKey, value);
                        }
                    }

                }
            }
            tilesetList.Add(tileset);
        }
        return tilesetList;
    }

    List<Layer> ParseLayers(JSONObject layersJson)
    {
        List<Layer> layerList = new List<Layer>();
        for (int i = layersJson.list.Count - 1; i >= 0; i--)
        {
            JSONObject jsonLayer = layersJson.list[i];
            Layer layer = new Layer();

            layer.widthLayer = (int)jsonLayer["width"].n;
            layer.heightLayer = (int)jsonLayer["height"].n;
            layer.typeLayer = jsonLayer["type"].str;
            layer.nameLayer = jsonLayer["name"].str;
            layer.visibleLayer = jsonLayer["visible"].b;
            List<TileData> tileData = null;
            if (layer.typeLayer == "objectgroup")
            {
                tileData = ParseObjects(jsonLayer["objects"]);
            }
            else if (layer.typeLayer == "tilelayer")
            {
                tileData = ParseTiles(jsonLayer["data"]);
            }

            layer.dataLayer = tileData;
            layerList.Add(layer);
        }
        return layerList;
    }

    List<TileData> ParseTiles(JSONObject tileDataJson)
    {
        List<TileData> tileDataList = new List<TileData>();

        foreach (JSONObject dataJson in tileDataJson.list)
        {
            tileDataList.Add(new TileData((int)dataJson.n));
        }
        return tileDataList;
    }

    List<TileData> ParseObjects(JSONObject objectDataJson)
    {
        List<TileData> tileDataList = new List<TileData>();

        foreach (JSONObject dataJson in objectDataJson.list)
        {
            var tileData = new TileData((int)dataJson.GetField("gid").n);
            tileData.x = dataJson.GetField("x").n;
            tileData.y = dataJson.GetField("y").n;
            tileData.visibleTile = dataJson.GetField("visible").b;
            JSONObject objectPropertiesJson = dataJson.GetField("properties");

            if (objectPropertiesJson != null)
            {
                for (int n = 0; n < objectPropertiesJson.list.Count; n++)
                {
                    string propertyKey = (string)objectPropertiesJson.keys[n];
                    string value = objectPropertiesJson.list[n].str;
                    tileData.tileProperties.Add(propertyKey, value);
                }
            }

            tileDataList.Add(tileData);
        }
        return tileDataList;
    }

    void CreateTileObject(Vector3 position, LevelData levelData, int spriteIndex, GameObject parent, TileData tileData, Dictionary<string, string> tileProperties)
    {
        GameObject tile = CreateTile(spriteIndex);
        CreatePropertyComponents(tile, levelData.levelProperties.tileLevelWidht, tileProperties);
        tile.transform.SetParent(parent.transform);
        tile.transform.Translate(position);
        if (!tileData.visibleTile)
        {
            tile.GetComponent<SpriteRenderer>().enabled = false;
        }
    }


    public GameObject CreateTile(int id)
    {
       // Debug.Log(id);
       // Debug.Log(list.tiles.Count);
     //   if (spritesTile == null) 
       // {
           // LoadSprites();
       // }
        GameObject prefub = list.tiles[id-1].gameObject;

        GameObject tile = Instantiate(prefub);      
      //  tile.GetComponent<SpriteRenderer>().sprite = spritesTile[id - 1];
        return tile;
    }

    public void CreatePropertyComponents(GameObject tile, float tilewidth, Dictionary<string, string> properties)
    {
        if (properties.ContainsKey("colliding"))
        {
            BoxCollider2D boxCollider2D = tile.GetComponent<BoxCollider2D>();
           // boxCollider2D.enabled = true;           
           // boxCollider2D.size = new Vector2(tilewidth, tilewidth);
        }

        if (properties.ContainsKey(("SpawnPlayer")))
        {
            Instantiate(playerPrefab);
           // tile.AddComponent<PlayerSpawner>();
        }
    }

}
