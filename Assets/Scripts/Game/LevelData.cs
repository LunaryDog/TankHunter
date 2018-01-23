using System.Collections;
using System.Collections.Generic;
using System;

public enum TypeObject
{
    TILE = 1,
    OBJECT = 2
}

[Serializable]
public class TileSet
{
    public int tileCount;
    public string pathTileSet;
    public string typeTileSet;

    public TileSet () 
    {
        tileCount = 0;
        pathTileSet = "";
        typeTileSet = "Tiles";
    }
    public TileSet(int count, string path, string type = "Tiles")
    {
        tileCount = count;
        pathTileSet = path;
        typeTileSet = type;
    }
}

[Serializable]
public class TileData
{   
    public int tileIndex;
    public float x;
    public float y;
    public bool visibleTile = true;

    public TileData()
    {
        tileIndex = 0;
        x = 0;
        y = 0;
        visibleTile = false;
    }

    public TileData(int index, float posX = 0f, float posY = 0f, bool visible = true)
    {
        tileIndex = index;
        x = posX;
        y = posY;
        visibleTile = visible;
    }
}

[Serializable]
public class Layer
{
    public string nameLayer;
    public string typeLayer;
    public bool visibleLayer;    
    public List<TileData> dataLayer;
    public Layer()
    {
        nameLayer = "Layer1";
        typeLayer = "Tiles";
        visibleLayer = false;
        dataLayer = new List<TileData>();
    }
    public Layer(string name, string type = "Tiles", bool visible = false)
    {
        nameLayer = name;
        typeLayer = type;
        visibleLayer = visible;
        dataLayer = new List<TileData>();
    }
}

[Serializable]
public class LevelProperties
{
    public int numberLevel;
    public int heightLevel;
    public int widthLevel;
    public float tileSize;   
    
    public LevelProperties()
    {
        numberLevel = 1;
        heightLevel = 5;
        widthLevel = 5;
        tileSize = 1f;
    }

    public LevelProperties(int num, int height, int width, float tile = 1f)
    {
        numberLevel = num;
        heightLevel = height;
        widthLevel = width;
        tileSize = tile;
    }
}

[Serializable] 
public class LevelData
{
    public LevelProperties levelProperties;
    public List<Layer> layers;
    public List<TileSet> tileSets;
}
