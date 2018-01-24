using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(SaveEditorScript))]
[RequireComponent(typeof(LoadEditorScript))]
[RequireComponent(typeof(UndoRedoEditorScript))]
[RequireComponent(typeof(ZoomEditorScript))]
[RequireComponent(typeof(FillEditorScript))]
[RequireComponent(typeof(LayerEditorScript))]
[RequireComponent(typeof(GridEditorScript))]
public class LevelEditor : BaseBehaviour {
    
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

    [SerializeField]
    public int numberLevel = 1;
    // Значения X, Y и Z карты
    [SerializeField]
    public int Height = 5;
    [SerializeField]
    public int Width = 5;
    
    public int Layers;
   
    public float tileSize = 1.28f;
   
    // Список плиток, которые пользователь может использовать для создания карт
    // Public, чтобы пользователь мог добавлять все созданные пользователем сборные файлы
    public Tilelist tileList;
    public Tilelist objectsList;
    private object[] allLevels;

    // Включен ли этот скрипт (false, если пользователь закрывает окно)
    private bool scriptEnabled = true;

    // Закрытая переменная для сохранения списка Transforms открытого Tileset
    private List<Transform> tiles;
    private List<Transform> objects;

    // Сценарий пользовательского интерфейса для редактора уровней
    private UserInterface uiScript;

    // Функциональные возможности
    private SaveEditorScript save;
    private LoadEditorScript load;
	private UndoRedoEditorScript undoredo;
	private FillEditorScript fill;
	private ZoomEditorScript zoom;
	private LayerEditorScript layer;
	private GridEditorScript grid;

    // Определение пустой плитки для карты
    private const int Empty = -1;

    // Внутреннее представление уровня (значения int)
    private int[,,] level;

    // Внешнее представление уровня (преобразование)
    private Transform[,,] gameObjects;

    // Используется для хранения текущего выбранного индекса и слоя плитки
    private int selectedTileIndex = Empty;

    // GameObject как родительский для всех слоев (чтобы окно Иерархия было чистым)
    private GameObject tileLevelParent;

    // Словарь как родительский для всех GameObjects на слой
    private Dictionary<int, GameObject> layerParents = new Dictionary<int, GameObject>();

    // Трансформация, используемая для предварительного просмотра выбранной плитки на карте
    private Transform previewTile;

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
        LoadAllLevel();
        SetupLevelData();
        SetupLevel();
        SetupUi();
        SetupFunctionalities();

    }

    private void LoadAllLevel()
    {
        allLevels = (object[])Resources.LoadAll("Levels");       
    }
        // Метод, который проверяет значения общедоступных переменных и при необходимости устанавливает их действительными значениями по умолчанию
     private void ValidateStartValues() {
        numberLevel = Mathf.Clamp(numberLevel, 1, numberLevel);
        Width = Mathf.Clamp(Width, 5, 500);
		Height = Mathf.Clamp(Height, 5, 500);
		Layers = Mathf.Clamp(Layers, 1, Layers);


		if (tileList == null) {
			tiles = new List<Transform>();
			Debug.LogError("No valid Tileset found");
		} else {
			tiles = tileList.tiles;
		}
        if (objectsList == null)
        {
            objects = new List<Transform>();
            Debug.LogError("No valid objectset found");
        }
        else
        {
            objects = objectsList.tiles;
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
            levelData.layers.Add(newLayer);           
        }
        TileData emptyTile = new TileData(0);
        foreach (Layer layer in levelData.layers)
        {
            for (int i = 0; i < Height*Width; i++)
            {
                layer.dataLayer.Add(emptyTile);
            }
        }

        levelData.tileSets = new List<TileSet>();
        string pathTileSet = Application.dataPath + "/Resorces/TileSets/" + tileList.transform.name;
        TileSet newTileset = new TileSet(tileList.tiles.Count, pathTileSet, "Tiles");
        levelData.tileSets.Add(newTileset);
        pathTileSet = Application.dataPath + "/Resorces/TileSets/" + objectsList.transform.name;
        TileSet newObjectset = new TileSet(objectsList.tiles.Count, pathTileSet, "Objects");
        levelData.tileSets.Add(newObjectset);
    }


    public void UpdateEditorAfterLoad(LevelData newLevelData)
    {
        if (levelData.layers.Count < Layers)
        {
            TileData emptyTile = new TileData(0);
            for (int i = levelData.layers.Count; i < Layers; i++)
            {
                Layer newLayer = new Layer("Layer " + i + 1);
                for (int j = 0; j < Height * Width; j++)
                {
                    newLayer.dataLayer.Add(emptyTile);
                }
                levelData.layers.Add(newLayer);
            }           
          
        }
        levelData = newLevelData;
        numberLevel = levelData.levelProperties.numberLevel;
        Width = levelData.levelProperties.widthLevel;
        Height = levelData.levelProperties.heightLevel;
        tileSize = levelData.levelProperties.tileSize;
        grid.SetupGridOverlay(Width, Height);
    }

    // Устанавливаем переменные и создаем пустой уровень с правильным размером
    private void SetupLevel() {
        // Получаем или создаем объект tileLevelParent, чтобы мы могли сделать его родительскими родителями вновь созданных объектов
        tileLevelParent = GameObject.Find("Level") ?? new GameObject("Level");

        // Имитировать уровень и gameObject на пустой уровень и пустую трансформацию
        level = CreateEmptyLevel();
        gameObjects = new Transform[Width, Height, Layers];
	}

	private void SetupUi() {
        // Создание образа LevelEditorUI
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null) {
			GameObject levelEditorUi = Instantiate(LevelEditorUiPrefab, canvas.transform);
			uiScript = levelEditorUi.GetComponent<UserInterface>();
		} else {
			Debug.LogError("Make sure there is a canvas GameObject present in the Hierarcy (Create UI/Canvas)");
		}
        // Настройка пользовательского интерфейса
        uiScript.Setup();
        // Устанавливаем SelectedTile в Empty (-1) и обновляем selectedTileImage
        SetSelectedTile(Empty);
    }

    // Настройка различных функций редактора уровней
    private void SetupFunctionalities() {
         save = GetComponent<SaveEditorScript>();
		save.Setup( levelData, tiles, objects);
			
		load = GetComponent<LoadEditorScript>();
		load.Setup(tiles, objects);

		undoredo = GetComponent<UndoRedoEditorScript>();
		undoredo.Setup();

		fill = GetComponent<FillEditorScript>();
		fill.Setup(FillCursor);

		zoom = GetComponent<ZoomEditorScript>();
		zoom.Setup(Width, Height);

		layer = GetComponent<LayerEditorScript>();
		layer.Setup(Layers);

		grid = GetComponent<GridEditorScript>();
		grid.Setup(Width, Height);

        parser = GetComponent<LevelParser>();
        creator = GetComponent<LevelCreate>();
    }
   

        // Обработка ввода (создание и удаление при нажатии)
    private void Update() {
        // Продолжаем только если скрипт включен (редактор уровня открыт)
        if (!scriptEnabled) return;
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
        UpdateLevelProperties();
    }

    private void UpdateLevelProperties ()
    {     
        if  (numberLevel != levelData.levelProperties.numberLevel)
        {
            numberLevel = Mathf.Clamp(numberLevel, 1, 500);
            levelData.levelProperties.numberLevel = numberLevel;
        }

        if (Width != levelData.levelProperties.widthLevel)
        {
            Width = Mathf.Clamp(Width, 5, 500);
            levelData.levelProperties.widthLevel = Width;
            grid.SetupGridOverlay(Width, Height);
            ResetBeforeLoad();
            UdateTileData();
        }

        if (Height != levelData.levelProperties.heightLevel)
        {
            Height = Mathf.Clamp(Height, 5, 500);
            levelData.levelProperties.heightLevel = Height;
            grid.SetupGridOverlay(Width, Height);
            ResetBeforeLoad();
            UdateTileData();
        }
    }


    private void UdateTileData()
    {
        TileData emptyTile = new TileData(0);
        foreach (Layer layer in levelData.layers)
        {
            layer.dataLayer.Clear();
            for (int i = 0; i < Height*Width; i++)
            {
                layer.dataLayer.Add(emptyTile);
            }
        }
    }
    // Обновить предварительный просмотр
    private void UpdatePreviewTilePosition(Vector3 worldMousePosition) {
		if (previewTile != null) {
			if (ValidPosition((int) worldMousePosition.x, (int) worldMousePosition.y, 0)) {
				previewTile.position =
					new Vector3(Mathf.RoundToInt(worldMousePosition.x), Mathf.RoundToInt(worldMousePosition.y), -1);
			}
		}
	}

    // Проверяем щелчки мыши и обрабатываем соответственно
    private void HandleInput(int posX, int posY) {
    int selectedLayer = layer.GetSelectedLayer();
		if (!ValidPosition(posX, posY, selectedLayer)) {
			return;
		}
        // Щелкните левой кнопкой мыши - создайте объект (проверьте hotControl, а не интерфейс)
        if (Input.GetMouseButton(0) && GUIUtility.hotControl == 0 && !EventSystem.current.IsPointerOverGameObject()) {
            // Разрешаем только добавления, если selectedTile не является ПУСТОЙ (не может добавлять / заполнять ничего)
            if (selectedTileIndex != Empty) {
                // Если режим заполнения, заполнение, другое положение щелчка (режим карандаша)
                if (fill.GetFillMode()) {
                    Fill(posX, posY, selectedLayer, true);
				} else {
					ClickedPosition(posX, posY, selectedLayer);
				}
			}
		}
        // Щелкните правой кнопкой мыши - Удалить объект (проверьте hotControl, а не интерфейс)
        if (Input.GetMouseButton(1) && GUIUtility.hotControl == 0 && !EventSystem.current.IsPointerOverGameObject()) {
            // Если мы ударим что-то (! = EMPTY), мы хотим уничтожить объект и обновить массив gameObject и массив уровней
            if (level[posX, posY, selectedLayer] != Empty) {
                DestroyBlock(posX, posY, selectedLayer);
				level[posX, posY, selectedLayer] = Empty;
			}
            // Если мы ничего не удалим, а previewTile - null, удалите его
            else if (previewTile != null) {
                DestroyImmediate(previewTile.gameObject);
                // Установите выбранную плитку и изображение в ПУСТОЙ
                
                SetSelectedTile(Empty);
			}
		}
	}


    // Метод для переключения selectedTile на выбор плитки
    // Используется clickListeners в скрипте UserInterface
    public void ButtonClick(int tileIndex, bool isObject) {
        SetSelectedTile(tileIndex);
		if (previewTile != null) {
			DestroyImmediate(previewTile.gameObject);
		}
		previewTile = Instantiate(GetTiles()[selectedTileIndex],
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100),
			Quaternion.identity);
		foreach (Collider2D c in previewTile.GetComponents<Collider2D>()) {
			c.enabled = false;
		}
    }

  
    // Возвращает, включен ли сценарий (например, зарегистрирован ли вход)
    public bool GetScriptEnabled() {
		return scriptEnabled;
    }

    // Возвращает статическое представление элемента EMPTY
    public static int GetEmpty() {
		return Empty;
    }

    // Возвращает уровень
    public int[,,] GetLevel() {
		return level;
	}

    // Метод определения для заданных x, y, z, является ли позиция действительной (в пределах ширины, высоты и слоев)
    // Обновляет уровень (внутреннее и внешнее представление)
    public void SetLevel(int[,,] newLevel) {
        level = newLevel;
		RebuildGameObjects();
    }

    // Возвращает массив Tiles
    public List<Transform> GetTiles() {
		return tiles;
    }

    public List<Transform> GetObjects()
    {
        return objects;
    }

    public object[] GetLevels()
    {   
        return allLevels;
    }

    public void LoadLevelClick(int indexLevel)
    {
        string file = allLevels[indexLevel].ToString();
        load.LoadLevel(file);
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
        level[xPos, yPos, zPos] = value;
        TileData tileData = new TileData(value+1);
        tileData.x = xPos;
        tileData.y = Height- yPos -1;
        // Debug.Log(value);
        levelData.layers[zPos].dataLayer.RemoveAt((int)(tileData.y * Width + tileData.x));
        levelData.layers[zPos].dataLayer.Insert((int)(tileData.y*Width+tileData.x), tileData);
      
        if (value != Empty) {
        BuildBlock(GetTiles()[value], xPos, yPos, zPos, GetLayerParent(zPos).transform);
		}
	}

    // UpdateLayerVisibility, вызываемый функцией LoadFunctionality для удаления зависимости от скрипта LayerFunctionality
    public void UpdateLayerVisibility() {
        layer.UpdateLayerVisibility();
	}

    // Метод, который включает / отключает все слои.
    public void ToggleLayerParent(int layer, bool show) {
        GetLayerParent(layer).SetActive(show);
    }

    // Метод, который включает / отключает все слои.
    public void ToggleLayerParents(bool show) {
		foreach (GameObject layerParent in layerParents.Values) {
			layerParent.SetActive(show);
		}
	}


    // Метод, который сбрасывает GameObjects и слои
    private void ResetTransformsAndLayers()
    {   foreach (Layer layer in levelData.layers)
        {
            for (int i = 0; i < layer.dataLayer.Count; i++) {
                layer.dataLayer[0] = new TileData(0);
            }
        }
        // Уничтожьте все внутри нашего текущего уровня, который создается динамически
        foreach (Transform child in tileLevelParent.transform) {
            Destroy(child.gameObject);

		}
		layerParents = new Dictionary<int, GameObject>();
	}

    // Метод, который сбрасывает уровень и GameObject перед загрузкой
    public void ResetBeforeLoad()
    {
        ResetTransformsAndLayers();
        // Сброс внутреннего представления
        level = CreateEmptyLevel();
        // Сброс настроек отмены и повтора
        undoredo.Reset();
    }

    // Метод создания пустого уровня путем прокрутки по высоте, ширине и слоям
    // и установив значение в EMPTY (-1)
    private int[,,] CreateEmptyLevel() {
    
    int[,,] emptyLevel = new int[Width, Height, Layers];
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				for (int z = 0; z < Layers; z++) {
					emptyLevel[x, y, z] = Empty;                               
				}
			}
		}  
		return emptyLevel;
	}
		
	private void BuildBlock(Transform toCreate, int xPos, int yPos, int zPos, Transform parent) {
        // Создаем объект, который хотим создать.
        Transform newObject = Instantiate(toCreate, new Vector3(xPos, yPos, toCreate.position.z), Quaternion.identity);
        newObject.transform.localScale= new Vector3((1f / tileSize), (1f / tileSize), 1f);
        // Дайте новому объекту то же имя, что и наш сборный плит
        newObject.name = toCreate.name;
        // Установите родительский объект в родительскую переменную слоя, чтобы он не загромождал нашу иерархию
        newObject.parent = parent;
        // Добавление нового объекта в массив gameObjects для правильного администрирования
        gameObjects[xPos, yPos, zPos] = newObject;
    }

    private void DestroyBlock(int posX, int posY, int posZ) {
		DestroyImmediate(gameObjects[posX, posY, posZ].gameObject);
        int tile_x = posX;
        int tile_y = Height - posY - 1;
        TileData emptyTile = new TileData(0);        
        levelData.layers[posZ].dataLayer[(int)(tile_y * Width + tile_x)] = emptyTile;
    }

    // Восстановить уровень (например, после использования отмены / повтора)
    // Сброс Transforms и Layer, затем зациклируем массив уровня и создаем блоки
    private void RebuildGameObjects() {
        ResetTransformsAndLayers();
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				for (int z = 0; z < Layers; z++) {
					if (level[x, y, z] != Empty) {
						BuildBlock(GetTiles()[level[x, y, z]], x, y, z, GetLayerParent(z).transform);
                        levelData.layers[z].dataLayer[(Height -1-y) * Width + x].tileIndex = level[x, y, z];
                    }
				}
			}
		}
	}
   

    // Заполнять из позиции рекурсивно. Только заполнять, если позиция действительна и пуста
    private void Fill(int posX, int posY, int selectedLayer, bool undoPush)
    {
        // Проверяем действительные и пустые
        if (ValidPosition(posX, posY, selectedLayer) && level[posX, posY, selectedLayer] == Empty)
        {
            if (undoPush)
            {
                // Нажимаем уровень на undoStack, так как он изменится
                undoredo.PushLevel(level);
            }
            // Создаем блок в позиции
            CreateBlock(selectedTileIndex, posX, posY, selectedLayer);
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
        if (level[posX, posY, selectedLayer] != selectedTileIndex) {
            // Нажимаем уровень на undoStack, так как он изменится
            undoredo.PushLevel(level);
            // Если позиция не пуста, уничтожьте текущий элемент (используя массив gameObjects)
            if (level[posX, posY, selectedLayer] != Empty) {
                DestroyBlock(posX, posY, selectedLayer);
			}
            // Создаем новый игровой объект
            CreateBlock(selectedTileIndex, posX, posY, selectedLayer);
        }
    }

    // Метод установки переменной selectedTile и selectedTileImage
    private void SetSelectedTile(int tileIndex) {
        // Обновить выбранную переменную tile
        selectedTileIndex = tileIndex;
        // Если EMPTY, установите для selectedTileImage значение noSelectedTileImage else соответствующему изображению Prefab tile
        uiScript.SetSelectedTileImageSprite(tileIndex);
    }

    // Метод, возвращающий родительский объект GameObject для слоя
    private GameObject GetLayerParent(int layer) {
		if (layerParents.ContainsKey(layer))
			return layerParents[layer];
		GameObject layerParent = new GameObject("Layer " + layer);
		layerParent.transform.parent = tileLevelParent.transform;
		layerParents.Add(layer, layerParent);
		return layerParents[layer];
	}

    // Включает / отключает редактор уровня (сценарий, наложение и панель)
    public void ToggleLevelEditor(bool enable) {
        scriptEnabled = enable;
		GridOverlay.Instance.enabled = enable;
		uiScript.ToggleLevelEditorPanel(enable);
	}

    // Закрываем панель редактора уровня, режим тестового уровня
    public void CloseLevelEditorPanel() {
		scriptEnabled = false;
		uiScript.ToggleLevelEditorPanel(false);
		uiScript.ToggleOpenButton(true);
	}

    // Откройте панель редактора уровня, режим редактора уровней
    public void OpenLevelEditorPanel() {
        uiScript.ToggleLevelEditorPanel(true);
		uiScript.ToggleOpenButton(false);
		scriptEnabled = true;
	}
}
