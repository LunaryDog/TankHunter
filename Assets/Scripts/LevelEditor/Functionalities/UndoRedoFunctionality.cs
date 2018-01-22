using UnityEngine;

public class UndoRedoFunctionality : MonoBehaviour {

	private LevelEditor _levelEditor;

    // Стеки для отслеживания отмены и повтора
    private FiniteStack<int[,,]> _undoStack;

	private FiniteStack<int[,,]> _redoStack;


	public void Setup() {
		_levelEditor = LevelEditor.Instance;
		_undoStack = new FiniteStack<int[,,]>();
		_redoStack = new FiniteStack<int[,,]>();
		SetupClickListeners();
	}

    // Подключить метод отмены / повтора к кнопке отмены / повтора
    private void SetupClickListeners() {
        Utilities.FindButtonAndAddOnClickListener("UndoButton", Undo);
		Utilities.FindButtonAndAddOnClickListener("RedoButton", Redo);
	}

	private void Update() {
		
		if (Input.GetKeyDown(KeyCode.Z)) {
			Undo();
		}
		
		if (Input.GetKeyDown(KeyCode.Y)) {
			Redo();
		}
	}


    // Подключить метод отмены / повтора к кнопке отмены / повтора
    public void Reset() {
    _undoStack = new FiniteStack<int[,,]>();
		_redoStack = new FiniteStack<int[,,]>();
	}

    // Нажимаем уровень в стек отмены, тем самым сохраняя его состояние
    public void PushLevel(int[,,] level) {
    _undoStack.Push(level.Clone() as int[,,]);
    }


    // Нажимаем уровень в стек отмены, тем самым сохраняя его состояние
    private void Undo() {
        // Посмотрим, есть ли что-нибудь в стеке отмены
        if (_undoStack.Count > 0) {
            // Если это так, перетащите его в стек повтора
            _redoStack.Push(_levelEditor.GetLevel());
        }
        // Получить запись последнего уровня
        int[,,] undoLevel = _undoStack.Pop();
		if (undoLevel != null) {
            // Установите уровень в предыдущее состояние
            _levelEditor.SetLevel(undoLevel);
        }
    }

    // Загружаем последний сохраненный уровень из уровня повтора и уровня восстановления
    private void Redo() {
        // Посмотрим, есть ли что-нибудь в стеке повтора
        if (_redoStack.Count > 0) {
            // Если это так, перетащите его в стек повтора
            _undoStack.Push(_levelEditor.GetLevel());
        }
        // Получить запись последнего уровня
        int[,,] redoLevel = _redoStack.Pop();
        if (redoLevel != null) {
            // Установите уровень в предыдущее состояние
            _levelEditor.SetLevel(redoLevel);
        }
    }
}