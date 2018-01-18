using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyManagerEvent
{
    ENEMYDIE,
    WIN,
    ENEMYSPAWN
}

public class EnemyManager : BaseBehaviour {

    public List<GameObject> enemies;
    public int enemyMaxCount = 10;
    Observer observer;
    private int killedEnemy = 0; 

    void Awake()
    {
        observer = Observer.Instance;
        enemies = new List<GameObject>();
    }

    private void Start()
    {
        observer.AddListener(EnemyEvents.DIE, this, RemoveEnemyToList);
        observer.AddListener(EnemyEvents.SPAWN, this, AddEnemyToList);
        observer.SendMessage(EnemyManagerEvent.ENEMYDIE, enemyMaxCount - killedEnemy);
    }

    private void AddEnemyToList(ObservParam obj)
    {
        
        GameObject param = (GameObject)obj.data;
        enemies.Add(param);
        observer.SendMessage(EnemyManagerEvent.ENEMYSPAWN, enemyMaxCount - enemies.Count);
        // Debug.Log(enemies.Count);
    }

    private void RemoveEnemyToList(ObservParam obj)
    {
        GameObject param = (GameObject)obj.data;
        enemyMaxCount = enemyMaxCount - 1;
        enemies.Remove(param);
        observer.SendMessage(EnemyManagerEvent.ENEMYDIE, enemyMaxCount - killedEnemy);
        if (enemyMaxCount - killedEnemy <=0)
        {
            observer.SendMessage(EnemyManagerEvent.WIN);
        }
    }
    private void RemoveAllEnemys()
    {
        enemies.Clear();
    }
        
    public int CountFreePlaceForEnemy
    {
        get { return enemyMaxCount - enemies.Count; }
    }

    public int EnemyCount
    {
        get { return enemyMaxCount; }
        set { enemyMaxCount = value; }
    }
    
    public int KilledEnemy
    {
        get { return killedEnemy; }
    }

    private void OnDestroy()
    {
        RemoveAllEnemys();
        observer.RemoveAllListeners(this);
    }
}
