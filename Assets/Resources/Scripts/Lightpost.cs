using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightpost : MonoBehaviour
{
    void Update()
    {
        GetComponent<Light>().enabled = !(DayNightCicle.day_time_circle >= 8 && DayNightCicle.day_time_circle <= 18);
    }
}
