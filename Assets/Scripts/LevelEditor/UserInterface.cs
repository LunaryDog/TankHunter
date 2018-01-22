using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
    // Кнопка Prefab используется для создания кнопок выбора плитки для каждого GameObjects.
    public GameObject ButtonPrefab;

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
    private LevelEditor _levelEditor;

    // Панель пользовательского интерфейса, используемая для хранения параметров редактора уровней
    private GameObject _levelEditorPanel;

    // Открываем кнопку, чтобы открыть редактор уровня после его закрытия.
    private GameObject _openButton;

    // GameObject используется для отображения выбранной в данный момент плитки
    private GameObject _selectedTile;

    // Изображение для отображения текущей выбранной плитки
    private Image _selectedTileImage;

    // GameObject как родительский для всех GameObject в выборе плитки
    private GameObject _prefabParent;


	public void Setup() {
		name = ("LevelEditorUI");
		_levelEditor = LevelEditor.Instance;
		_levelEditorPanel = Utilities.FindGameObjectOrError("LevelEditorPanel");
		SetupOpenCloseButton();
		SetupSelectedTile();
		SetupPrefabsButtons();
		
		UpdatePrefabButtonsSize();
	}

	private void SetupOpenCloseButton() {
        // Подключить метод CloseLevelEditorPanel к CloseButton
        Utilities.FindButtonAndAddOnClickListener("CloseButton", _levelEditor.CloseLevelEditorPanel);

        // Подключить метод OpenLevelEditorPanel к OpenButton и отключить его при запуске
        _openButton = Utilities.FindButtonAndAddOnClickListener("OpenButton", _levelEditor.OpenLevelEditorPanel);
		_openButton.SetActive(false);
	}

	private void SetupSelectedTile() {
		_selectedTile = Utilities.FindGameObjectOrError("SelectedTile");
        // Найти компонент изображения в SelectedTileImage GameObject
        _selectedTileImage = Utilities.FindGameObjectOrError("SelectedTileImage").GetComponent<Image>();
	}

	private void SetupPrefabsButtons() {
        // Найти объект prefabParent и установить cellSize для кнопок выбора плитки
        _prefabParent = Utilities.FindGameObjectOrError("Prefabs");
		if (_prefabParent.GetComponent<GridLayoutGroup>() == null) {
			Debug.LogError("Make sure prefabParent has a GridLayoutGroup component");
		}
        // Счетчик, чтобы определить, какая кнопка плитки нажата
        int tileCounter = 0;
        // Создаем кнопку для каждой плитки в плитке
        foreach (Transform tile in _levelEditor.GetTiles()) {
			int index = tileCounter;
			GameObject button = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity);
			button.name = tile.name;
			button.GetComponent<Image>().sprite = tile.gameObject.GetComponent<SpriteRenderer>().sprite;
			button.transform.SetParent(_prefabParent.transform, false);
			button.transform.localScale = new Vector3(ButtonImageScale, ButtonImageScale, ButtonImageScale);
            // Добавление обработчика клика к кнопке
            button.GetComponent<Button>().onClick.AddListener(() => { _levelEditor.ButtonClick(index); });
			tileCounter++;
		}
	}

    // Обновляет пользовательский интерфейс, поэтому он настраивается во время выполнения
    void Update() {
        // Продолжаем только если скрипт включен (редактор уровня открыт), и ошибок нет
        if (!_levelEditor.GetScriptEnabled()) return;
        // Обновляем размер кнопки для масштабирования во время выполнения
        UpdatePrefabButtonsSize();
        // Обновление выбранного игрового объекта плитки во время выполнения
        UpdateSelectedTileSize();
	}

    // Обновляем размер объектов prefab tile, изображения будут квадратными, чтобы сохранить соотношение сторон оригинала
    private void UpdatePrefabButtonsSize() {
		_prefabParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(ButtonSize, ButtonSize);
	}

    // Обновляем размер выбранного игрового объекта плитки, изображения будут масштабироваться до половины
    private void UpdateSelectedTileSize() {
		_selectedTile.GetComponent<RectTransform>().sizeDelta = new Vector2(SelectedTileSize, SelectedTileSize);
		_selectedTileImage.GetComponent<RectTransform>().sizeDelta = new Vector2(SelectedTileSize / 2, SelectedTileSize / 2);
	}

    // Включает / отключает панель редактора уровней
    public void ToggleLevelEditorPanel(bool enable) {
		_levelEditorPanel.SetActive(enable);
	}

    // Включает / отключает кнопку «Открыть» (инвертирует переключатель «Редактор уровня»)
    public void ToggleOpenButton(bool enable) {
		_openButton.SetActive(enable);
	}

    // Обновляет выбранное изображение плитки.
    // Или устанавливает его в NoSelectedTileImage, когда tileIndex пуст (по умолчанию -1)
    // Или для спрайта выбранной плитки
    public void SetSelectedTileImageSprite(int tileIndex) {
		_selectedTileImage.sprite = (tileIndex == LevelEditor.GetEmpty()
			? NoSelectedTileImage
			: _levelEditor.GetTiles()[tileIndex].gameObject.GetComponent<SpriteRenderer>().sprite);
	}
}
