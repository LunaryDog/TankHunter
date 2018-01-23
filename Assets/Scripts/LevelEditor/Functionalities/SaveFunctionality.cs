using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveFunctionality : MonoBehaviour {
	private LevelEditor _levelEditor;

    private LevelData levelData; 

    // Плитки, используемые для построения уровня
    private List<Transform> _tiles;

    public void Setup( LevelData levelDataEditor, List<Transform> tiles) {
		_levelEditor = LevelEditor.Instance;      
        levelData = levelDataEditor;       
        _tiles = tiles;
		SetupClickListeners();
	}

    // Подключить метод Save / Load Level для сохранения / загрузки
    private void SetupClickListeners() {
        Utilities.FindButtonAndAddOnClickListener("SaveButton", SaveLevel);
	}

    // Сохранение в файл по пути
    public void SaveLevelUsingPath(string path) {       
        if (path.Length != 0) {
            // Сохранение уровня в файл
            string levelJsonFile = _levelEditor.creator.Create(levelData);
            path = path + "Level" + levelData.levelProperties.numberLevel + ".json";        
            FileStream file = File.Create(path);
            file.Close();
            File.AppendAllText (path, levelJsonFile);	
        }
        else {
			Debug.Log("Invalid path given");
		}
	}

    // Сохранение уровня 
    private void SaveLevel() { 
        
        SaveLevelUsingPath(Application.dataPath + "/Resources/Levels/");       
    }
    

}