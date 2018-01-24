using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner :BaseBehaviour {
    Observer observer;

    public List<GameObject> botPrefabs;
    public float spawnInterval = 5f;
    private float spawnTime = 0f;
    public int maxEnemies = 3;
    private List<GameObject> spawnedEnemies;
    private int freeEnemyCount = 10;
    private Animator animator;
    private void Awake()
    {
        observer = Observer.Instance;
    }

    void Start()
    {
        GameObject player = GameObject.Find("Player");
        if (!player)
        {
            this.enabled = false;
        }
        spawnedEnemies = new List<GameObject>(maxEnemies);
        animator = gameObject.GetComponent<Animator>();
        observer.AddListener(EnemyManagerEvent.ENEMYSPAWN, this, SetFreeEnemyCount);
    }

    void Update()
    {
        spawnTime += Time.deltaTime;
        
        if (spawnTime >= spawnInterval)
        {
            RefreshEnemies();
            if( (spawnedEnemies.Count < maxEnemies) && (freeEnemyCount > 0)){
                SpawnEnemy();
               
            }
            spawnTime = 0f;
            //animator.SetBool("isSpawn", false);
        }
    }

    public void SpawnEnemy()
    {
        animator.SetBool("isSpawn", true);
        GameObject botPrefab = botPrefabs[Random.Range(0, botPrefabs.Count)];
        GameObject enemy = (GameObject)Instantiate(botPrefab, transform.position, Quaternion.identity);
        enemy.transform.parent = gameObject.transform;
        spawnedEnemies.Add(enemy);
        StartCoroutine(SetSpawnAnimation());

       
    }

    public void SetFreeEnemyCount(ObservParam obj)
    {
        int param = (int)obj.data;
        freeEnemyCount = param;
    }

    public void RefreshEnemies()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject go in spawnedEnemies)
        {
            if (!go)
                toRemove.Add(go);
        }
        foreach (GameObject t in toRemove)
            spawnedEnemies.Remove(t);
    }

    IEnumerator SetSpawnAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isSpawn", false);
    }

    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }
}
