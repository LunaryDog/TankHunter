using UnityEngine;

public class MoveCamera : MonoBehaviour {
    public float PanSpeed = 4.0f;  // Скорость камеры при запуске

    private Vector3 _mouseOrigin; // Позиция курсора при запуске мыши
    private bool _isPanning; // Запускается ли камера?

    void Update() {
        // Получить правую кнопку мыши
        if (Input.GetMouseButtonDown(1)) {
            // Получить мышь
            _mouseOrigin = Input.mousePosition;
            _isPanning = true;
        }

        // Отключить движения при отпускании кнопки
        if (!Input.GetMouseButton(1)) _isPanning = false;

        // Переместите камеру на XY-плоскость
        if (!_isPanning) return;
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);
        Vector3 move = new Vector3(pos.x * PanSpeed, pos.y * PanSpeed, 0);
        transform.Translate(move, Space.Self);
    }
   
}