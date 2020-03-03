using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *  Source: https://gist.github.com/Nordaj/33301ae473968c4fd0d096ffc427eb66
 */
public class DeformableMesh : MonoBehaviour
{
    //Public
    public float minImpulse = 2;
    public float malleability = 0.001f;
    public float radius = 0.075f;

    //Private
    private Mesh m;
    private MeshCollider mc;
    private Vector3[] verts;
    private Vector3[] iVerts;

    private void Start()
    {
        m = GetComponent<MeshFilter>().mesh;
        mc = GetComponent<MeshCollider>();
        iVerts = m.vertices;
    }

    public void Repair()
    {
        m.vertices = iVerts;
        mc.sharedMesh = m;

        //Recalculate mesh stuff
        ///Currently gets unity to recalc normals. Could be optimized and improved by doing it ourselves.
        m.RecalculateNormals();
        m.RecalculateBounds();
    }

    public void OnCollisionEnter(Collision collision)
    {

        foreach (ContactPoint c in collision)
        {
            //Get point, impulse mag, and normal
            Vector3 pt = transform.InverseTransformPoint(c.point);
            Vector3 nrm = transform.InverseTransformDirection(c.normal);
            float imp = collision.impulse.magnitude;

            imp = 100f;
            //Deform vertices
            verts = m.vertices;
            float scale; ///Declare outside of tight loop
            for (int i = 0; i < verts.Length; i++)
            {
                //Get deformation scale based on distance
                scale = Mathf.Clamp(radius - (pt - verts[i]).magnitude, 0, radius);


                //Deform by impulse multiplied by scale and strength parameter
                verts[i] += nrm * imp * scale * malleability;
            }
        }

        //Apply changes to collider and mesh
        m.vertices = verts;
        mc.sharedMesh = m;

        //Recalculate mesh stuff
        ///Currently gets unity to recalc normals. Could be optimized and improved by doing it ourselves.
        m.RecalculateNormals();
        m.RecalculateBounds();
    }

    private void OnApplicationQuit()
    {
        //Need to reset mesh after quit
        m.vertices = iVerts;
    }
}