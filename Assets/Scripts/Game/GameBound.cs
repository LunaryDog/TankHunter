using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBound : MonoBehaviour {

    public int countTileHight = 10;
    public int countTileWight = 15;
    public float sizeTileHight = 1.28f;
    public float sizeTileWight = 1.28f;
    private static Boundary gameBound;

    private void Awake()
    {
        gameBound = new Boundary(countTileWight * sizeTileWight, countTileHight * sizeTileHight);
    }
    // Use this for initialization
    void Start () {
        Boundary boundWorld = WorldBound;
       
        GameObject gameBound = new GameObject("GameBound");

        GameObject righBound = new GameObject("RightBound");
        righBound.transform.parent = gameBound.transform;
        righBound.AddComponent<BoxCollider2D>();
        righBound.GetComponent<BoxCollider2D>().size = new Vector2(0.2f, boundWorld.UpBound * 2);
        righBound.transform.position = new Vector3(boundWorld.RightBound, 0, 0);


        GameObject leftBound = new GameObject("LeftBound");
        leftBound.transform.parent = gameBound.transform;
        leftBound.AddComponent<BoxCollider2D>();
        leftBound.GetComponent<BoxCollider2D>().size = new Vector2(0.2f, boundWorld.UpBound * 2);
        leftBound.transform.position = new Vector3(boundWorld.LeftBound, 0, 0);

        GameObject upBound = new GameObject("UpBound");
        upBound.transform.parent = gameBound.transform;
        upBound.AddComponent<BoxCollider2D>();
        upBound.GetComponent<BoxCollider2D>().size = new Vector2( boundWorld.RightBound * 2, .2f);
        upBound.transform.position = new Vector3(0,boundWorld.UpBound, 0);

        GameObject downBound = new GameObject("DownBound");
        downBound.transform.parent = gameBound.transform;
        downBound.AddComponent<BoxCollider2D>();
        downBound.GetComponent<BoxCollider2D>().size = new Vector2(boundWorld.RightBound * 2, .2f);
        downBound.transform.position = new Vector3(0, boundWorld.DownBound, 0);

    }
    public static Boundary WorldBound
    {
        get { return gameBound; }
    }
}
