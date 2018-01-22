using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesBulder : MonoBehaviour {

    public List<GameObject> prefubTiles;

    public int wightCountTiles;
    public int hightCountTiles;

    static float hightWorld;
    static float wightWorld;

    public static float HightWorld
    {
        get { return hightWorld; }
    }
    public static float WightWorld
    {
        get { return wightWorld; }
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
