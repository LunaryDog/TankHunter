using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LoadFunctionality : MonoBehaviour {
   // Observer observer;
	private LevelEditor _levelEditor;
    private LevelData levelData;
	
	private List<Transform> _tiles;

    public void Setup(List<Transform> tiles) {
		_levelEditor = LevelEditor.Instance;
       // levelData = levelDataEditor;		
		_tiles = tiles;
		SetupClickListeners();
	}

	private void SetupClickListeners() {       
           Utilities.FindButtonAndAddOnClickListener("LoadButton", LoadAllLevel);
	}


    private void LoadAllLevel()
    {
        object[] text = (object[]) Resources.LoadAll("Levels");
        string file = text[text.Length-1].ToString();
        levelData = _levelEditor.parser.Parse(file);
        LoadLevelFromLayers(levelData.layers);
        //Debug.Log(file);
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
	
		// Place each block in order in the correct x and y position
		for (int i = 0; i < data.Count; i++) {
            int posY = (int)(i / levelData.levelProperties.widthLevel);
            int posX = (int)(i - (levelData.levelProperties.widthLevel * posY));
            int value = 0;
           // if ((data[i].tileIndex - 1) >= 0)
           // {
                value = data[i].tileIndex - 1;
           // }
				_levelEditor.CreateBlock(value, posX, levelData.levelProperties.heightLevel - posY - 1, layer);
			}

        // Update to only show the correct layer(s)
     //   _levelEditor.levelData = levelData;
		_levelEditor.UpdateLayerVisibility();
        _levelEditor.UpdateEditorAfterLoad(levelData);
	}

	
}
