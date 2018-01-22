using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class Properties {

    public float MaxValue { get; set; }
    public float MinValue { get; set; }    
    public float StepValue { get; set; }
    float valueProteries;
    public float Value
    {
        get { return valueProteries; }
        set
        {
            valueProteries = value;
            if (valueProteries <= MinValue)
            {
                valueProteries = MinValue;
            }
            if (valueProteries >= MaxValue)
            {
                valueProteries = MaxValue;
            }
        }
    }

    public  Properties()
    {
        MinValue = 0f;
        MaxValue = 100;
        Value = MaxValue;
        StepValue = 1f;
    }

    public  Properties(float max, float min = 0f, float step = 1f)
    {
        MinValue = min;
        MaxValue = max;
        Value = MaxValue;
        StepValue = step;
    }

    public  Properties(float max, float current, float min = 0f, float step = 1f)
    {
        MinValue = min;
        MaxValue = max;
        Value = current;
        StepValue = step;
    }

    public virtual void AddValue(float addValue)
    {
        Value += addValue * StepValue;       
    }

    public virtual void RemoveValue(float removeValue)
    {
        Value -= removeValue * StepValue;       
    }
}
