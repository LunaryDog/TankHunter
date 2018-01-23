using UnityEngine;


public class GridOverlay : MonoBehaviour {

	public static GridOverlay Instance;

    // Размеры сетки
    private float _gridSizeX = 10;

	private float _gridSizeY = 10;
	private const int GridSizeZ = 0;

    // Шаги, предпринятые при перемещении сетки
    public float SmallStep = 0.5f;

    // Шаги для определения размеров каждого квадрата
    public float LargeStep = 1;

    // Начальная позиция
    public float StartX;

	public float StartY;
	public float StartZ;

    // Смещения
    private float _offsetX = -0.5f;

	private float _offsetY = -0.5f;

    // Материал сетки
    public Material LineMaterial;

    // Цвет сетки
    public Color MainColor = new Color(1f, 1f, 1f, 1f);

    // Метод для создания экземпляра GridOverlay и предотвращения его уничтожения
    void Awake() {
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}
	}

    // Изменяет размер ширины сетки
    public void SetGridSizeX(float x) {
		_gridSizeX = x;
	}

    // Изменяет размер высоты сетки
    public void SetGridSizeY(float y) {
		_gridSizeY = y;
	}

    // Обновляет размеры на квадрат с помощью 0.5
    public void GridSizeUp() {
		LargeStep += 0.5f;
	}

    // Обновление размеров на квадрат с -0.5
    public void GridSizeDown() {
		LargeStep -= 0.5f;
	}

    // Перемещаем сетку на величину smallStep
    public void GridUp() {
		_offsetY += SmallStep;
	}

    // Переместите сетку вниз на величину smallStep
    public void GridDown() {
		_offsetY -= SmallStep;
	}

    // Переместите сетку влево на величину smallStep
    public void GridLeft() {
		_offsetX -= SmallStep;
	}

    // Переместите сетку справа на величину smallStep
    public void GridRight() {
		_offsetX += SmallStep;
	}

	// Рисуем сетку
	void OnPostRender() {
        //  Удостоверьтесь, что largeStep никогда не <= 0, так как тогда программа вылетает
        LargeStep = Mathf.Max(LargeStep, 0.5f);
        // установить текущий материал
        LineMaterial.SetPass(0);

		GL.Begin(GL.LINES);

		GL.Color(MainColor);

		//Слои
		for (float j = 0; j <= _gridSizeY; j += LargeStep) {
            //X осевые линии
            for (float i = 0; i <= GridSizeZ; i += LargeStep) {
				GL.Vertex3(StartX + _offsetX, j + _offsetY, StartZ + i);
				GL.Vertex3(_gridSizeX + _offsetX, j + _offsetY, StartZ + i);
			}

            //Z осевые линии
            for (float i = 0; i <= _gridSizeX; i += LargeStep) {
				GL.Vertex3(StartX + i + _offsetX, j + _offsetY, StartZ);
				GL.Vertex3(StartX + i + _offsetX, j + _offsetY, GridSizeZ);
			}
		}

        //Y осевые линии
        for (float i = 0; i <= GridSizeZ; i += LargeStep) {
			for (float k = 0; k <= _gridSizeX; k += LargeStep) {
				GL.Vertex3(StartX + k + _offsetX, StartY + _offsetY, StartZ + i);
				GL.Vertex3(StartX + k + _offsetX, _gridSizeY + _offsetY, StartZ + i);
			}
		}

		GL.End();
	}
}

