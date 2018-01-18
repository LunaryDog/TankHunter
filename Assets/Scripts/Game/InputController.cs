using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputEvents
{
    MOVE
}

public enum InputDirection
{
   // LeftUp = new Vector2(-1,1),
}
public class InputController : BaseBehaviour {
    Observer observer;

    private void Awake()
    {
        observer = Observer.Instance;
    }

    

    public void GetDirection (string direction)
    {

    }
    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }
}
