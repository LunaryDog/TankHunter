using UnityEngine;
using UnityEngine.UI;

public class GridFunctionality : MonoBehaviour {

    // Объекты пользовательского интерфейса для переключения сетки
    private GameObject _gridEyeImage;

    private GameObject _gridClosedEyeImage;
	private Toggle _gridEyeToggleComponent;
	

	public void Setup(int width, int height) {
		SetupClickListeners();
        // Настройка наложения сетки
        SetupGridOverlay(width, height);
        // Сетка инициализации
        ToggleGrid(true);
    }

    // Подключить методы Grid к кнопке Grid
    private void SetupClickListeners() {
        // Подключить метод ToggleGrid к GridToggle
        GameObject gridEyeToggle = Utilities.FindGameObjectOrError("GridEyeToggle");
        _gridEyeImage = Utilities.FindGameObjectOrError("GridEyeImage");
		_gridClosedEyeImage = Utilities.FindGameObjectOrError("GridClosedEyeImage");
		_gridEyeToggleComponent = gridEyeToggle.GetComponent<Toggle>();
		_gridEyeToggleComponent.onValueChanged.AddListener(ToggleGrid);

        // Подключить методы размера сетки к кнопкам размера сетки
        Utilities.FindButtonAndAddOnClickListener("GridSizeUpButton", GridOverlay.Instance.GridSizeUp);
		Utilities.FindButtonAndAddOnClickListener("GridSizeDownButton", GridOverlay.Instance.GridSizeDown);

        // Подключить сетку. Методы навигации к кнопкам навигации Grid.
        Utilities.FindButtonAndAddOnClickListener("GridUpButton", GridOverlay.Instance.GridUp);
        Utilities.FindButtonAndAddOnClickListener("GridDownButton", GridOverlay.Instance.GridDown);
		Utilities.FindButtonAndAddOnClickListener("GridLeftButton", GridOverlay.Instance.GridLeft);
		Utilities.FindButtonAndAddOnClickListener("GridRightButton", GridOverlay.Instance.GridRight);
	}

    // Определим размеры уровней как размеры для сетки
    private void SetupGridOverlay(int width, int height) {
    GridOverlay.Instance.SetGridSizeX(width);
		GridOverlay.Instance.SetGridSizeY(height);
	}


    // Метод, который переключает сетку
    private void ToggleGrid(bool enable) {
    GridOverlay.Instance.enabled = enable;
		// Update UI 
		_gridEyeImage.SetActive(!enable);
		_gridClosedEyeImage.SetActive(enable);
		_gridEyeToggleComponent.targetGraphic =
			enable ? _gridClosedEyeImage.GetComponent<Image>() : _gridEyeImage.GetComponent<Image>();
	}
}
