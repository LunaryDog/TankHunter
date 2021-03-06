﻿using System.Collections;
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
       
    }

    void Start()
    {
        observer.AddListener(LevelBulderEvens.LOAD, this, StartAfterLoad);
        observer.AddListener(PlayerEvents.DIE, this, LoseGameMenu);
        observer.AddListener(EnemyManagerEvent.WIN, this, WinGameMenu);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void StartAfterLoad(ObservParam obj)
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
