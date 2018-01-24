using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LoadEditorScript : MonoBehaviour {
   
	private LevelEditor levelEditor;
    private LevelData levelData;
	
	private List<Transform> tiles;
    private List<Transform> objects;
   

    public void Setup(List<Transform> tilesEditor, List<Transform> objectsEditor) {
		levelEditor = LevelEditor.Instance;      	
		tiles = tilesEditor;
        objects = objectsEditor;
       
        SetupClickListeners();
	}

	private void SetupClickListeners() {       
          
	}
   
    public void LoadLevel(string file)
    {   
        
        levelData = levelEditor.parser.Parse(file);
        levelEditor.ResetBeforeLoad();
        LoadLevelFromLayers(levelData.layers);        
    }

	
	// Метод для загрузки слоев
	private void LoadLevelFromLayers(List<Layer> layers) {
 
        int numLayer = 0;
		foreach (Layer layer in layers) {
			
				LoadLevelFrom(numLayer, layer.dataLayer);
                numLayer++;
		}
	}

	// Метод для загрузки данных из слоя
	private void LoadLevelFrom(int layer, List<TileData> data) {	
		
		for (int i = 0; i < data.Count; i++) {
            int posY = (int)(i / levelData.levelProperties.widthLevel);
            int posX = (int)(i - (levelData.levelProperties.widthLevel * posY));
                   
            int value = data[i].tileIndex - 1;
        
			levelEditor.CreateBlock(value, posX, levelData.levelProperties.heightLevel - posY - 1, layer);
		}

        levelEditor.UpdateLayerVisibility();
        levelEditor.UpdateEditorAfterLoad(levelData);
	}

	
}
