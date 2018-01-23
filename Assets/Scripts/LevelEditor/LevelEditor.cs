using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


// Enum используется для определения типа сохранения
public enum TileIdentificationMethod {
	Index,
	Name
}

[RequireComponent(typeof(SaveFunctionality))]
[RequireComponent(typeof(LoadFunctionality))]
[RequireComponent(typeof(UndoRedoFunctionality))]
[RequireComponent(typeof(ZoomFunctionality))]
[RequireComponent(typeof(FillFunctionality))]
[RequireComponent(typeof(LayerFunctionality))]
[RequireComponent(typeof(GridFunctionality))]
public class LevelEditor : MonoBehaviour {

    // Экземпляр LevelEditor
    public static LevelEditor Instance;
    public LevelData levelData;
    // Родительский объект интерфейса редактора уровней как prefab
    public GameObject LevelEditorUiPrefab;
    public LevelParser parser;
    public LevelCreate creator;

    // Объекты пользовательского интерфейса для отображения режима 
    public Texture2D FillCursor;

    // FileBrowser Prefab, чтобы открыть Save- и LoadFilePanel
    //public GameObject FileBrowserPrefab;

    // Расширение файла, используемое для сохранения и загрузки уровней
    public string FileExtension = "json";

    // Метод определения фрагментов при сохранении
    public TileIdentificationMethod SaveMethod;

    // Метод идентификации фрагментов при загрузке
    public TileIdentificationMethod LoadMethod;

    public int numberLevel = 1;
    // Значения X, Y и Z карты
    public int Height = 14;

    public int Width = 16;
		
	public int Layers = 10;

    public float tileSize = 1f;
   
    // Список плиток, которые пользователь может использовать для создания карт
    // Public, чтобы пользователь мог добавлять все созданные пользователем сборные файлы
    public Tilelist tileList;


    // Включен ли этот скрипт (false, если пользователь закрывает окно)
    private bool _scriptEnabled = true;

    // Закрытая переменная для сохранения списка Transforms открытого Tileset
    private List<Transform> _tiles;

    // Сценарий пользовательского интерфейса для редактора уровней
    private UserInterface _uiScript;

    // Функциональные возможности
    private SaveFunctionality _saveFunctionality;
    private LoadFunctionality _loadFunctionality;
	private UndoRedoFunctionality _undoRedoFunctionality;
	private FillFunctionality _fillFunctionality;
	private ZoomFunctionality _zoomFunctionality;
	private LayerFunctionality _layerFunctionality;
	private GridFunctionality _gridFunctionality;

    // Определение пустой плитки для карты
    private const int Empty = -1;

    // Внутреннее представление уровня (значения int)
    private int[,,] _level;

    // Внешнее представление уровня (преобразование)
    private Transform[,,] _gameObjects;

    // Используется для хранения текущего выбранного индекса и слоя плитки
    private int _selectedTileIndex = Empty;

    // GameObject как родительский для всех слоев (чтобы окно Иерархия было чистым)
    private GameObject _tileLevelParent;

    // Словарь как родительский для всех GameObjects на слой
    private Dictionary<int, GameObject> _layerParents = new Dictionary<int, GameObject>();

    // Трансформация, используемая для предварительного просмотра выбранной плитки на карте
    private Transform _previewTile;

    // Метод для создания экземпляра LevelEditor и предотвращения его уничтожения
    void Awake() {
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}
	}

    // Метод для создания зависимостей и переменных
    void Start() {
        // Метод для создания зависимостей и переменных
        ValidateStartValues();
        SetupLevelData();
        SetupLevel();
		SetupUi();
		SetupFunctionalities();
	}

    // Метод, который проверяет значения общедоступных переменных и при необходимости устанавливает их действительными значениями по умолчанию
    private void ValidateStartValues() {
    Width = Mathf.Clamp(Width, 1, Width);
		Height = Mathf.Clamp(Height, 1, Height);
		Layers = Mathf.Clamp(Layers, 1, Layers);

		if (tileList == null) {
			_tiles = new List<Transform>();
			Debug.LogError("No valid Tileset found");
		} else {
			_tiles = tileList.tiles;
		}
	}
    private void SetupLevelData()
    {
        levelData = new LevelData();
        levelData.levelProperties = new LevelProperties(numberLevel, Height, Width, tileSize);        
        
        levelData.layers = new List<Layer>();
        for (int i = 0; i < Layers; i ++)
        {
            Layer newLayer = new Layer("Layer " + i + 1);
           // newLayer.nameLayer = "Layer " + i + 1;
           // newLayer.dataLayer = new List<TileData>();          
           // newLayer.visibleLayer = false;
            levelData.layers.Add(newLayer);           
        }
    
        levelData.tileSets = new List<TileSet>();
        string pathTileSet = "";
        TileSet newTileset = new TileSet(tileList.tiles.Count, pathTileSet, "Tiles");
        levelData.tileSets.Add(newTileset);
      //  levelData.tilesets[0] = new Tileset();
      //  levelData.tilesets[0].tileCount = tileList.tiles.Count;
      //  levelData.tilesets[0].tileWidth = (int)tileWidth;
      //  levelData.tilesets[0].tileHeight = (int)tileHeight;
    }

    // Устанавливаем переменные и создаем пустой уровень с правильным размером
    private void SetupLevel() {
        // Получаем или создаем объект tileLevelParent, чтобы мы могли сделать его родительскими родителями вновь созданных объектов
        _tileLevelParent = GameObject.Find("Level") ?? new GameObject("Level");

        // Имитировать уровень и gameObject на пустой уровень и пустую трансформацию
        _level = CreateEmptyLevel();
        _gameObjects = new Transform[Width, Height, Layers];
	}

	private void SetupUi() {
        // Создание образа LevelEditorUI
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null) {
			GameObject levelEditorUi = Instantiate(LevelEditorUiPrefab, canvas.transform);
			_uiScript = levelEditorUi.GetComponent<UserInterface>();
		} else {
			Debug.LogError("Make sure there is a canvas GameObject present in the Hierarcy (Create UI/Canvas)");
		}
        // Настройка пользовательского интерфейса
        _uiScript.Setup();
        // Устанавливаем SelectedTile в Empty (-1) и обновляем selectedTileImage
        SetSelectedTile(Empty);
    }

    // Настройка различных функций редактора уровней
    private void SetupFunctionalities() {
         _saveFunctionality = GetComponent<SaveFunctionality>();
		_saveFunctionality.Setup( levelData, _tiles);
			
		_loadFunctionality = GetComponent<LoadFunctionality>();
	//	_loadFunctionality.Setup(FileBrowserPrefab, FileExtension, LoadMethod, _tiles);

		_undoRedoFunctionality = GetComponent<UndoRedoFunctionality>();
		_undoRedoFunctionality.Setup();

		_fillFunctionality = GetComponent<FillFunctionality>();
		_fillFunctionality.Setup(FillCursor);

		_zoomFunctionality = GetComponent<ZoomFunctionality>();
		_zoomFunctionality.Setup(Width, Height);

		_layerFunctionality = GetComponent<LayerFunctionality>();
		_layerFunctionality.Setup(Layers);

		_gridFunctionality = GetComponent<GridFunctionality>();
		_gridFunctionality.Setup(Width, Height);

        parser = GetComponent<LevelParser>();
        creator = GetComponent<LevelCreate>();
    }
   

        // Обработка ввода (создание и удаление при нажатии)
        private void Update() {
        // Продолжаем только если скрипт включен (редактор уровня открыт)
        if (!_scriptEnabled) return;
        // Обновить предварительную позицию плитки с учетом текущей позиции мыши относительно мировой точки
        UpdatePreviewTilePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // Получить позицию мыши перед нажатием
        Vector3 mousePos = Input.mousePosition;
        // Установите положение по оси z в противоположную сторону камеры, чтобы положение находилось в мире
        // поэтому ScreenToWorldPoint предоставит нам действительные значения.
        mousePos.z = Camera.main.transform.position.z * -1;
        mousePos.z = Camera.main.transform.position.z * -1;
		Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
        // Поделитесь с мышью не точно на блоке
        int posX = Mathf.FloorToInt(pos.x + .5f);
        int posY = Mathf.FloorToInt(pos.y + .5f);
        // Обработка ввода только при нажатии на действительную позицию
        HandleInput(posX, posY);
    }

    // Обновить предварительный просмотр
    private void UpdatePreviewTilePosition(Vector3 worldMousePosition) {
		if (_previewTile != null) {
			if (ValidPosition((int) worldMousePosition.x, (int) worldMousePosition.y, 0)) {
				_previewTile.position =
					new Vector3(Mathf.RoundToInt(worldMousePosition.x), Mathf.RoundToInt(worldMousePosition.y), -1);
			}
		}
	}

    // Проверяем щелчки мыши и обрабатываем соответственно
    private void HandleInput(int posX, int posY) {
    int selectedLayer = _layerFunctionality.GetSelectedLayer();
		if (!ValidPosition(posX, posY, selectedLayer)) {
			return;
		}
        // Щелкните левой кнопкой мыши - создайте объект (проверьте hotControl, а не интерфейс)
        if (Input.GetMouseButton(0) && GUIUtility.hotControl == 0 && !EventSystem.current.IsPointerOverGameObject()) {
            // Разрешаем только добавления, если selectedTile не является ПУСТОЙ (не может добавлять / заполнять ничего)
            if (_selectedTileIndex != Empty) {
                // Если режим заполнения, заполнение, другое положение щелчка (режим карандаша)
                if (_fillFunctionality.GetFillMode()) {
                Fill(posX, posY, selectedLayer, true);
				} else {
					ClickedPosition(posX, posY, selectedLayer);
				}
			}
		}
        // Щелкните правой кнопкой мыши - Удалить объект (проверьте hotControl, а не интерфейс)
        if (Input.GetMouseButton(1) && GUIUtility.hotControl == 0 && !EventSystem.current.IsPointerOverGameObject()) {
            // Если мы ударим что-то (! = EMPTY), мы хотим уничтожить объект и обновить массив gameObject и массив уровней
            if (_level[posX, posY, selectedLayer] != Empty) {
            DestroyBlock(posX, posY, selectedLayer);
				_level[posX, posY, selectedLayer] = Empty;
			}
            // Если мы ничего не удалим, а previewTile - null, удалите его
            else if (_previewTile != null) {
            DestroyImmediate(_previewTile.gameObject);
                // Установите выбранную плитку и изображение в ПУСТОЙ
                SetSelectedTile(Empty);
                SetSelectedTile(Empty);
			}
		}
	}


    // Метод для переключения selectedTile на выбор плитки
    // Используется clickListeners в скрипте UserInterface
    public void ButtonClick(int tileIndex) {
    SetSelectedTile(tileIndex);
		if (_previewTile != null) {
			DestroyImmediate(_previewTile.gameObject);
		}
		_previewTile = Instantiate(GetTiles()[_selectedTileIndex],
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100),
			Quaternion.identity);
		foreach (Collider2D c in _previewTile.GetComponents<Collider2D>()) {
			c.enabled = false;
		}
    }

    // Возвращает, включен ли сценарий (например, зарегистрирован ли вход)
    public bool GetScriptEnabled() {
		return _scriptEnabled;
    }

    // Возвращает статическое представление элемента EMPTY
    public static int GetEmpty() {
		return Empty;
    }

    // Возвращает уровень
    public int[,,] GetLevel() {
		return _level;
	}

    // Метод определения для заданных x, y, z, является ли позиция действительной (в пределах ширины, высоты и слоев)
    // Обновляет уровень (внутреннее и внешнее представление)
    public void SetLevel(int[,,] newLevel) {
    _level = newLevel;
		RebuildGameObjects();
    }

    // Возвращает массив Tiles
    public List<Transform> GetTiles() {
		return _tiles;
    }

    // Метод определения для заданных x, y, z, является ли позиция действительной (в пределах ширины, высоты и слоев)
    public bool ValidPosition(int x, int y, int z) {
		return x >= 0 && x < Width && y >= 0 && y < Height && z >= 0 && z < Layers;
    }

    // Метод, который создает объект GameObject при щелчке
    public void CreateBlock(int value, int xPos, int yPos, int zPos) {
        // Возвращение на недопустимые позиции
        if (!ValidPosition(xPos, yPos, zPos)) {
        return;
		}
        // Установите значение для представления внутреннего уровня
        _level[xPos, yPos, zPos] = value;
        TileData tileData = new TileData(value+1);
        tileData.x = xPos;
        tileData.y = Height- yPos -1;
       // Debug.Log(value);
          levelData.layers[zPos].dataLayer.Insert((int)(tileData.y*Width+tileData.x), tileData);
        // Если значение не пустое, установите его на правильную плиту
        if (value != Empty) {
        BuildBlock(GetTiles()[value], xPos, yPos, zPos, GetLayerParent(zPos).transform);
		}
	}

    // UpdateLayerVisibility, вызываемый функцией LoadFunctionality для удаления зависимости от скрипта LayerFunctionality
    public void UpdateLayerVisibility() {
    _layerFunctionality.UpdateLayerVisibility();
	}

    // Метод, который включает / отключает все слои.
    public void ToggleLayerParent(int layer, bool show) {
    GetLayerParent(layer).SetActive(show);
    }

    // Метод, который включает / отключает все слои.
    public void ToggleLayerParents(bool show) {
		foreach (GameObject layerParent in _layerParents.Values) {
			layerParent.SetActive(show);
		}
	}


    // Метод, который сбрасывает GameObjects и слои
    private void ResetTransformsAndLayers()
    {
        // Уничтожьте все внутри нашего текущего уровня, который создается динамически
        foreach (Transform child in _tileLevelParent.transform) {
        Destroy(child.gameObject);
		}
		_layerParents = new Dictionary<int, GameObject>();
	}

    // Метод, который сбрасывает уровень и GameObject перед загрузкой
    public void ResetBeforeLoad()
    {
        ResetTransformsAndLayers();
        // Сброс внутреннего представления
        _level = CreateEmptyLevel();
        // Сброс настроек отмены и повтора
        _undoRedoFunctionality.Reset();
    }

    // Метод создания пустого уровня путем прокрутки по высоте, ширине и слоям
    // и установив значение в EMPTY (-1)
    private int[,,] CreateEmptyLevel() {
    
    int[,,] emptyLevel = new int[Width, Height, Layers];
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				for (int z = 0; z < Layers; z++) {
					emptyLevel[x, y, z] = Empty;
                    TileData tileData = new TileData(0);
                    levelData.layers[z].dataLayer.Add(tileData);                  
				}
			}
		}  
		return emptyLevel;
	}
		
	private void BuildBlock(Transform toCreate, int xPos, int yPos, int zPos, Transform parent) {
        // Создаем объект, который хотим создать.
        Transform newObject = Instantiate(toCreate, new Vector3(xPos, yPos, toCreate.position.z), Quaternion.identity);
        // Дайте новому объекту то же имя, что и наш сборный плит
        newObject.name = toCreate.name;
        // Установите родительский объект в родительскую переменную слоя, чтобы он не загромождал нашу иерархию
        newObject.parent = parent;
        // Добавление нового объекта в массив gameObjects для правильного администрирования
        _gameObjects[xPos, yPos, zPos] = newObject;
    }

    private void DestroyBlock(int posX, int posY, int posZ) {
		DestroyImmediate(_gameObjects[posX, posY, posZ].gameObject);
	}

    // Восстановить уровень (например, после использования отмены / повтора)
    // Сброс Transforms и Layer, затем зациклируем массив уровня и создаем блоки
    private void RebuildGameObjects() {
    ResetTransformsAndLayers();
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				for (int z = 0; z < Layers; z++) {
					if (_level[x, y, z] != Empty) {
						BuildBlock(GetTiles()[_level[x, y, z]], x, y, z, GetLayerParent(z).transform);
					}
				}
			}
		}
	}

    // Заполнять из позиции рекурсивно. Только заполнять, если позиция действительна и пуста
    private void Fill(int posX, int posY, int selectedLayer, bool undoPush)
    {
        // Проверяем действительные и пустые
        if (ValidPosition(posX, posY, selectedLayer) && _level[posX, posY, selectedLayer] == Empty)
        {
            if (undoPush)
            {
                // Нажимаем уровень на undoStack, так как он изменится
                _undoRedoFunctionality.PushLevel(_level);
            }
            // Создаем блок в позиции
            CreateBlock(_selectedTileIndex, posX, posY, selectedLayer);
            // Заполнение x + 1, x-1, y + 1, y-1
            Fill(posX + 1, posY, selectedLayer, false);
            Fill(posX - 1, posY, selectedLayer, false);
			Fill(posX, posY + 1, selectedLayer, false);
			Fill(posX, posY - 1, selectedLayer, false);
		}
    }

    // Нажмите на позицию, так что проверьте, не совпадают ли они и (уничтожьте и), если необходимо
    private void ClickedPosition(int posX, int posY, int selectedLayer) {
        // Если это то же самое, просто сохраните предыдущий и ничего не сделайте, else (destroy and) build
        if (_level[posX, posY, selectedLayer] != _selectedTileIndex) {
            // Нажимаем уровень на undoStack, так как он изменится
            _undoRedoFunctionality.PushLevel(_level);
            // Если позиция не пуста, уничтожьте текущий элемент (используя массив gameObjects)
            if (_level[posX, posY, selectedLayer] != Empty) {
            DestroyBlock(posX, posY, selectedLayer);
			}
            // Создаем новый игровой объект
            CreateBlock(_selectedTileIndex, posX, posY, selectedLayer);
        }
    }

    // Метод установки переменной selectedTile и selectedTileImage
    private void SetSelectedTile(int tileIndex) {
        // Обновить выбранную переменную tile
        _selectedTileIndex = tileIndex;
        // Если EMPTY, установите для selectedTileImage значение noSelectedTileImage else соответствующему изображению Prefab tile
        _uiScript.SetSelectedTileImageSprite(tileIndex);
    }

    // Метод, возвращающий родительский объект GameObject для слоя
    private GameObject GetLayerParent(int layer) {
		if (_layerParents.ContainsKey(layer))
			return _layerParents[layer];
		GameObject layerParent = new GameObject("Layer " + layer);
		layerParent.transform.parent = _tileLevelParent.transform;
		_layerParents.Add(layer, layerParent);
		return _layerParents[layer];
	}

    // Включает / отключает редактор уровня (сценарий, наложение и панель)
    public void ToggleLevelEditor(bool enable) {
    _scriptEnabled = enable;
		GridOverlay.Instance.enabled = enable;
		_uiScript.ToggleLevelEditorPanel(enable);
	}

// Закрываем панель редактора уровня, режим тестового уровня
    public void CloseLevelEditorPanel() {
		_scriptEnabled = false;
		_uiScript.ToggleLevelEditorPanel(false);
		_uiScript.ToggleOpenButton(true);
	}

    // Откройте панель редактора уровня, режим редактора уровней
    public void OpenLevelEditorPanel() {
    _uiScript.ToggleLevelEditorPanel(true);
		_uiScript.ToggleOpenButton(false);
		_scriptEnabled = true;
	}
}
