using UnityEngine;

public class ArriveBehaviour : SteeringBase {
   
    private float slowRadius = 1f;
    private float targetRadius =  0.5f;
    private float timeToTarget = 0.1f;

    // Возвращает поведение когда обьект должен достигнуть цели
    public override Vector3 GetAcceleration()
    {
       /* Vector3 acceleration = Vector3.zero;
        Vector3 direction = target.transform.position - manager.agent.Position;

        float distance = direction.magnitude;
        float targetSpeed;
        if (distance < targetRadius) {
            return acceleration;
        } 

        if (distance > slowRadius) {
            targetSpeed = manager.agent.MaxVelocity;
        }
        else {
            targetSpeed = manager.agent.MaxVelocity * ((distance - targetRadius) / (slowRadius - targetRadius));
        }

        Vector3 desiredVelocity = direction;
        desiredVelocity.Normalize();
        desiredVelocity *= targetSpeed;
        acceleration = desiredVelocity - manager.agent.Velocity;
        acceleration /= timeToTarget;
       
        return acceleration;*/


         
          float distance = Vector3.Distance( manager.agent.Position, target.transform.position);
          Vector3 newVelocity = (target.transform.position - manager.agent.Position).normalized;

          if (distance < targetRadius)
              newVelocity = Vector3.zero;
          else if (distance < slowRadius)
              newVelocity = newVelocity * manager.agent.MaxVelocity * ((distance - targetRadius) / (slowRadius - targetRadius));
          else
              newVelocity = newVelocity * manager.agent.MaxVelocity;

          return newVelocity - manager.agent.Velocity;
    }
}
