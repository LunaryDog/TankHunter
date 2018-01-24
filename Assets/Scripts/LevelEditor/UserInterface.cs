using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
    // Кнопка Prefab используется для создания кнопок выбора плитки для каждого GameObjects.
    public GameObject ButtonPrefab;
    public GameObject ButtonLevelPrefab;
    // Размеры, используемые для представления кнопок выбора плитки GameObject
    // Представлен с использованием ползунка 0-200 в редакторе
    [Range(1.0f, 200.0f)] public float ButtonSize = 100;

    // Размеры, используемые для представления выбранного игрового объекта плитки
    // Представлен с использованием ползунка 0-200 в редакторе
    [Range(1.0f, 200.0f)] public float SelectedTileSize = 100;

    // Масштабирование изображений в отношении общего размера прямоугольника изображения
    [Range(0.1f, 1.0f)] public float ButtonImageScale = 0.8f;

    // Sprite, чтобы указать, что в данный момент не выбран плит
    public Sprite NoSelectedTileImage;

    // Редактор уровня с использованием этого пользовательского интерфейса
    private LevelEditor levelEditor;

    // Панель пользовательского интерфейса, используемая для хранения параметров редактора уровней
    private GameObject levelEditorPanel;

    // Открываем кнопку, чтобы открыть редактор уровня после его закрытия.
    private GameObject openButton;

    // GameObject используется для отображения выбранной в данный момент плитки
    private GameObject selectedTile;

    // Изображение для отображения текущей выбранной плитки
    private Image selectedTileImage;

    // GameObject как родительский для всех GameObject в выборе плитки
    private GameObject prefabParent;

    // GameObject как родительский для всех GameObject в выборе плитки
    private GameObject objectsParent;
    private GameObject levelsParent;

    public void Setup() {
		name = ("LevelEditorUI");
		levelEditor = LevelEditor.Instance;
		levelEditorPanel = Utilities.FindGameObjectOrError("LevelEditorPanel");
		SetupOpenCloseButton();
		SetupSelectedTile();
		SetupPrefabsButtons();
       // SetupObjectsButtons();
        SetupLevelsButtons();

        UpdatePrefabButtonsSize();
        
       // Button button = Utilities.FindGameObjectOrError("Collapse Level Button").GetComponent<Button>();
       // button.OnClick();
    }

	private void SetupOpenCloseButton() {
        // Подключить метод CloseLevelEditorPanel к CloseButton
        Utilities.FindButtonAndAddOnClickListener("CloseButton", levelEditor.CloseLevelEditorPanel);

        // Подключить метод OpenLevelEditorPanel к OpenButton и отключить его при запуске
        openButton = Utilities.FindButtonAndAddOnClickListener("OpenButton", levelEditor.OpenLevelEditorPanel);
		openButton.SetActive(false);
	}

	private void SetupSelectedTile() {
		selectedTile = Utilities.FindGameObjectOrError("SelectedTile");
        // Найти компонент изображения в SelectedTileImage GameObject
        selectedTileImage = Utilities.FindGameObjectOrError("SelectedTileImage").GetComponent<Image>();
	}

	private void SetupPrefabsButtons() {
        // Найти объект prefabParent и установить cellSize для кнопок выбора плитки
        prefabParent = Utilities.FindGameObjectOrError("Prefabs");
		if (prefabParent.GetComponent<GridLayoutGroup>() == null) {
			Debug.LogError("Make sure prefabParent has a GridLayoutGroup component");
		}
        // Счетчик, чтобы определить, какая кнопка плитки нажата
        int tileCounter = 0;
        // Создаем кнопку для каждой плитки в плитке
        foreach (Transform tile in levelEditor.GetTiles()) {
			int index = tileCounter;
			GameObject button = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity);
			button.name = tile.name;
			button.GetComponent<Image>().sprite = tile.gameObject.GetComponent<SpriteRenderer>().sprite;
			button.transform.SetParent(prefabParent.transform, false);
			button.transform.localScale = new Vector3(ButtonImageScale, ButtonImageScale, ButtonImageScale);
            // Добавление обработчика клика к кнопке
            button.GetComponent<Button>().onClick.AddListener(() => { levelEditor.ButtonClick(index, false); });
			tileCounter++;
		}
	}

    private void SetupObjectsButtons()
    {
        // Найти объект prefabParent и установить cellSize для кнопок выбора плитки
        objectsParent = Utilities.FindGameObjectOrError("Objects");
        if (objectsParent.GetComponent<GridLayoutGroup>() == null)
        {
            Debug.LogError("Make sure objectsParent has a GridLayoutGroup component");
        }
        // Счетчик, чтобы определить, какая кнопка плитки нажата
        int objectsCounter = 0;
        // Создаем кнопку для каждой плитки в плитке
        foreach (Transform objects in levelEditor.GetObjects())
        {
            int index = objectsCounter;
            GameObject button = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity);
            button.name = objects.name;
            button.GetComponent<Image>().sprite = objects.gameObject.GetComponent<SpriteRenderer>().sprite;
            button.transform.SetParent(objectsParent.transform, false);
            button.transform.localScale = new Vector3(ButtonImageScale, ButtonImageScale, ButtonImageScale);
            // Добавление обработчика клика к кнопке
            button.GetComponent<Button>().onClick.AddListener(() => { levelEditor.ButtonClick(index, true); });
            objectsCounter++;
        }
    }

    private void SetupLevelsButtons()
    {
        // Найти объект prefabParent и установить cellSize для кнопок выбора плитки
        levelsParent = Utilities.FindGameObjectOrError("Levels");
      //  if (levelsParent.GetComponent<GridLayoutGroup>() == null)
     //   {
       //     Debug.LogError("Make sure levelsParent has a GridLayoutGroup component");
       // }
        // Счетчик, чтобы определить, какая кнопка плитки нажата
        int levelsCounter = 0;
        // Создаем кнопку для каждой плитки в плитке
        foreach (object levels in levelEditor.GetLevels())
        {           
            int index = levelsCounter;
            GameObject button = Instantiate(ButtonLevelPrefab, Vector3.zero, Quaternion.identity);
            button.name = (levelsCounter + 1).ToString();
            button.GetComponent<Text>().text = "Level " + button.name;
            button.transform.SetParent(levelsParent.transform, false);
            button.transform.localScale = new Vector3(ButtonImageScale, 1f, 1f);
            // Добавление обработчика клика к кнопке
            button.GetComponent<Button>().onClick.AddListener(() => { levelEditor.LoadLevelClick(index); });
            levelsCounter++;
        }
    }
    // Обновляет пользовательский интерфейс, поэтому он настраивается во время выполнения
    void Update() {
        // Продолжаем только если скрипт включен (редактор уровня открыт), и ошибок нет
        if (!levelEditor.GetScriptEnabled()) return;
        // Обновляем размер кнопки для масштабирования во время выполнения
        UpdatePrefabButtonsSize();
        // Обновление выбранного игрового объекта плитки во время выполнения
        UpdateSelectedTileSize();
	}

    // Обновляем размер объектов prefab tile, изображения будут квадратными, чтобы сохранить соотношение сторон оригинала
    private void UpdatePrefabButtonsSize() {
		prefabParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(ButtonSize, ButtonSize);
	}

    // Обновляем размер выбранного игрового объекта плитки, изображения будут масштабироваться до половины
    private void UpdateSelectedTileSize() {
		selectedTile.GetComponent<RectTransform>().sizeDelta = new Vector2(SelectedTileSize, SelectedTileSize);
		selectedTileImage.GetComponent<RectTransform>().sizeDelta = new Vector2(SelectedTileSize / 2, SelectedTileSize / 2);
	}

    // Включает / отключает панель редактора уровней
    public void ToggleLevelEditorPanel(bool enable) {
		levelEditorPanel.SetActive(enable);
	}

    // Включает / отключает кнопку «Открыть» (инвертирует переключатель «Редактор уровня»)
    public void ToggleOpenButton(bool enable) {
		openButton.SetActive(enable);
	}

    // Обновляет выбранное изображение плитки.
    // Или устанавливает его в NoSelectedTileImage, когда tileIndex пуст (по умолчанию -1)
    // Или для спрайта выбранной плитки
    public void SetSelectedTileImageSprite(int tileIndex) {
		selectedTileImage.sprite = (tileIndex == LevelEditor.GetEmpty()
			? NoSelectedTileImage
			: levelEditor.GetTiles()[tileIndex].gameObject.GetComponent<SpriteRenderer>().sprite);
	}
}
