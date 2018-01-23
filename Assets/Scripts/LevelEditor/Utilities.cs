using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

    
	public class Utilities : MonoBehaviour {

        // Находит и возвращает игровой объект по имени или печатает ошибку и возвращает null
        public static GameObject FindGameObjectOrError(string objectName) {
			GameObject foundGameObject = GameObject.Find(objectName);
			if (foundGameObject == null) {
				Debug.LogError("Make sure " + objectName + " is present");
			}
			return foundGameObject;
		}

        // Пытается найти кнопку по имени и добавить к ней слушателя
        // Возвращает результирующую кнопку
        public static GameObject FindButtonAndAddOnClickListener(string buttonName, UnityAction listenerAction) {
			GameObject button = FindGameObjectOrError(buttonName);
			button.GetComponent<Button>().onClick.AddListener(listenerAction);
			return button;
		}
	}
