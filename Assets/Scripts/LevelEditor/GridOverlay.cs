using UnityEngine;


public class GridOverlay : MonoBehaviour {

	public static GridOverlay Instance;

    // Размеры сетки
    private float gridSizeX = 10;

	private float gridSizeY = 10;
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
    private float offsetX = -0.5f;

	private float offsetY = -0.5f;

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
		gridSizeX = x;
	}

    // Изменяет размер высоты сетки
    public void SetGridSizeY(float y) {
		gridSizeY = y;
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
		offsetY += SmallStep;
	}

    // Переместите сетку вниз на величину smallStep
    public void GridDown() {
		offsetY -= SmallStep;
	}

    // Переместите сетку влево на величину smallStep
    public void GridLeft() {
		offsetX -= SmallStep;
	}

    // Переместите сетку справа на величину smallStep
    public void GridRight() {
		offsetX += SmallStep;
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
		for (float j = 0; j <= gridSizeY; j += LargeStep) {
            //X осевые линии
            for (float i = 0; i <= GridSizeZ; i += LargeStep) {
				GL.Vertex3(StartX + offsetX, j + offsetY, StartZ + i);
				GL.Vertex3(gridSizeX + offsetX, j + offsetY, StartZ + i);
			}

            //Z осевые линии
            for (float i = 0; i <= gridSizeX; i += LargeStep) {
				GL.Vertex3(StartX + i + offsetX, j + offsetY, StartZ);
				GL.Vertex3(StartX + i + offsetX, j + offsetY, GridSizeZ);
			}
		}

        //Y осевые линии
        for (float i = 0; i <= GridSizeZ; i += LargeStep) {
			for (float k = 0; k <= gridSizeX; k += LargeStep) {
				GL.Vertex3(StartX + k + offsetX, StartY + offsetY, StartZ + i);
				GL.Vertex3(StartX + k + offsetX, gridSizeY + offsetY, StartZ + i);
			}
		}

		GL.End();
	}
}

