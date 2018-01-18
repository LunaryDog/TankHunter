using UnityEngine;

public abstract class SteeringBase : MonoBehaviour {

    public float weight = 1f;
    public GameObject target;
    protected SteeringManager manager;

    public virtual Vector3 GetAcceleration()
    {
        return new Vector3(0, 0, 0);
    }

    public virtual void Awake()
    {
        manager = GetComponent<SteeringManager>();
        manager.RegisterBehaviour(this);
    }

     public virtual void OnDestroy()
    {
        if (manager != null)
            manager.DeregisterBehaviour(this);
    }
}
