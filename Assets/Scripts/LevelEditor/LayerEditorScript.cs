using UnityEngine;
using UnityEngine.UI;


public class LayerEditorScript : MonoBehaviour {
    
	private LevelEditor levelEditor;
	private int selectedLayer;
	private int totalyayers;

    // чтобы определить, показывать ли все слои или только текущий
    private bool onlyShowCurrentLayer;

    // Объекты пользовательского интерфейса для переключения 
    private GameObject layerEyeImage;

    private GameObject layerClosedEyeImage;
	private Toggle onlyShowCurrentLayerToggleComponent;

    // Текст, используемый для представления текущего выбранного слоя
    private Text layerText;

    public void Setup(int layers) {
		levelEditor = LevelEditor.Instance;
		totalyayers = layers;
		SetupClickListeners();
	}

    private void SetupClickListeners()
    {       
        Utilities.FindButtonAndAddOnClickListener("+LayerButton", LayerUp);
        Utilities.FindButtonAndAddOnClickListener("-LayerButton", LayerDown);

       
        layerEyeImage = GameObject.Find("LayerEyeImage");
        layerClosedEyeImage = GameObject.Find("LayerClosedEyeImage");
		onlyShowCurrentLayerToggleComponent =
		Utilities.FindGameObjectOrError("OnlyShowCurrentLayerToggle").GetComponent<Toggle>();
		onlyShowCurrentLayerToggleComponent.onValueChanged.AddListener(ToggleOnlyShowCurrentLayer);

        // Создаем экземпляр игрового объекта LayerText для отображения текущего слоя
        layerText = Utilities.FindGameObjectOrError("LayerText").GetComponent<Text>();
    }

   
    private void Update() {		
		UpdateLayerText();
	}
    	
	private void UpdateLayerText() {
		layerText.text = "" + (selectedLayer + 1);
	}
    	
	public int GetSelectedLayer() {
		return selectedLayer;
    }

    // Метод, который обновляет слои, которые должны отображаться
    // Public, поэтому его можно вызывать после загрузки уровня
    public void UpdateLayerVisibility() {
		if (onlyShowCurrentLayer) {
			OnlyShowCurrentLayer();
		} else {
			ShowAllLayers();
		}
	}
   
    private void LayerUp() {
        selectedLayer = Mathf.Min(selectedLayer + 1, totalyayers - 1);
		UpdateLayerVisibility();
	}
       
    private void LayerDown() {
        selectedLayer = Mathf.Max(selectedLayer - 1, 0);
		UpdateLayerVisibility();
	}

    // Метод, который управляет переключением пользовательского интерфейса, показывать только текущий слой
    private void ToggleOnlyShowCurrentLayer(bool enable) {
        onlyShowCurrentLayer = enable;

		layerEyeImage.SetActive(enable);
		layerClosedEyeImage.SetActive(!enable);
		if (enable) {
			onlyShowCurrentLayerToggleComponent.targetGraphic = layerEyeImage.GetComponent<Graphic>();
			OnlyShowCurrentLayer();
		} else {
			onlyShowCurrentLayerToggleComponent.targetGraphic = layerClosedEyeImage.GetComponent<Graphic>();
			ShowAllLayers();
		}
	}
        
    private void ShowAllLayers() {
        levelEditor.ToggleLayerParents(true);
	}

    // Метод, который отключает все слои, кроме данного
    private void OnlyShowCurrentLayer() {
        levelEditor.ToggleLayerParents(false);
		levelEditor.ToggleLayerParent(selectedLayer, true);
	}
}
