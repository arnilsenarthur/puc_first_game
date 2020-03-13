using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveSimulator : MonoBehaviour
{
    // Start is called before the first frame update
    float start_y = 0;
    float start_x = 0;
    void Start()
    {
        start_y = transform.position.y;
        start_x = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = (transform.position.z - Shader.GetGlobalVector("_CurveOrigin").z) / Shader.GetGlobalVector("_Scale").z;

        float direction = direction = Mathf.Lerp(Shader.GetGlobalVector("_ReferenceDirection").z, dist, Mathf.Min(dist, 1));

        float theta = Mathf.Acos(Mathf.Clamp(Vector3.Dot(new Vector3(0,0,direction), new Vector3(0,0, Shader.GetGlobalVector("_ReferenceDirection").z)), -1, 1));

        float waveMultiplier = Mathf.Cos(theta * Shader.GetGlobalFloat("_HorizonWaveFrequency"));

        dist = Mathf.Max(0, dist - Shader.GetGlobalFloat("_FlatMargin"));

       float f = dist * dist * Shader.GetGlobalFloat("_Curvature") * waveMultiplier;

      
        transform.position = new Vector3(start_x + f * 2f, start_y + f*2f, transform.position.z);

    }
}
