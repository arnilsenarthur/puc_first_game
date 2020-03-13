using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCicle : MonoBehaviour
{
    Light l;

    //floats from 0 to 24
    public static float day_time_circle = 6f;


    void Awake()
    {
        l = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
       float coef = day_time_circle / 24f;
       float light_rotation_coef = (day_time_circle - 6f) / 12f;

       //Rotate shadow from left to right (6h to 18h)
       gameObject.transform.localEulerAngles = new Vector3(50,-90 + 180 * light_rotation_coef, 0);

        float min_strength = 0.2f;
       //Set shadow strenght
       l.shadowStrength = (day_time_circle > 5 && day_time_circle < 19) ? ((1 - Mathf.Abs(12 - day_time_circle) / 7f)  + min_strength) : min_strength;
        //Set shadow color
        if(day_time_circle > 6 && day_time_circle <= 12)
        {
            l.color = Color.Lerp(Color.white, new Color(0.04855575f, 0.02936988f, 0.4150943f),(Mathf.Abs(12 - day_time_circle) / 6f));
        }
        else if (day_time_circle > 12 && day_time_circle <= 20)
        {
            l.color = Color.Lerp(Color.white, new Color(0.04855575f, 0.02936988f, 0.4150943f), (Mathf.Abs(12 - day_time_circle) / 8f));
        }
        else
        {
            l.color = new Color(0.04855575f, 0.02936988f, 0.4150943f);
        }
        
      day_time_circle += Time.fixedDeltaTime/8f;
      day_time_circle %= 24;
    }
}
