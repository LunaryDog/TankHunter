using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : BaseBehaviour {
    Observer observer;
    private Transform player;
    private Boundary gameBound;
    float wightCamera;
    float hightCamera;

    private void Awake()
    {
        observer = Observer.Instance;
    }

    // Use this for initialization
    void Start () {
        observer.AddListener(LevelBulderEvens.LOAD, this, StartAfterLoad);
	}
	
    void StartAfterLoad(ObservParam obj)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameBound = GameBound.WorldBound;
        wightCamera = Camera.main.orthographicSize * Camera.main.aspect;
        hightCamera = Camera.main.orthographicSize;
    }

	// Update is called once per frame
	void Update () {
       
        if ((player) )
        {
            transform.position = new Vector3(Mathf.Clamp(player.position.x, gameBound.LeftBound + wightCamera, gameBound.RightBound - wightCamera), Mathf.Clamp(player.position.y, gameBound.DownBound + hightCamera, gameBound.UpBound - hightCamera), transform.position.z);
        }
		
	}


    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }
}
