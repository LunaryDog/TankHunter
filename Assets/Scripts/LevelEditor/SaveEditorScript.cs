using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveEditorScript : MonoBehaviour {
	private LevelEditor levelEditor;

    private LevelData levelData; 

    // Плитки, используемые для построения уровня
    private List<Transform> tiles;
    private List<Transform> objects;

    public void Setup( LevelData levelDataEditor, List<Transform> tilesEditor, List<Transform> objectsEditor) {
		levelEditor = LevelEditor.Instance;      
        levelData = levelDataEditor;       
        tiles = tilesEditor;       
        objects = objectsEditor;
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
            string levelJsonFile = levelEditor.creator.Create(levelData);
            path = path + "Level" + levelData.levelProperties.numberLevel + ".json";        
            FileStream file = File.Create(path);
            file.Close();
            File.AppendAllText (path, levelJsonFile);
            Debug.Log(("Level" + levelData.levelProperties.numberLevel + ".json")  + " create!");
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