using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIEvents
{
    HEALTH,
    ENEMY,
    RESISTANCE,
    TIME,
    SCORE
}
public class UIScreenManager : BaseBehaviour {
    public Text textHealth;
    public Text textResistance;
    public Text textEnemy;
    public Text textTime;

    public Slider healthSlider;
    public Slider resistanceSlider;

    Observer observer;
    
    float health = 100f;  
    float resistance = 100f;    
    int enemyCount = 0;    
    int scoore = 0;   
    float time = 0;

    private void Awake()
    {
        observer = Observer.Instance;
    }
    void Start () {
        observer.AddListener(PlayerEvents.HEALTH,this,SetHealth);
        observer.AddListener(PlayerEvents.RESISTANCE, this, SetResistance);
        observer.AddListener(EnemyManagerEvent.ENEMYDIE, this, SetEnemyCount);        
        observer.AddListener(ScoreManagerEvens.SCORE, this, SetScooreCount);
        healthSlider.maxValue = health;
        resistanceSlider.maxValue = resistance;
        
    }

    private void Update()
    {
        time += Time.deltaTime;
        int seconds = (int)time;
        SetTimeCount(seconds);
    }

    private void SetHealth(ObservParam obj)
    {
        float param = (float)obj.data;
        health = param;
        textHealth.text = health.ToString();
        healthSlider.value = health;
    }

    private void SetResistance(ObservParam obj)
    {
        float param = (float)obj.data;
        resistance = param;
        textResistance.text = resistance.ToString();
        resistanceSlider.value = resistance;
    }

    private void SetEnemyCount(ObservParam obj)
    {
        int param = (int)obj.data;
        enemyCount = param;
        textEnemy.text = enemyCount.ToString();
    }

    private void SetScooreCount(ObservParam obj)
    {
        int param = (int)obj.data;
        scoore = param;

    }
    
    private void SetTimeCount(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;
        string min = minutes.ToString();
        string sec = seconds.ToString();

        if (minutes < 10)
        {
            min = "0" + minutes;
        }
        if (seconds < 10)
        {
            sec = "0" + seconds;
        }
        textTime.text = min + ":" +sec;
    }
    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }

}

