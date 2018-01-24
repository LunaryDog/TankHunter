using UnityEngine;
using UnityEngine.UI;

public class FillEditorScript: MonoBehaviour {

	private LevelEditor levelEditor;

    // Объекты пользовательского интерфейса для отображения режима карандаша / заливки
    private Texture2D fillCursor;

    // Boolean, чтобы определить, использовать ли режим заливки или режим карандаша
    private bool fillMode;

    // Объекты пользовательского интерфейса для отображения режима карандаша / заливки
    private Image pencilModeButtonImage;

    private Image fillModeButtonImage;

    // Цвет для отображения отключенного режима
    private static readonly Color32 DisabledColor = new Color32(150, 150, 150, 255);

    public void Setup(Texture2D fillCursorTexture) {
		levelEditor = LevelEditor.Instance;
		fillCursor = fillCursorTexture;
		SetupClickListeners();
     
        DisableFillMode();
    }

    // Подключить методы режима к кнопке режима
    private void SetupClickListeners() {
      
        GameObject pencilModeButton = Utilities.FindButtonAndAddOnClickListener("PencilButton", DisableFillMode);
        pencilModeButtonImage = pencilModeButton.GetComponent<Image>();
      
        GameObject fillModeButton = Utilities.FindButtonAndAddOnClickListener("FillButton", EnableFillMode);
        fillModeButtonImage = fillModeButton.GetComponent<Image>();
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
      
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (fillMode && levelEditor.ValidPosition((int) worldMousePosition.x, (int) worldMousePosition.y, 0)) {
            // Если допустимая позиция, установите курсор на ведро
            Cursor.SetCursor(fillCursor, new Vector2(30, 25), CursorMode.Auto);
        }
        else {
            // Повторный сброс курсора
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}

	public bool GetFillMode() {
		return fillMode;
    }

    // Переключить режим заполнения (между режимом заливки и карандаша)
    private void ToggleFillMode() {
		if (fillMode) {
			DisableFillMode();
		} else {
			EnableFillMode();
		}
    }

    // Включить режим заполнения и обновить интерфейс
    private void EnableFillMode() {
		fillMode = true;
		fillModeButtonImage.GetComponent<Image>().color = Color.black;
		pencilModeButtonImage.GetComponent<Image>().color = DisabledColor;
	}

    // Отключить режим заполнения и обновить интерфейс и курсор
    private void DisableFillMode() {
        fillMode = false;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		pencilModeButtonImage.GetComponent<Image>().color = Color.black;
		fillModeButtonImage.GetComponent<Image>().color = DisabledColor;
	}
}
