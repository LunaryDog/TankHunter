using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameManagerEvent
{
    WIN,
    LOSE
}

public class GameManager : BaseBehaviour
{
    Observer observer;
    private float playTime;

    private static GameObject player;
   
    public static GameObject Player
    {
        get { return player; }
    }


    void Awake()
    {

        observer = Observer.Instance;
        player = GameObject.FindGameObjectWithTag("Player");  
    }

    void Start()
    {
        observer.AddListener(PlayerEvents.DIE, this, LoseGameMenu);
        observer.AddListener(EnemyManagerEvent.WIN, this, WinGameMenu);
    }

    void Update()
    {      
            playTime += Time.deltaTime;       
    }
    

    private void LoseGameMenu(ObservParam obj)
    {
        observer.SendMessage(GameManagerEvent.LOSE);
    }

    private void WinGameMenu(ObservParam obj)
    {
        observer.SendMessage(GameManagerEvent.WIN, playTime);
    }
    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }

}
