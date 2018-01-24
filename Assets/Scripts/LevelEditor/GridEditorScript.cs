using UnityEngine;
using UnityEngine.UI;

public class GridEditorScript : MonoBehaviour {

    // Объекты пользовательского интерфейса для переключения сетки
    private GameObject gridEyeImage;

    private GameObject gridClosedEyeImage;
	private Toggle gridEyeToggleComponent;
	

	public void Setup(int width, int height) {
		SetupClickListeners();      
        SetupGridOverlay(width, height);       
        ToggleGrid(true);
    }

    // Подключить методы Grid к кнопке Grid
    private void SetupClickListeners() {
       
        GameObject gridEyeToggle = Utilities.FindGameObjectOrError("GridEyeToggle");
        gridEyeImage = Utilities.FindGameObjectOrError("GridEyeImage");
		gridClosedEyeImage = Utilities.FindGameObjectOrError("GridClosedEyeImage");
		gridEyeToggleComponent = gridEyeToggle.GetComponent<Toggle>();
		gridEyeToggleComponent.onValueChanged.AddListener(ToggleGrid);
       
        Utilities.FindButtonAndAddOnClickListener("GridSizeUpButton", GridOverlay.Instance.GridSizeUp);
		Utilities.FindButtonAndAddOnClickListener("GridSizeDownButton", GridOverlay.Instance.GridSizeDown);
       
        Utilities.FindButtonAndAddOnClickListener("GridUpButton", GridOverlay.Instance.GridUp);
        Utilities.FindButtonAndAddOnClickListener("GridDownButton", GridOverlay.Instance.GridDown);
		Utilities.FindButtonAndAddOnClickListener("GridLeftButton", GridOverlay.Instance.GridLeft);
		Utilities.FindButtonAndAddOnClickListener("GridRightButton", GridOverlay.Instance.GridRight);
	}

    // Определим размеры уровней как размеры для сетки
    public void SetupGridOverlay(int width, int height) {
        GridOverlay.Instance.SetGridSizeX(width );
		GridOverlay.Instance.SetGridSizeY(height );
	}


    // Метод, который переключает сетку
    private void ToggleGrid(bool enable) {
        GridOverlay.Instance.enabled = enable;		
		gridEyeImage.SetActive(!enable);
		gridClosedEyeImage.SetActive(enable);
		gridEyeToggleComponent.targetGraphic =
			enable ? gridClosedEyeImage.GetComponent<Image>() : gridEyeImage.GetComponent<Image>();
	}
}
