using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
    private Transform player;
    private Boundary gameBound;

	// Use this for initialization
	void Start () {      
       
            player = GameManager.Player.transform;
            gameBound =GameBound.WorldBound;
       
	}
	
	// Update is called once per frame
	void Update () {
        float wightCamera = Camera.main.orthographicSize * Camera.main.aspect;
        float hightCamera = Camera.main.orthographicSize;
        if ((player) )
        {
            transform.position = new Vector3(Mathf.Clamp(player.position.x, gameBound.LeftBound + wightCamera, gameBound.RightBound - wightCamera), Mathf.Clamp(player.position.y, gameBound.DownBound + hightCamera, gameBound.UpBound - hightCamera), transform.position.z);
        }
		
	}
}
