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
    private bool isDied = false;
    private SteeringManager manager;
    private ArriveBehaviour arrive;    
    private GameObject moveObject;
    public HealthProperties health  = new HealthProperties();
    public ResistanceProperties resistance = new ResistanceProperties();

    public ParticleSystem dieParticle;
    Animator animator;

    void Awake()
    {
        observer = Observer.Instance;
    }

    
    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        moveObject = new GameObject();
        SteeringBound = GameBound.WorldBound;       
        moveObject.transform.parent = gameObject.transform;
        manager = transform.GetComponent<SteeringManager>();
        arrive = transform.GetComponent<ArriveBehaviour>();
        MaxVelocity = maxPlayerVelocity;
        MaxAcceleration = maxPlayerAcceleration;
        Size = sizePlayer;        
        observer.SendMessage(PlayerEvents.HEALTH, health.Value);
        observer.SendMessage(PlayerEvents.RESISTANCE, resistance.Value);
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
        if (!isDied) {
            float param = (float)obj.data;
            health.RemoveValue(param);

            observer.SendMessage(PlayerEvents.HEALTH, health.Value);
            if (health.Value <= 0f)
            {
        
                //observer.SendMessage(PlayerEvents.DIE);
                Die();
            }
        }
    }

    private void GetDeceleration(ObservParam obj)
    {
        float slow = (float)obj.data;
        MaxVelocity = maxPlayerVelocity * slow;
    }

    private void RemoveResistance(float lostResistent)
    {
        if (!isDied)
        {
            resistance.RemoveValue(lostResistent);
           
            observer.SendMessage(PlayerEvents.RESISTANCE, resistance.Value);
            if (resistance.Value <= 0f)
            {
              
                Die();
            }
    }   }

    void Die()
    {
        if (dieParticle) {
            dieParticle.Play();
        }
        StartCoroutine(Destroy());
        isDied = true;
    }
    private void OnDestroy()
    {
        observer.RemoveAllListeners(this);
        Destroy(manager);
    }

    IEnumerator Destroy()
    {
      
        yield return new WaitForSeconds(0.5f);
        //Destroy(gameObject);
        observer.SendMessage(PlayerEvents.DIE);
        
    }
}