using UnityEngine;

public class LeaveBehaviour : SteeringBase {

    public float escapeRadius;
    public float dangerRadius;
    public float timeToTarget = 0.1f;

    public override Vector3 GetAcceleration()
    {
        Vector3 acceleration = Vector3.zero;       
        Vector3 direction = manager.agent.Position - target.transform.position;

        float distance = direction.magnitude;
        if (distance > dangerRadius) {
            return acceleration;
        }

        float reduce;
        if (distance<escapeRadius) {
            reduce = 0f;
        }
        else {
            reduce = distance / dangerRadius* manager.agent.MaxVelocity;
        }

        float targetSpeed = manager.agent.MaxVelocity - reduce;
        Vector3 desiredVelocity = direction;
        desiredVelocity.Normalize();
        desiredVelocity *= targetSpeed;
        acceleration = desiredVelocity - manager.agent.Velocity;
        acceleration /= timeToTarget;       
        return acceleration;
    }
}
