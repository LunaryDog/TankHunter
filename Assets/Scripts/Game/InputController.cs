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
    Dictionary<string, Vector2> direction;
    GameObject player;
    private void Awake()
    {
        observer = Observer.Instance;
    }

    private void Start()
    {
        direction = new Dictionary<string, Vector2>
        {   {"LeftUp",    new Vector2 (-1,1 ) },
            {"Up",        new Vector2(0,1)},
            {"RightUp",   new Vector2(1,1)},
            {"Left",      new Vector2(-1,0)},
            {"Right",     new Vector2(1,0)},
            {"LeftDown",  new Vector2(-1,-1)},
            {"Down",      new Vector2(0,-1)},
            {"RightDown", new Vector2(1,-1)},
            {"Stop",      new Vector2(0,0)}
        };

#if UNITY_EDITOR
     this.enabled = false;
#endif
#if UNITY_EDITOR_WIN
        this.enabled = false;
# endif 
    }

    public void GetDirection (string dir)
    {
        observer.SendMessage(InputEvents.MOVE, direction[dir]);
    }
    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }
}
