using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreManagerEvens
{
    SCORE
}

public class ScoreManager : BaseBehaviour {
    Observer observer;
    private int score = 0;
    private void Awake()
    {
        observer = Observer.Instance;
    }

    // Use this for initialization
    void Start () {
        observer.AddListener(EnemyEvents.SCORE, this, SetScore);
	}
	
    private void SetScore(ObservParam obj)
    {
        int param = (int)obj.data;
        score += param;
        observer.SendMessage(ScoreManagerEvens.SCORE, score);
    }
    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }
}
