using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreate : MonoBehaviour {

    public string Create(LevelData levelData)
    {
       
        JSONObject jsonData = new JSONObject(JsonUtility.ToJson(levelData.levelProperties));

        JSONObject layersJson = CreateJsonLayers(levelData.layers);       
        jsonData.AddField("layers", layersJson);

        JSONObject tileSetsJson = CreateJsonTilesets(levelData.tileSets);
        jsonData.AddField("tileSets", tileSetsJson);

        string levelString = jsonData.ToString();
        return levelString;
    }


    JSONObject CreateJsonProperties(LevelProperties properties)
    {
        JSONObject jsonProperties = new JSONObject(JsonUtility.ToJson(properties));        
        return jsonProperties;
    }

    JSONObject CreateJsonTilesets(List<TileSet> tileSets)
    {
        JSONObject tileListsJson = new JSONObject(JSONObject.Type.ARRAY);
       
        foreach (TileSet tileSet in tileSets)
        {
            JSONObject newTileSetJson = new JSONObject(JsonUtility.ToJson(tileSet));           
            tileListsJson.Add(newTileSetJson);
        }
        return tileListsJson;
    }

    JSONObject CreateJsonLayers(List<Layer> layers)
    {
        JSONObject jsonLayers = new JSONObject(JSONObject.Type.ARRAY);
        List<Layer> newLayers = ClearEmptyLayer(layers);
        if (newLayers.Count > 0)
        {
            foreach (Layer layer in newLayers)
            {
                JSONObject newlayersJson = new JSONObject(JSONObject.Type.OBJECT);               
                newlayersJson.AddField("nameLayer", layer.nameLayer);
                newlayersJson.AddField("visibleLayer", layer.visibleLayer);
                newlayersJson.AddField("typeLayer", layer.typeLayer);
                JSONObject dataLayerJson = new JSONObject(JSONObject.Type.ARRAY);
                if (layer.typeLayer == "Tiles")
                {
                    dataLayerJson = CreateJsonTiles(layer.dataLayer);
                    newlayersJson.AddField("dataLayer", dataLayerJson);
                }
                else
                {
                    if (layer.typeLayer == "Objects")
                    {
                        dataLayerJson = CreatJsonObjects(layer.dataLayer);
                        newlayersJson.AddField("objects", dataLayerJson);
                    }
                }
                
                jsonLayers.Add(newlayersJson);
            }
        }      
        return jsonLayers;
    }

    JSONObject CreateJsonTiles(List<TileData> tileData)
    {
        JSONObject dataLayerJson = new JSONObject(JSONObject.Type.ARRAY);
        if (tileData.Count > 0)
        {
            foreach (TileData data in tileData)
            {
                dataLayerJson.Add(data.tileIndex);
            }
        }
        return dataLayerJson;
    }

    JSONObject CreatJsonObjects(List<TileData> objectData)
    {
        JSONObject tileDataJson = new JSONObject(JSONObject.Type.ARRAY);

        foreach (TileData dataJson in objectData)
        {
            JSONObject objectJson = new JSONObject(JsonUtility.ToJson(dataJson));           
            tileDataJson.Add(objectJson);
        }
        return tileDataJson;
    }

    private List<Layer> ClearEmptyLayer(List<Layer> layers)
    {
        List<Layer> buffLayer = new List<Layer>();
        foreach (Layer layer in layers)
        {
            int count = 0;
            if (layer.dataLayer.Count > 0)
            {
                foreach (TileData tile in layer.dataLayer)
                {
                    if (tile.tileIndex > 0)
                    {
                        count++;
                    }
                }
            }
            if (count != 0)
            {
                buffLayer.Add(layer);
            }
        }
        layers = buffLayer;
        if (layers.Count > 0)
        {
            int count = 1;
            foreach (Layer layer in layers)
            {
                layer.nameLayer = "Layer " + count;
                layer.visibleLayer = true;
                layer.typeLayer = "Tiles";
                count++;
            }
        }
        return layers;
    }
}

