using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveFunctionality : MonoBehaviour {
	
	private LevelEditor _levelEditor;

    // Расширение файла для сохраненного файла
    private string _fileExtension;

    // Метод идентификации плиток при сохранении
    private TileIdentificationMethod _saveMethod;

    // Временная переменная для сохранения уровня до получения пути с помощью FileBrowser
    private string _levelToSave;

    // Временная переменная, чтобы сохранить состояние редактора уровней перед открытием браузера файлов и восстановить его после сохранения / загрузки
    private bool _preFileBrowserState = true;

    // Плитки, используемые для построения уровня
    private List<Transform> _tiles;

    public void Setup(GameObject fileBrowserPrefab, string fileExtension, TileIdentificationMethod saveMethod, List<Transform> tiles) {
		_levelEditor = LevelEditor.Instance;
		//_fileBrowserPrefab = fileBrowserPrefab;
		_fileExtension = fileExtension.Trim() == "" ? "lvl" : fileExtension;
		_saveMethod = saveMethod;
		_tiles = tiles;
		SetupClickListeners();
	}

    // Подключить метод Save / Load Level для сохранения / загрузки
    private void SetupClickListeners() {
    Utilities.FindButtonAndAddOnClickListener("SaveButton", SaveLevel);
	}

    // Сохранение в файл по пути
    public void SaveLevelUsingPath(string path) {
        // Включить LevelEditor, когда файл fileBrowser
        _levelEditor.ToggleLevelEditor(_preFileBrowserState);
        if (path.Length != 0) {
            // Сохранение уровня в файл
            BinaryFormatter bFormatter = new BinaryFormatter();
            FileStream file = File.Create(path);
			bFormatter.Serialize(file, _levelToSave);
			file.Close();
            // Сброс временной переменной
            _levelToSave = null;
        }
        else {
			Debug.Log("Invalid path given");
		}
	}


    // Метод определения пустого слоя (пустые слои не сохраняются)
    private bool EmptyLayer(int[,,] level, int width, int height, int layer, int empty) {
    bool result = true;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (level[x, y, layer] != empty) {
					result = false;
				}
			}
		}
		return result;
    }

    // Преобразует преобразование внутреннего уровня (целое число) в тип idenfication
    // Плитки можно идентифицировать, используя их индекс в массиве Tileset или имя объекта игры prefab
    // Пустые плитки будут сохранены с помощью имени "EMPTY"
    // По умолчанию будет LevelEditor.GetEmpty () (значение по умолчанию -1)
    private string TileSaveRepresentationToString(int[,,] levelToSave, int x, int y, int layer) {
		switch (_saveMethod) {
			case TileIdentificationMethod.Index:
				return "" + levelToSave[x, y, layer];
			case TileIdentificationMethod.Name:
				return levelToSave[x, y, layer] == LevelEditor.GetEmpty()
					? "EMPTY"
					: _tiles[levelToSave[x, y, layer]].gameObject.name;
			default:
				return "" + LevelEditor.GetEmpty();
		}
	}

    // Сохранение уровня для переменной и файла с помощью FileBrowser и SaveLevelUsingPath
    private void SaveLevel() {
    int[,,] levelToSave = _levelEditor.GetLevel();
		int width = _levelEditor.Width;
		int height = _levelEditor.Height;
		int layers = _levelEditor.Layers;
		List<string> newLevel = new List<string>();
        // Прокручиваем слои
        for (int layer = 0; layer < layers; layer++) {
            // Если слой не пуст, добавьте его и добавьте \ t в конец "
            if (!EmptyLayer(levelToSave, width, height, layer, LevelEditor.GetEmpty())) {
                // Перебираем строки и добавляем \ n в конец "
                for (int y = 0; y < height; y++) {
                string newRow = "";
					for (int x = 0; x < width; x++) {
						newRow += TileSaveRepresentationToString(levelToSave, x, y, layer) + ",";
					}
					if (y != 0) {
						newRow += "\n";
					}
					newLevel.Add(newRow);
				}
				newLevel.Add("\t" + layer);
			}
		}

        // Обратим строки, чтобы закончить окончательную версию
        newLevel.Reverse();
        string levelComplete = "";
		foreach (string level in newLevel) {
			levelComplete += level;
		}
        // Временно сохранить уровень, чтобы сохранить его с помощью 
        _levelToSave = levelComplete;
        // Откройте браузер файлов, чтобы получить путь и имя файла
        OpenFileBrowser();
    }
    
    private void OpenFileBrowser() {
		_preFileBrowserState = _levelEditor.GetScriptEnabled();
		// Disable the LevelEditor while the fileBrowser is open
		_levelEditor.ToggleLevelEditor(false);
		// Create the file browser and name it
	//	GameObject fileBrowserObject = Instantiate(_fileBrowserPrefab, transform);
	//	fileBrowserObject.name = "FileBrowser";
		// Set the mode to save or load
	//	FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
		//fileBrowserScript.SetupFileBrowser(ViewMode.Landscape);
	//	fileBrowserScript.SaveFilePanel(this, "SaveLevelUsingPath", "Level", _fileExtension);
	}
}