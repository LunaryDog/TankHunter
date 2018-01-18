using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// обхождение обьектов с коллайдерами
public class WallAvoidanceBehaviour : SteeringBase {

    // длина вектора для обнаружения
    private float mainWhiskerLen = 2f;

    // дистанция обхода стены
    private float wallAvoidDistance = 1f;

    private float sideWhiskerLen = 1f;

    //
    private float sideWhiskerAngle = 45f;   

    public override Vector3 GetAcceleration()
    {
        return getSteering(manager.agent.Velocity);
    }

    public Vector3 getSteering(Vector3 facingDir)
    {
        Vector3 acceleration = Vector3.zero;

        Vector3[] rayDirs = new Vector3[3];
        rayDirs[0] = facingDir.normalized;

        float orientation = Mathf.Atan2(facingDir.y, facingDir.x);
        
        rayDirs[1] = orientationToVector(orientation + sideWhiskerAngle * Mathf.Deg2Rad);
        rayDirs[2] = orientationToVector(orientation - sideWhiskerAngle * Mathf.Deg2Rad);

        RaycastHit2D hit;

        // проверка на обнаржение коллайдеров поблизости
        if (!findObstacle(rayDirs, out hit))
        {
            return acceleration;
        }

        // Создаем цель для движения
        Vector3 targetPostition = hit.point + hit.normal * wallAvoidDistance;

        Vector3 cross = Vector3.Cross(facingDir, hit.normal);
        if (cross.magnitude < 0.005f)
        {
            targetPostition = targetPostition + new Vector3(-hit.normal.y, hit.normal.x, 0);
        }
       
        return targetPostition - manager.agent.Position; 
    }

    // определяем ориентацию вектора
    private Vector3 orientationToVector(float orientation)
    {
        return new Vector3(Mathf.Cos(orientation), Mathf.Sin(orientation), 0);
    }

    private bool findObstacle(Vector3[] rayDirs, out RaycastHit2D firstHit)
    {
        firstHit = new RaycastHit2D();
        bool foundObs = false;

        for (int i = 0; i < rayDirs.Length; i++)
        {
            float rayDist = (i == 0) ? mainWhiskerLen : sideWhiskerLen;

            RaycastHit2D hit = Physics2D.Raycast((Vector3)manager.agent.Position, (Vector3)rayDirs[i], rayDist);

            if ((hit.collider != null) && (!hit.collider.isTrigger))
            {
                foundObs = true;
                firstHit = hit;
                break;
            }

            
        }

        return foundObs;
    }
}
