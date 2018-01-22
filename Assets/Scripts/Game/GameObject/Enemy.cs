 using UnityEngine;
using System.Collections;

public enum EnemyEvents
{
    SPAWN,
    WANDER,
    ATTACK,
    EVADE,
    SCORE,
    DIE,
    DAMAGE
};
[System.Serializable]
public class Enemy : SteeringAgent {
    Observer observer;
    private EvadeBehaviour evade;    
    private CollisionAvoidanceBehaviour colAvoid;
    private WallAvoidanceBehaviour wallAvoid;
    //private GameManager gameManager;
    private WanderBehaviour wander;
    private SeekBehaviour seek;
    private EnemyEvents state = EnemyEvents.WANDER;
    private SteeringAgent player;
    private GameObject playerObject = GameManager.Player;
    bool isDie = false;
    SteeringManager manager;
    public float damage = 0.5f;
    public float maxEnemyVelocity = 1.0f;
    public float maxEnemyAcceleration = 3.0f;
    public float sizeEnemy = 0.3f;
   // Vector3 targetPoint;
    Animator animator;
    public float attackDistance = 1.5f;    
    private float wanderMaxVelocity = 1f;
    private float evadeMaxVelocity = 2f;

    public PropertiesBar resistanceBar;
    ResistanceProperties resistance = new ResistanceProperties();

    public PropertiesBar healthBar;
    HealthProperties health = new HealthProperties();
    // Use this for initialization
    void Awake()
    {
        observer = Observer.Instance;
        animator = gameObject.GetComponent<Animator>();        
        manager = transform.GetComponent<SteeringManager>();
      //  gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
      
        //playerObject = GameManager.Player;
       // leave = gameObject.GetComponent<LeaveBehaviour>();
        wander = GetComponent<WanderBehaviour>();
        colAvoid = GetComponent<CollisionAvoidanceBehaviour>();
        wallAvoid = GetComponent<WallAvoidanceBehaviour>();  
        evade = GetComponent<EvadeBehaviour>();
        seek = GetComponent<SeekBehaviour>();
        wander.target = gameObject;
        seek.target = playerObject;
        evade.target = playerObject;
        //state = EnemyEvents.WANDER;
      //  leave.target = gameObject.transform.parent.gameObject;
       // resistance = maxResistance;
     
        if (resistanceBar)
        {
            resistanceBar.SetValue(resistance.Value);
            resistanceBar.SetMaxValue (resistance.MaxValue);
        }

        //health = new HealthProperties();
        if (healthBar)
        {
            healthBar.SetValue(health.Value);
            healthBar.SetMaxValue(health.MaxValue);
        }

    }
    void Start()
    {
        observer.SendMessage(EnemyEvents.SPAWN, gameObject);
        MaxVelocity = maxEnemyVelocity;
        MaxAcceleration = maxEnemyAcceleration;
        Size = sizeEnemy;
        SteeringBound = GameBound.WorldBound;       
        wander.weight = 1f;
        colAvoid.weight = 0f;
        wallAvoid.weight = 0f;
        evade.weight = 0f;
        seek.weight = 0f;
        observer.AddListener(TileEvens.DAMAGE, this, GetDamage);
        observer.AddListener(TileEvens.SLOW, this, GetDeceleration);
        observer.AddListener(PlayerEvents.DAMAGE, this, GetDamage);
        player = playerObject.GetComponent<SteeringAgent>();
    }
        
    void Update()
    {
        
        if (CheckforDie())
        {
            observer.SendMessage(PlayerEvents.DAMAGE, 5f);           
        }
        else
        {
            if (CheckforAttack())
            {
                state = EnemyEvents.ATTACK;
            }
            else
            {
                animator.SetBool("isAttack", false);
                if (LookDangerous())
                {
                    state = EnemyEvents.EVADE;
                }
                else
                {
                    state = EnemyEvents.WANDER;
                }
            }
        }
        if (!isDie) {
            SetBehaviorWeight(state);
        }

        MoveEnemy();


    }
    private bool LookDangerous()
    {
        float distance = Vector3.Distance(Position, player.Position);
        return (distance < 3f) ? true : false;
    }

    private void SetBehaviorWeight(EnemyEvents stateNow)
    {
        switch(stateNow)
        {
            case EnemyEvents.WANDER:
                wander.weight = 0.5f;
                colAvoid.weight = 1f;
                wallAvoid.weight = 0.8f;
                evade.weight = 0.0f;
                seek.weight = 0.0f;
                //   Wander();
                break;
            case EnemyEvents.EVADE:
                wander.weight = 0f;
                colAvoid.weight = 1f;
                wallAvoid.weight = 0.9f;
                evade.weight = 0.7f;
                seek.weight = 0.0f;
                //    Evade();
                break;
            case EnemyEvents.ATTACK:
                wander.weight = 0f;
                colAvoid.weight = 1f;
                wallAvoid.weight = 0.8f;
                evade.weight = 0f;
                seek.weight = 0.8f;
                Attack();
                break;
            case EnemyEvents.DIE:
                wander.weight = 0f;
                colAvoid.weight = 0f;
                wallAvoid.weight = 0f;
                evade.weight = 0f;
                seek.weight = 0.0f;
                Die();
                break;
        }
    }


    private bool CheckforDie()   {
        if (Vector3.Distance(Position, player.Position ) <= Size)  {
            return true;
        }
        else   {
            return false;
        }       
    }

    private bool CheckforAttack()  {
        if (Vector3.Distance(Position, player.Position) <= attackDistance)   {
            return true;
        }
        else   {
            return false;
        }
    }

    private void MoveEnemy()
    {
        manager.Move();
        if (resistanceBar)
        {           
                resistance.RemoveValue(Time.deltaTime);
                resistanceBar.SetValue(resistance.Value);           
        }
    }

    private void Attack()
    {        
        animator.SetBool("isAttack", true);
        CauseDamage(damage);
    }

    private void CauseDamage (float damage)
    {
        observer.SendMessage(EnemyEvents.DAMAGE, damage);
    }

    private void GetDamage(ObservParam obj)
    {
        float hurt = (float)obj.data;
        health.RemoveValue(hurt);
        healthBar.SetValue(health.Value);
        if (health.Value <= 0f)
        {
            state = EnemyEvents.DIE;
        }
    }

    private void GetDeceleration(ObservParam obj)
    {
        float slow = (float)obj.data;
        MaxVelocity = maxEnemyVelocity*slow;
    }

    private void Die()
    {
        isDie = true;
        Velocity = Vector3.zero;
        animator.SetBool("isDied", true);
        Destroy(resistanceBar.gameObject);
        Destroy(healthBar.gameObject);
        StartCoroutine(Destroy());
    }

    private void OnDestroy()
    {
        observer.SendMessage(EnemyEvents.DIE, gameObject);
        observer.RemoveAllListeners(this);
       // EnemyManager.RemoveEnemyToList(gameObject);
       // colAvoid.RemoveAgentToList(gameObject);

    }
    
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

   
}