using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileEvens
{
    DAMAGE,
    SLOW
}
[RequireComponent(typeof(BoxCollider2D))]
public class Tile : BaseBehaviour
{
    Observer observer;
    public float friction_koeff = 0.9f;
    public float damage = 0f;

    private void Awake()
    {
        observer = Observer.Instance;
    }
    public virtual void Start()
    {
        gameObject.tag = "Tile";
        gameObject.GetComponent<BoxCollider2D>().size = gameObject.GetComponent<SpriteRenderer>().size;
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger == true)
        {
            observer.SendMessage(TileEvens.DAMAGE, damage);
            observer.SendMessage(TileEvens.SLOW, friction_koeff);
        }
    }


}