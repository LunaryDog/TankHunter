using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerEvents
{
   HEALTH,
   RESISTANCE,
   DIE,
   DAMAGE
}


public class Player : SteeringAgent {

    Observer observer;
    public float maxPlayerVelocity = 3.5f;
    public float maxPlayerAcceleration = 10.0f;
    public float sizePlayer = 0.5f;

    private SteeringManager manager;
    private ArriveBehaviour arrive;    
    private GameObject moveObject;
    public float maxHealth = 100f;
    private float health;
    public float maxResistance = 100;
    private float resistance;
    private float stepResistance = 0.5f;
    public ParticleSystem dieParticle;

    void Awake()
    {
        observer = Observer.Instance;
    }

    // Use this for initialization
    void Start () {
        SteeringBound = GameBound.WorldBound;
        moveObject = new GameObject();
        moveObject.transform.parent = gameObject.transform;
        manager = transform.GetComponent<SteeringManager>();
        arrive = transform.GetComponent<ArriveBehaviour>();
        MaxVelocity = maxPlayerVelocity;
        MaxAcceleration = maxPlayerAcceleration;
        Size = sizePlayer;
        health = maxHealth;
        resistance = maxResistance;
        observer.SendMessage(PlayerEvents.HEALTH, health);
        observer.SendMessage(PlayerEvents.RESISTANCE, resistance);
        observer.AddListener(InputEvents.MOVE, this, MovePlayer);
        observer.AddListener(EnemyEvents.DAMAGE, this, GetDamage);
        observer.AddListener(TileEvens.DAMAGE, this, GetDamage);
        observer.AddListener(TileEvens.SLOW, this, GetDeceleration);
        if (dieParticle)
        {
            dieParticle.Stop();
        }
    }

   
    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        Vector2 movePoint = new Vector2(h, v);
        observer.SendMessage(InputEvents.MOVE, movePoint);


    }

     void MovePlayer(ObservParam obj)
    {
        Vector2 param = (Vector2)obj.data;
        Debug.Log(param.magnitude);
        Vector3 movePoint = Position +  new Vector3(param.x, param.y, 0);
        moveObject.transform.position = movePoint;
        arrive.target = moveObject;
        manager.Move();
        if (Velocity.magnitude > 0.0005f)
        {
            RemoveResistance(Velocity.magnitude * Time.deltaTime);
        }
    }

    private void GetDamage(ObservParam obj)
    {
        float param = (float)obj.data;
        health = health - param;
        observer.SendMessage(PlayerEvents.HEALTH, health);
        if (health <= 0f)
        {
        
            //observer.SendMessage(PlayerEvents.DIE);
            Die();
        }
    }

    private void GetDeceleration(ObservParam obj)
    {
        float slow = (float)obj.data;
        MaxVelocity = maxPlayerVelocity * slow;
    }

    private void RemoveResistance(float lostResistent)
    {
        resistance = resistance - lostResistent * stepResistance;
        observer.SendMessage(PlayerEvents.RESISTANCE, resistance);
        if (resistance <= 0f)
        {
           observer.SendMessage(PlayerEvents.DIE);
        }
    }

    void Die()
    {
        dieParticle.Play();
        StartCoroutine(Destroy());

    }
    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1);
        observer.SendMessage(PlayerEvents.DIE);
        Destroy(gameObject);
    }
}