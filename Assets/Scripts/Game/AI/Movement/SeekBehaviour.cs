using UnityEngine;

public class SeekBehaviour : SteeringBase {

    public override Vector3 GetAcceleration()
    {
        //Get the direction
        Vector3 acceleration = target.transform.position - manager.agent.Position;

        return acceleration;
    }
}
