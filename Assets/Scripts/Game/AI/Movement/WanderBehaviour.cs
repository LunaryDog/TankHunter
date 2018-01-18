using UnityEngine;

public class WanderBehaviour : SteeringBase {

    protected GameObject targetAux;
    public float rate = .2f;

    public float offset = 1.0f;
    public float radius = 1.0f;
    public float jitter = .2f;

    private Vector3 targetDir;
    private Vector3 randomDir;

    public float wanderOrientation;
    

    public override Vector3 GetAcceleration()
    {
        // SET force to zero
        Vector3 acceleration = Vector3.zero;
        Vector3 targetPosition = target.transform.forward;
       // Generate random numbers between a certain range

       // 0x7fff = 32767
       float randX = Random.Range(0, 0x7fff) - (0x7fff * .5f);
       float randY = Random.Range(0, 0x7fff) - (0x7fff * .5f);
        // float randX = Random.Range(manager.agent.SteeringBound.LeftBound, manager.agent.SteeringBound.RightBound);
        //  float randY = Random.Range(manager.agent.SteeringBound.DownBound, manager.agent.SteeringBound.UpBound);

        randomDir = new Vector3(randX, randY,0);
        randomDir = randomDir.normalized * jitter;

        targetDir += randomDir;      
        targetDir = targetDir.normalized * radius;

        Vector3 seekPos = manager.agent.Position + targetDir;
 
        seekPos += targetPosition * offset;
        targetPosition = new Vector3(Mathf.Clamp(seekPos.x, manager.agent.SteeringBound.LeftBound, manager.agent.SteeringBound.RightBound), Mathf.Clamp(seekPos.y, manager.agent.SteeringBound.DownBound, manager.agent.SteeringBound.UpBound), seekPos.z);
        //Debug.Log(targetPosition);
        //Debug.Log(manager.agent.Angle);
        // SET desiredForce to seekPos - position
       // acceleration = seekPos - manager.agent.Position;
       
        acceleration = targetPosition - manager.agent.Position;

        //чтобы не залипал по углам
        if ((acceleration == Vector3.zero))
        {
            acceleration = Vector3.zero - manager.agent.Position;
        }

   
        return acceleration;
    }

    public  Vector3 getAcceleration()
    {
       
        Vector3 acceleration = Vector3.zero;
        float characterOrientation = Mathf.Atan2(manager.agent.Velocity.y, manager.agent.Velocity.x);

        
        wanderOrientation += randomBinomial() * rate;

        float targetOrientation = wanderOrientation + characterOrientation;

        Vector3 orientationVec = GetOrientation(characterOrientation);

        Vector3 seekPos = (offset * orientationVec) + manager.agent.Position;
        seekPos = seekPos + (GetOrientation(targetOrientation)* radius);
        Vector3 targetPosition = new Vector3(Mathf.Clamp(seekPos.x, manager.agent.SteeringBound.LeftBound, manager.agent.SteeringBound.RightBound), Mathf.Clamp(seekPos.y, manager.agent.SteeringBound.DownBound, manager.agent.SteeringBound.UpBound), seekPos.z);
        // targetAux.transform.position = targetPosition;       

        /* Vector3 direction = targetAux.transform.position - manager.agent.Position;
         if (direction.magnitude > 0.0f)
         {
             targetOrientation = Mathf.Atan2(direction.x,    direction.z);
             targetOrientation *= Mathf.Rad2Deg;           
         }

         float rotation = targetOrientation - manager.agent.Angle.z;
         rotation = MapToRange(rotation);
         float rotationSize = Mathf.Abs(rotation);
         if (rotationSize < targetRadius)
             return acceleration;
         float targetRotation;
         if (rotationSize > slowRadius)
             targetRotation = maxRotation;
         else
             targetRotation = maxRotation * rotationSize /slowRadius;
         targetRotation *= rotation / rotationSize;
         steering.angular = targetRotation - agent.rotation;
         steering.angular /= timeToTarget;
         float angularAccel = Mathf.Abs(steering.angular);
         if (angularAccel > agent.maxAngularAccel)
         {
             steering.angular /= angularAccel;
             steering.angular *= agent.maxAngularAccel;
         }
         return steering;*/


        acceleration = targetPosition - manager.agent.Position;
        if ((acceleration == Vector3.zero))
        {
            acceleration = Vector3.zero - manager.agent.Position;
        }
        return acceleration;    
    }

    float randomBinomial()
    {
        //return Random.value - Random.value;
        float randX = Random.Range(0, 0x7fff) - (0x7fff * .5f);
        float randY = Random.Range(0, 0x7fff) - (0x7fff * .5f);
        Vector3 RandomDirection = new Vector3(randX, randY, 0);
        return Mathf.Atan2(RandomDirection.y, RandomDirection.x);
    }

    public Vector3 GetOrientation(float orientation)
    {
        Vector3 vector = Vector3.zero;
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1.0f;
        vector.y = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1.0f;
        return vector.normalized;
    }

    public float MapToRange(float rotation)
    {
        rotation %= 360.0f;
        if (Mathf.Abs(rotation) > 180.0f)
        {
            if (rotation < 0.0f)
                rotation += 360.0f;
            else
                rotation -= 360.0f;
        }
        return rotation;
    }
}
