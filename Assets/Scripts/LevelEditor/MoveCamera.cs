using UnityEngine;

public class MoveCamera : MonoBehaviour {
    public float PanSpeed = 1.0f;  // Скорость камеры при запуске

    private Vector3 mouseOrigin; 
    private bool isPanning; // Запускается ли камера?

    void Update() {
        // Получить правую кнопку мыши
        if (Input.GetMouseButtonDown(1)) {
            // Получить мышь
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        // Отключить движения при отпускании кнопки
        if (!Input.GetMouseButton(1)) isPanning = false;

        // Переместите камеру на XY-плоскость
        if (!isPanning) return;
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
        Vector3 move = new Vector3(pos.x * PanSpeed, pos.y * PanSpeed, 0);
        transform.Translate(move, Space.Self);
    }
   
}