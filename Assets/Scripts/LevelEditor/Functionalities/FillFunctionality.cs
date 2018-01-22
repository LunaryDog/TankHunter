using UnityEngine;
using UnityEngine.UI;

public class FillFunctionality : MonoBehaviour {

	private LevelEditor _levelEditor;

    // Объекты пользовательского интерфейса для отображения режима карандаша / заливки
    private Texture2D _fillCursor;

    // Boolean, чтобы определить, использовать ли режим заливки или режим карандаша
    private bool _fillMode;

    // Объекты пользовательского интерфейса для отображения режима карандаша / заливки
    private Image _pencilModeButtonImage;

    private Image _fillModeButtonImage;

    // Цвет для отображения отключенного режима
    private static readonly Color32 DisabledColor = new Color32(150, 150, 150, 255);

    public void Setup(Texture2D fillCursor) {
		_levelEditor = LevelEditor.Instance;
		_fillCursor = fillCursor;
		SetupClickListeners();
        // Изначально отключить режим заполнения
        DisableFillMode();
    }

    // Подключить методы режима к кнопке режима
    private void SetupClickListeners() {
        // Подключить метод EnablePencilMode к PencilButton
        GameObject pencilModeButton = Utilities.FindButtonAndAddOnClickListener("PencilButton", DisableFillMode);
        _pencilModeButtonImage = pencilModeButton.GetComponent<Image>();
        // Подключить метод EnableFillMode к FillButton
        GameObject fillModeButton = Utilities.FindButtonAndAddOnClickListener("FillButton", EnableFillMode);
        _fillModeButtonImage = fillModeButton.GetComponent<Image>();
	}

	private void Update() {
        // Если нажать F, переключите FillMode;
        if (Input.GetKeyDown(KeyCode.F)) {
        ToggleFillMode();
		}
		
		UpdateCursor();
	}

    // Обновить курсор (только показывать курсор на сетке)
    private void UpdateCursor() {
        // Сохранить точку мира, щелкнув мышью
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (_fillMode && _levelEditor.ValidPosition((int) worldMousePosition.x, (int) worldMousePosition.y, 0)) {
            // Если допустимая позиция, установите курсор на ведро
            Cursor.SetCursor(_fillCursor, new Vector2(30, 25), CursorMode.Auto);
        }
        else {
        // Повторный сброс курсора
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}

	public bool GetFillMode() {
		return _fillMode;
    }

    // Переключить режим заполнения (между режимом заливки и карандаша)
    private void ToggleFillMode() {
		if (_fillMode) {
			DisableFillMode();
		} else {
			EnableFillMode();
		}
    }

    // Включить режим заполнения и обновить интерфейс
        private void EnableFillMode() {
		_fillMode = true;
		_fillModeButtonImage.GetComponent<Image>().color = Color.black;
		_pencilModeButtonImage.GetComponent<Image>().color = DisabledColor;
	}

    // Отключить режим заполнения и обновить интерфейс и курсор
    private void DisableFillMode() {
    _fillMode = false;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		_pencilModeButtonImage.GetComponent<Image>().color = Color.black;
		_fillModeButtonImage.GetComponent<Image>().color = DisabledColor;
	}
}
