using UnityEngine;
using UnityEngine.UI;


public class LayerFunctionality : MonoBehaviour {
    
	private LevelEditor _levelEditor;

	private int _selectedLayer;

	private int _totalyayers;

    // Boolean, чтобы определить, показывать ли все слои или только текущий
    private bool _onlyShowCurrentLayer;

    // Объекты пользовательского интерфейса для переключения 
    private GameObject _layerEyeImage;

    private GameObject _layerClosedEyeImage;
	private Toggle _onlyShowCurrentLayerToggleComponent;

    // Текст, используемый для представления текущего выбранного слоя
    private Text _layerText;

    public void Setup(int layers) {
		_levelEditor = LevelEditor.Instance;
		_totalyayers = layers;
		SetupClickListeners();
	}

    // Подключите методы слоя к кнопке Layer
    private void SetupClickListeners()
    {
        // Подключить методы смены слоя к кнопкам смены слоев
         Utilities.FindButtonAndAddOnClickListener("+LayerButton", LayerUp);
        Utilities.FindButtonAndAddOnClickListener("-LayerButton", LayerDown);

        // Подключить метод ToggleOnlyShowCurrentLayer к OnlyShowCurrentLayerToggle
        _layerEyeImage = GameObject.Find("LayerEyeImage");
        _layerClosedEyeImage = GameObject.Find("LayerClosedEyeImage");
		_onlyShowCurrentLayerToggleComponent =
			Utilities.FindGameObjectOrError("OnlyShowCurrentLayerToggle").GetComponent<Toggle>();
		_onlyShowCurrentLayerToggleComponent.onValueChanged.AddListener(ToggleOnlyShowCurrentLayer);

        // Создаем экземпляр игрового объекта LayerText для отображения текущего слоя
        _layerText = Utilities.FindGameObjectOrError("LayerText").GetComponent<Text>();
    }

   
    private void Update() {		
		UpdateLayerText();
	}
    	
	private void UpdateLayerText() {
		_layerText.text = "" + (_selectedLayer + 1);
	}
    	
	public int GetSelectedLayer() {
		return _selectedLayer;
    }

    // Метод, который обновляет слои, которые должны отображаться
    // Public, поэтому его можно вызывать после загрузки уровня
    public void UpdateLayerVisibility() {
		if (_onlyShowCurrentLayer) {
			OnlyShowCurrentLayer();
		} else {
			ShowAllLayers();
		}
	}

    // Метод, который увеличивает выбранный уровень
    private void LayerUp() {
    _selectedLayer = Mathf.Min(_selectedLayer + 1, _totalyayers - 1);
		UpdateLayerVisibility();
	}

    // Метод, который уменьшает выбранный уровень
    private void LayerDown() {
    _selectedLayer = Mathf.Max(_selectedLayer - 1, 0);
		UpdateLayerVisibility();
	}

    // Метод, который управляет переключением пользовательского интерфейса, показывать только текущий слой
    private void ToggleOnlyShowCurrentLayer(bool enable) {
    _onlyShowCurrentLayer = enable;

		_layerEyeImage.SetActive(enable);
		_layerClosedEyeImage.SetActive(!enable);
		if (enable) {
			_onlyShowCurrentLayerToggleComponent.targetGraphic = _layerEyeImage.GetComponent<Graphic>();
			OnlyShowCurrentLayer();
		} else {
			_onlyShowCurrentLayerToggleComponent.targetGraphic = _layerClosedEyeImage.GetComponent<Graphic>();
			ShowAllLayers();
		}
	}

    // Метод, который позволяет всем слоям
    private void ShowAllLayers() {
    _levelEditor.ToggleLayerParents(true);
	}

    // Метод, который отключает все слои, кроме данного
    private void OnlyShowCurrentLayer() {
    _levelEditor.ToggleLayerParents(false);
		_levelEditor.ToggleLayerParent(_selectedLayer, true);
	}
}
