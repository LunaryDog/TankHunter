using UnityEngine;

public class ZoomFunctionality : MonoBehaviour {

    private Camera _mainCameraComponent;
    private float _mainCameraInitialSize;

    // Найдите камеру, расположите ее в середине нашего уровня и сохраните начальный уровень масштабирования
    public void Setup(int width, int height) {
    GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null) {
            mainCamera.transform.position =
                new Vector3(width / 2.0f, height / 2.0f, mainCamera.transform.position.z);
            // Сохранять начальный уровень масштабирования
            _mainCameraComponent = mainCamera.GetComponent<Camera>();
            _mainCameraInitialSize = _mainCameraComponent.orthographic
                ? _mainCameraComponent.orthographicSize
                : _mainCameraComponent.fieldOfView;
            SetupClickListeners();
        } else {
            Debug.LogError("Object with tag MainCamera not found");
        }
    }

    // Подключить методы масштабирования к кнопкам масштабирования
    private void SetupClickListeners() {
        Utilities.FindButtonAndAddOnClickListener("ZoomInButton", ZoomIn);
        Utilities.FindButtonAndAddOnClickListener("ZoomOutButton", ZoomOut);
        Utilities.FindButtonAndAddOnClickListener("ZoomDefaultButton", ZoomDefault);
    }

    private void Update() {
       
        if (Input.GetKeyDown(KeyCode.Equals)) {
            ZoomIn();
        }
      
        if (Input.GetKeyDown(KeyCode.Minus)) {
            ZoomOut();
        }
     
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            ZoomDefault();
        }
    }

 
    private void ZoomIn() {
        if (_mainCameraComponent.orthographic) {
            _mainCameraComponent.orthographicSize = Mathf.Max(_mainCameraComponent.orthographicSize - 1, 1);
        } else {
            _mainCameraComponent.fieldOfView = Mathf.Max(_mainCameraComponent.fieldOfView - 1, 1);
        }
    }


    private void ZoomOut() {
        if (_mainCameraComponent.orthographic) {
            _mainCameraComponent.orthographicSize += 1;
        } else {
            _mainCameraComponent.fieldOfView += 1;
        }
    }


    private void ZoomDefault() {
        if (_mainCameraComponent.orthographic) {
            _mainCameraComponent.orthographicSize = _mainCameraInitialSize;
        } else {
            _mainCameraComponent.fieldOfView = _mainCameraInitialSize;
        }
    }
 }