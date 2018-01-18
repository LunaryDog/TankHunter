using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionAvoidanceBehaviour : SteeringBase {

    public float collisionRadius = 0.4f;
   // GameObject[] targets;
    List<GameObject> targetsList;

    public override void Awake()
    {
        base.Awake();
      //  targets = GameObject.FindGameObjectsWithTag("Agent");
        targetsList = new List<GameObject>();
       // foreach (GameObject t in targets)
       // {
          //  targetsList.Add(t);
       // }
    }

    public override Vector3 GetAcceleration()
    {
        Refresh();
        Vector3 acceleration = Vector3.zero; 
        float shortestTime = float.PositiveInfinity;
              
        GameObject firstTarget = null;
       
        float firstMinSeparation = 0f;
        float firstDistance = 0f;
        Vector3 firstRelativePos = Vector3.zero;
        Vector3 firstRelativeVel = Vector3.zero;

        foreach (GameObject t in targetsList)
        {

            Vector3 relativePos;
            SteeringAgent targetAgent = t.GetComponent<SteeringAgent>();
            relativePos = targetAgent.Position - manager.agent.Position;
            Vector3 relativeVel = targetAgent.Velocity - manager.agent.Velocity;
            float relativeSpeed = relativeVel.magnitude;
            float timeToCollision = Vector3.Dot(relativePos, relativeVel);
            timeToCollision /= relativeSpeed * relativeSpeed * -1;
            float distance = relativePos.magnitude;          

            if (relativeSpeed == 0)
            {
                continue;
            }
               
            Vector3 separation = relativePos + relativeVel * timeToCollision;
            float minSeparation = separation.magnitude;         

            if (minSeparation > collisionRadius * 2 )
            //if (minSeparation > 2 * agentRadius)
            {
                continue;
            }

            // Check if its the shortest 
            if (timeToCollision > 0 && timeToCollision < shortestTime)
            {
                shortestTime = timeToCollision;
                firstTarget = t;
                firstMinSeparation = minSeparation;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
            }
        }

        // Calculate the steering 

        if (firstTarget == null)  {
            return acceleration;
        }

        if (firstMinSeparation <= 0.0f || firstDistance < 2 * collisionRadius) {
            firstRelativePos = firstTarget.transform.position;
        }
        else {
            firstRelativePos += firstRelativeVel * shortestTime;
        }

        acceleration = -firstRelativePos;
        

        return acceleration;
    }
   
    public void AddAgentToList(GameObject agent)
    {
        targetsList.Add(agent);        
    }

    public void RemoveAgentToList(GameObject agent)
    {
        targetsList.Remove(agent);     
    }

    public void Refresh()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Agent");
        targetsList.Clear(); 
         foreach (GameObject t in targets)
         {
          targetsList.Add(t);
         }       
    }
}
