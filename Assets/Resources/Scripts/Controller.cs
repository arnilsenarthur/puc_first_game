using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller PLAYER;

    GameObject[] wheels;
    GameObject collider;
   public GameObject plane;
    public bool working = true;

    private void Start()
    {
        PLAYER = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (GetComponent<PlaneController>() == null)
            {
                ToPlaneMode();
            }
            else
            {
                ToCarMode();
            }
        }
    }



    public void ToPlaneMode()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        wheels = gameObject.GetComponent<CarController>().wheels;
        collider = gameObject.GetComponent<CarController>().car_collider;
        
        Destroy(gameObject.GetComponent<CarController>());
        rb.mass = 1f;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        rb.velocity = new Vector3(0,0, rb.velocity.z);
       // rb.AddForce(new Vector3(0, 500f, 0), ForceMode.Force);


        gameObject.AddComponent<PlaneController>();
        StartCoroutine("EnablePlane", 2f);
        plane.GetComponent<MeshRenderer>().enabled = true;
        
        foreach (GameObject o in wheels)
        {
            o.transform.parent.GetComponent<WheelCollider>().enabled = false;
        }      
    }

    IEnumerator EnablePlane(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.GetComponent<PlaneController>().controlable = true;
        yield return null;

    }

    public void ToCarMode()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
       
        Destroy(gameObject.GetComponent<PlaneController>());
        float f = rb.velocity.z;
        rb.mass = 1000f;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
        plane.GetComponent<MeshRenderer>().enabled = false;

        gameObject.AddComponent<CarController>();

        gameObject.GetComponent<CarController>().wheels = wheels;
        gameObject.GetComponent<CarController>().car_collider = collider;

        foreach (GameObject o in wheels)
        {
            o.transform.parent.GetComponent<WheelCollider>().enabled = true;
        }

        rb.velocity = new Vector3(0, 0, f);

    }
}
