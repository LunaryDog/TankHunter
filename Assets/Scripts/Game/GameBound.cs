using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBound : BaseBehaviour {
    Observer observer;
    //public int countTileHight = 10;
   // public int countTileWight = 15;
   // public float sizeTileHight = 1.28f;
   // public float sizeTileWight = 1.28f;
    private static Boundary gameBound;

    private void Awake()
    {
        observer = Observer.Instance;
        // gameBound = new Boundary(countTileWight * sizeTileWight, countTileHight * sizeTileHight);
        //
    }
    // Use this for initialization
    private void Start()
    {
        observer.AddListener(LevelBulderEvens.LOAD, this, StartAfterLoad);
        gameBound = new Boundary(LevelBulder.WidhtWorld, LevelBulder.HeightWorld);
    }

    void StartAfterLoad (ObservParam obj) {
        //gameBound = new Boundary(LevelBulder.WidhtWorld, LevelBulder.HeightWorld);
        Boundary boundWorld = WorldBound;
       
        GameObject gameBoundObj = new GameObject("GameBound");

        GameObject righBound = new GameObject("RightBound");
        righBound.transform.parent = gameBoundObj.transform;
        righBound.AddComponent<BoxCollider2D>();
        righBound.GetComponent<BoxCollider2D>().size = new Vector2(0.2f, boundWorld.UpBound * 2);
        righBound.transform.position = new Vector3(boundWorld.RightBound, 0, 0);


        GameObject leftBound = new GameObject("LeftBound");
        leftBound.transform.parent = gameBoundObj.transform;
        leftBound.AddComponent<BoxCollider2D>();
        leftBound.GetComponent<BoxCollider2D>().size = new Vector2(0.2f, boundWorld.UpBound * 2);
        leftBound.transform.position = new Vector3(boundWorld.LeftBound, 0, 0);

        GameObject upBound = new GameObject("UpBound");
        upBound.transform.parent = gameBoundObj.transform;
        upBound.AddComponent<BoxCollider2D>();
        upBound.GetComponent<BoxCollider2D>().size = new Vector2( boundWorld.RightBound * 2, .2f);
        upBound.transform.position = new Vector3(0,boundWorld.UpBound, 0);

        GameObject downBound = new GameObject("DownBound");
        downBound.transform.parent = gameBoundObj.transform;
        downBound.AddComponent<BoxCollider2D>();
        downBound.GetComponent<BoxCollider2D>().size = new Vector2(boundWorld.RightBound * 2, .2f);
        downBound.transform.position = new Vector3(0, boundWorld.DownBound, 0);

    }
    public static Boundary WorldBound
    {
        get { return gameBound; }
    }

    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }
}
