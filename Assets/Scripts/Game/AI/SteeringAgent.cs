using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringAgent : BaseBehaviour {

    Vector3 velocity;
    float maxVelocity;
    Vector3 position;
    float maxAcceleration;
    Vector3 angle;
    Boundary steeringBound;
    float size;

    public Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    public float MaxVelocity
    {
        get { return maxVelocity; }
        set { maxVelocity = value; }
    }

    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
       
    public float MaxAcceleration
    {
        get { return maxAcceleration; }
        set { maxAcceleration = value; }
    }

    public Vector3 Angle
    {
        get { return transform.eulerAngles; }
        set { transform.eulerAngles = value; }
    }

    public Boundary SteeringBound
    {
        get { return steeringBound; }
        set { steeringBound = value; }
    }
    public float Size
    {
        get { return size; }
        set { size = value; }
    }

    public virtual void SetBoundForMove()
    {
        float wight = Camera.main.orthographicSize * Camera.main.aspect * 2 - size;
        float hight = Camera.main.orthographicSize * 2 - size;
        SteeringBound = new Boundary(wight, hight);
    }

  
}
