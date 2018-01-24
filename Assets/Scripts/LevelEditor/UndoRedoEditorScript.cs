using UnityEngine;

public class UndoRedoEditorScript : MonoBehaviour {

	private LevelEditor levelEditor;

    // Стеки для отслеживания отмены и повтора
    private FiniteStack<int[,,]> undoStack;

	private FiniteStack<int[,,]> redoStack;


	public void Setup() {
		levelEditor = LevelEditor.Instance;
		undoStack = new FiniteStack<int[,,]>();
		redoStack = new FiniteStack<int[,,]>();
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
        undoStack = new FiniteStack<int[,,]>();
		redoStack = new FiniteStack<int[,,]>();
	}

    // Нажимаем уровень в стек отмены, тем самым сохраняя его состояние
    public void PushLevel(int[,,] level) {
        undoStack.Push(level.Clone() as int[,,]);
    }


    // Нажимаем уровень в стек отмены, тем самым сохраняя его состояние
    private void Undo() {
       
        if (undoStack.Count > 0) {           
            redoStack.Push(levelEditor.GetLevel());
        }
        int[,,] undoLevel = undoStack.Pop();
		if (undoLevel != null) {
            levelEditor.SetLevel(undoLevel);
        }
    }

    // Загружаем последний сохраненный уровень из уровня повтора и уровня восстановления
    private void Redo() {
        if (redoStack.Count > 0) {
            undoStack.Push(levelEditor.GetLevel());
        }
        int[,,] redoLevel = redoStack.Pop();
        if (redoLevel != null) {
            levelEditor.SetLevel(redoLevel);
        }
    }
}