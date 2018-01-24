using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelParser : MonoBehaviour {
    
    public LevelData Parse(string levelJson)
    {
        JSONObject jsonObject = new JSONObject(levelJson);
        LevelData levelData = new LevelData();
        List<Layer> layers = ParseLayers(jsonObject.GetField("layers"));
        levelData.layers = layers;

        List<TileSet> tilesets = ParseTilesets(jsonObject.GetField("tileSets"));
        levelData.tileSets = tilesets;

        LevelProperties levelProperties = ParseLevelProperties(jsonObject);
        levelData.levelProperties = levelProperties;

        return levelData;
    }

    LevelProperties ParseLevelProperties(JSONObject levelsPropertiesJson)
    {
        
        int number = (int)levelsPropertiesJson.GetField("numberLevel").n;
        int width = (int)levelsPropertiesJson.GetField("widthLevel").n;
        int height = (int)levelsPropertiesJson.GetField("heightLevel").n;
        float tile = levelsPropertiesJson.GetField("tileSize").n;
        LevelProperties levelProperties = new LevelProperties(number, height, width, tile);
        return levelProperties;
    }

    List<TileSet> ParseTilesets(JSONObject tilesetsJson)
    {
        List<TileSet> tilesetList = new List<TileSet>();
        foreach (JSONObject tilesetJson in tilesetsJson.list)
        {
            TileSet tileset = new TileSet();
            tileset.tileCount = (int)tilesetJson.GetField("tileCount").n;
            tileset.typeTileSet = tilesetJson.GetField("typeTileSet").str;
            tileset.pathTileSet = tilesetJson.GetField("pathTileSet").str;
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
            layer.typeLayer = jsonLayer["typeLayer"].str;
            layer.nameLayer = jsonLayer["nameLayer"].str;
            layer.visibleLayer = jsonLayer["visibleLayer"].b;
            List<TileData> tileData = null;
            if (layer.typeLayer == "Objects")
            {
                tileData = ParseObjects(jsonLayer["objects"]);
            }
            else if (layer.typeLayer == "Tiles")
            {
                tileData = ParseTiles(jsonLayer["dataLayer"]);
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
            var tileData = new TileData((int)dataJson.GetField("tileIndex").n);
            tileData.x = dataJson.GetField("x").n;
            tileData.y = dataJson.GetField("y").n;
            tileData.visibleTile = dataJson.GetField("visibleTile").b;
            tileDataList.Add(tileData);
        }
        return tileDataList;
    }
}
