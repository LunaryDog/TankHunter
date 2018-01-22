using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum TileIdMethod {
	Index,
	Name
}

// The LevelBuilder allows the user to build level without requiring the LevelEditor prefab or script
// It contains the public method LoadLevelUsingPath which will load the file from the path and build the level
// For an example, see the LevelBuilderDemoCaller script
// Most of the code is extracted from the LoadFunctionality and LevelEditor scripts
public class LevelBuilder : MonoBehaviour {

public string RelativePath = "Resources/Levels/level.json";
    // Набор элементов, используемый для построения уровня
    [SerializeField] private Tilelist _tileset;

    // Метод загрузки для идентификации тайлов
    [SerializeField] private TileIdentificationMethod _loadMethod;

    // Интеральное представление пустого тайла
    [SerializeField] private int _empty = -1;

	// Массив плит из набора
	private List<Transform> _tiles;

    // GameObject как родительский для всех слоев (чтобы окно Иерархия было чистым)
    private GameObject _tileLevelParent;

    // Словарь как родительский для всех GameObjects на слой
    private readonly Dictionary<int, GameObject> _layerParents = new Dictionary<int, GameObject>();

    //Устанавливаем сборку TileLevel и устанавливаем переменную _tile
    private void Awake() {
		_tileLevelParent = GameObject.Find("TileLevel") ?? new GameObject("TileLevel");
		_tiles = _tileset.Tiles;
	}
    private void Start()
    {
            LoadLevelUsingPath(Application.dataPath + RelativePath);
    }
    // Загрузка из файла с использованием пути
    public void LoadLevelUsingPath(string path) {
		if (path.Length != 0) {
			BinaryFormatter bFormatter = new BinaryFormatter();
            // Сбросить уровень
            FileStream file = File.OpenRead(path);
            // Преобразование файла из байтового массива в строку
            string levelData = bFormatter.Deserialize(file) as string;
            // Мы закончили работу с файлом, чтобы мы могли закрыть его
            file.Close();
			LoadLevelFromStringLayers(levelData);
		} else {
			Debug.Log("Invalid path given");
		}
	}

    // Метод, который загружает слои
    private void LoadLevelFromStringLayers(string content) {
        // Разделим наш уровень на слоях с помощью новых вкладок (\ t)
        List<string> layers = new List<string>(content.Split('\t'));
		foreach (string layer in layers) {
			if (layer.Trim() != "") {
				LoadLevelFromString(int.Parse(layer[0].ToString()), layer.Substring(1));
			}
		}
	}

    // Метод, который загружает один слой
    private void LoadLevelFromString(int layer, string content) {
        // Разбиваем наш слой на строки по новым строкам (\ n)
        List<string> lines = new List<string>(content.Split('\n'));
        // Поместите каждый блок в правильное положение x и y
        for (int i = 0; i < lines.Count; i++) {
			string[] blockIDs = lines[i].Split(',');
			for (int j = 0; j < blockIDs.Length - 1; j++) {
				CreateBlock(TileStringRepresentationToInt(blockIDs[j]), j, lines.Count - i - 1, layer);
			}
		}
	}

    // Преобразует тип идентификации плитки, считанный из файла, в целое число, используемое как внутреннее представление в редакторе уровня
    // Для индекса, проанализируем строку в int
    // Для имени, поперечный Плитки и попытаться совместить имя игрового объекта или ПУСТОЙ
    // По умолчанию _empty (-1)
    private int TileStringRepresentationToInt(string tileString) {
		switch (_loadMethod) {
			case TileIdentificationMethod.Index:
				try {
					return int.Parse(tileString);
				}
				catch (FormatException) {
					Debug.LogError("Error: Trying to load a Name based level using Index loading");
					return _empty;
				}
				catch (ArgumentNullException) {
					Debug.LogError("Error: Encountered a null in the file");
					return _empty;
				}
			case TileIdentificationMethod.Name:
				if (tileString == "EMPTY")
					return _empty;
				for (int i = 0; i < _tiles.Count; i++) {
					if (_tiles[i].name == tileString) {
						return i;
					}
				}
				return _empty;
			default:
				return _empty;
		}
	}

    // Метод, который создает GameObject для заданного значения и позиции
    // Значение должно быть индексом в переменной _tiles, что приводит к созданию плитки
    private void CreateBlock(int value, int xPos, int yPos, int zPos) {
        // Если значение не пустое, установите его на правильную плиту
        if (value != _empty) {
			BuildBlock(_tiles[value], xPos, yPos, GetLayerParent(zPos).transform);
		}
	}

    // Создает блок, создавая его и устанавливая его родительский
    private void BuildBlock(Transform toCreate, int xPos, int yPos, Transform parent) {
        // Создаем объект, который хотим создать.
        Transform newObject = Instantiate(toCreate, new Vector3(xPos, yPos, toCreate.position.z), Quaternion.identity);
        // Дайте новому объекту то же имя, что и наш сборный плит
        newObject.name = toCreate.name;
        // Установите родительский объект в родительскую переменную слоя, чтобы он не загромождал нашу иерархию
        newObject.parent = parent;
	}

    // Возвращает родительский объект GameObject для слоя
    private GameObject GetLayerParent(int layer) {
		if (_layerParents.ContainsKey(layer))
			return _layerParents[layer];
		GameObject layerParent = new GameObject("Layer " + layer);
		layerParent.transform.parent = _tileLevelParent.transform;
		_layerParents.Add(layer, layerParent);
		return _layerParents[layer];
	}
}
