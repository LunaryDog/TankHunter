using UnityEngine;


public class EvadeBehaviour : FleeBehaviour
{
    
    public float maxPrediction = 1f;
    private GameObject targetAux;
    private SteeringAgent targetAgent;

    public override void Awake()
    {
        base.Awake();
        targetAgent = target.GetComponent<SteeringAgent>();
       // targetAux = target;
      //  target = new GameObject();
    }
   
    public  Vector3 getAcceleration()
    {
        
        Vector3 displacement = targetAux.transform.position - manager.agent.Position;
        float distance = displacement.magnitude;     
  
        float speed = manager.agent.Velocity.magnitude;

        float prediction;
        if (speed <= distance / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
            //Place the predicted position a little before the target reaches the character
            prediction *= 0.9f;
        }
        target.transform.position = targetAux.transform.position;
        target.transform.position += targetAgent.Velocity * prediction;

        return base.GetAcceleration();
    }

    public override Vector3 GetAcceleration()
    {
        float distance = Vector3.Distance(manager.agent.Position, targetAgent.Position);

        if (distance < 3f)
        {
            float t = distance / targetAgent.MaxVelocity;
            Vector3 targetPoint = targetAgent.Position + targetAgent.Velocity * t;

            return -(((targetAgent.Position - manager.agent.Position).normalized * manager.agent.MaxVelocity) - manager.agent.Velocity);
        }
        else
            return Vector2.zero;
    }

}
