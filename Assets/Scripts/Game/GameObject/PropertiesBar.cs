using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesBar : MonoBehaviour {

    private Transform owner;
    private SpriteRenderer render;

    private float offset;
    private float value;
    private float maxValue;
    private float transparency = 0.5f;

    private Color colorFull = Color.green;
    private Color colorEmpty = Color.red;

    void Start()
    {
        // health bar unparents itself from the ship, and follows its position
        // both have a reference to each other

        owner = transform.parent;
        offset = transform.localPosition.y;
        render = GetComponent<SpriteRenderer>();

       // colorFull = Color.green;
       // colorEmpty = Color.red;

        colorFull.a = transparency;
        colorEmpty.a = transparency;
    }

    void LateUpdate()
    {
        if (!owner)
            Destroy(gameObject);

        transform.position = owner.position + (Vector3.up * offset);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(value / 20f, transform.localScale.y, transform.localScale.z), Time.deltaTime * 10f);
        render.color = Color.Lerp(colorEmpty, colorFull, value / maxValue);
    }

    public void SetValue(float value)
    {
        this.value = value;
      //  SetBarView();
    }

    public void SetMaxValue(float value)
    {
        this.maxValue = value;
      //  SetBarView();
    }

    private void SetBarView()
    {
      //  transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(value / 20f, transform.localScale.y, transform.localScale.z), Time.deltaTime * 10f);
       // render.color = Color.Lerp(colorEmpty, colorFull, value / maxValue);
    }
}
