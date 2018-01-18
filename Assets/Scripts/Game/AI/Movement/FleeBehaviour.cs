using UnityEngine;
using System.Collections;


public class FleeBehaviour : SteeringBase {

    private float panicDist = 3.5f;
    private bool decelerateOnStop = true;
    private float timeToTarget = 0.1f;

    public override Vector3 GetAcceleration()
    {       
        Vector3 acceleration = manager.agent.Position - target.transform.position;
        if (acceleration.magnitude > panicDist)
        {
            //Slow down if we should decelerate on stop
            if (decelerateOnStop && manager.agent.Velocity.magnitude > 0.001f)
            {
                //Decelerate to zero velocity in time to target amount of time
                acceleration = -manager.agent.Velocity / timeToTarget;
                return acceleration;
            }
            else
            {
               // rb.velocity = Vector2.zero;
                return Vector3.zero;
            }
        }
        return acceleration;
    }

    private Vector3 giveMaxAccel(Vector3 v, float maxAcceleration)
    {
        //Remove the z coordinate
        v.z = 0;

        v.Normalize();

        //Accelerate to the target
        v *= maxAcceleration;

        return v;
    }
}
