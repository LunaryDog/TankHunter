using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Tileset
{
    public int tileCount;
    public int tileWidth;
    public int tileHeight;
    public Dictionary<int, Dictionary<string, string>> tileproperties = new Dictionary<int, Dictionary<string, string>>();
}

[Serializable]
public class TileData
{
    public TileData(int brushIndex)
    {
        this.brushIndex = brushIndex;
    }
    public Dictionary<string, string> tileProperties = new Dictionary<string, string>();
    public int brushIndex;
    public float x;
    public float y;
    public bool visibleTile = true;
}

[Serializable]
public class Layer
{
    public bool visibleLayer;
    public string typeLayer;
    public int widthLayer;
    public int heightLayer;
    public string nameLayer;
    public List<TileData> dataLayer;
}

[Serializable]
public class LevelProperties
{
    public int number;
    public int levelHeight;
    public int levelWidth;
    public float tileLevelНeight;
    public float tileLevelWidht;
}

[Serializable] 
public class LevelData
{
    public LevelProperties levelProperties;
    public List<Layer> layers;
    public List<Tileset> tilesets;
}
