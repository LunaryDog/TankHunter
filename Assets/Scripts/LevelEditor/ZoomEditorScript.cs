using UnityEngine;

public class ZoomEditorScript : MonoBehaviour {

    private Camera mainCameraComponent;
    private float mainCameraInitialSize;

    // Найдите камеру, расположите ее в середине нашего уровня и сохраните начальный уровень масштабирования
    public void Setup(int width, int height) {
    GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null) {
            mainCamera.transform.position = new Vector3(width / 2.0f, height / 2.0f, mainCamera.transform.position.z);
            // Сохранять начальный уровень масштабирования
            mainCameraComponent = mainCamera.GetComponent<Camera>();
            mainCameraInitialSize = mainCameraComponent.orthographic
                ? mainCameraComponent.orthographicSize
                : mainCameraComponent.fieldOfView;
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
        if (mainCameraComponent.orthographic) {
            mainCameraComponent.orthographicSize = Mathf.Max(mainCameraComponent.orthographicSize - 1, 1);
        } else {
            mainCameraComponent.fieldOfView = Mathf.Max(mainCameraComponent.fieldOfView - 1, 1);
        }
    }


    private void ZoomOut() {
        if (mainCameraComponent.orthographic) {
            mainCameraComponent.orthographicSize += 1;
        } else {
            mainCameraComponent.fieldOfView += 1;
        }
    }


    private void ZoomDefault() {
        if (mainCameraComponent.orthographic) {
            mainCameraComponent.orthographicSize = mainCameraInitialSize;
        } else {
            mainCameraComponent.fieldOfView = mainCameraInitialSize;
        }
    }
 }