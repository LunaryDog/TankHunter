using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager : MonoBehaviour {

  
    public SteeringAgent agent; 
   
    List<SteeringBase> behaviours = new List<SteeringBase>();
    public static List<SteeringManager> ManagerList = new List<SteeringManager>();
    float turnSpeed = 10.0f;
    void Start()
    {
        ManagerList.Add(this);
        agent = transform.GetComponent<SteeringAgent>();        
        
    }

    public void RegisterBehaviour(SteeringBase behaviour)
    {
        behaviours.Add(behaviour);
    }

    public void DeregisterBehaviour(SteeringBase behaviour)
    {
        behaviours.Remove(behaviour);
    }

    public void Move()
    {
        Vector3 acceleration = Vector3.zero;

        foreach (SteeringBase behaviour in behaviours)
        {
            if (behaviour.enabled) {
                //  behaviour.target = target;
                acceleration += behaviour.GetAcceleration() * behaviour.weight;
            }
               
        }
     //   Debug.Log( behaviours.Count);
        agent.Velocity += acceleration ;
        //agent.Velocity -= agent.Velocity * Friction;

        if (agent.Velocity.magnitude > agent.MaxVelocity)
            agent.Velocity = agent.Velocity.normalized * agent.MaxVelocity;

        Vector3 newPosition = agent.Position + agent.Velocity * Time.deltaTime;
      
        agent.Position = new Vector3(Mathf.Clamp(newPosition.x, agent.SteeringBound.LeftBound, agent.SteeringBound.RightBound), Mathf.Clamp(newPosition.y, agent.SteeringBound.DownBound, agent.SteeringBound.UpBound), 0);
        Rotate();   
    }

    /*Поворачивает игровой обьект в сторону куда он движется*/
    void Rotate()
    {
        Vector3 direction = agent.Velocity;
        int numSamplesForSmoothing = 5;
        Queue<Vector3> velocitySamples = new Queue<Vector3>();

        if (velocitySamples.Count == numSamplesForSmoothing)
        {
            velocitySamples.Dequeue();
        }

        velocitySamples.Enqueue(agent.Velocity);

        direction = Vector3.zero;

        foreach (Vector3 v in velocitySamples)
        {
            direction += v;
        }

        direction /= velocitySamples.Count;

        direction.Normalize();

        // поворачивает если есть движение
        if (direction.sqrMagnitude > 0.001f)
        {
            float toRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            float rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, toRotation, Time.deltaTime * turnSpeed);
            agent.Angle = new Vector3(0, 0, rotation);
          //  transform.rotation = Quaternion.Euler(0, 0, rotation);

        }
    }

    // The public API (one method for each behavior)
    //   public seek(target :Vector3D, slowingRadius :Number = 20) :void {}
    //   public flee(target :Vector3D) :void {}
    //   public wander() :void {}
    //   public evade(target :IBoid) :void {}
    //    public pursuit(target :IBoid) :void {}

    // The update method. 
    // Should be called after all behaviors have been invoked
    //   public function update() :void {}

    // Reset the internal steering force.
    //   public function reset() :void {}

    // The internal API
    //   private function doSeek(target :Vector3D, slowingRadius :Number = 0) :Vector3D {}
    //   private function doFlee(target :Vector3D) :Vector3D {}
    //   private function doWander() :Vector3D {}
    //   private function doEvade(target :IBoid) :Vector3D {}
    //   private function doPursuit(target :IBoid) :Vector3D {}

    void OnDestroy()
    {
        ManagerList.Remove(this);
    }
}
