using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    /*
     * Main variables
     */
    Rigidbody rb;
    float acceleration = 18f;
    float maxSpeed = 25f;
    float minSpeed = 10f; //15f;
    public float speedometer = 0;
    float max_angle = 30;
    float brake = 0.5f;

    public bool working = true;

    /*
     * Car parts
     */
    public GameObject car_collider;
    public GameObject[] wheels;

    /*
     * Start method
     */
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /*
     * Fixed Update
     */
    private void FixedUpdate()
    {
        //Update car speedometer
        speedometer = transform.InverseTransformDirection(rb.velocity).z;
        //speedometer = rb.velocity.magnitude;

        //Car forward direction
        Vector3 fw = /*transform.forward;*/ new Vector3(1 * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad),0, 1 * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad));

        //Accelerate/Brake
        if (working)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                rb.AddForce(fw * acceleration, ForceMode.Acceleration);
            else if (Input.GetKey(KeyCode.DownArrow))
                rb.AddForce(fw * -speedometer * brake, ForceMode.Acceleration);


            if (Input.GetKey(KeyCode.LeftArrow))
            {
                //Brake car for safe rotation
                rb.AddForce(new Vector3(0, 0, -speedometer * 1.5f), ForceMode.Acceleration);

                //Rotate car
                rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, Quaternion.Euler(0, -max_angle, 0), 50f * Time.fixedDeltaTime);

                //Rotate tires
                wheels[0].transform.localRotation = Quaternion.RotateTowards(wheels[0].transform.localRotation, Quaternion.Euler(0, -max_angle * 2, 0), 80f * Time.fixedDeltaTime);
                wheels[1].transform.localRotation = Quaternion.RotateTowards(wheels[1].transform.localRotation, Quaternion.Euler(0, -max_angle * 2, 0), 80f * Time.fixedDeltaTime);

            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                //Brake car for safe rotation
                rb.AddForce(new Vector3(0, 0, -speedometer * 1.5f), ForceMode.Acceleration);

                //Rotate car
                rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, Quaternion.Euler(0, max_angle, 0), 50f * Time.deltaTime);

                //Rotate tires
                wheels[0].transform.localRotation = Quaternion.RotateTowards(wheels[0].transform.localRotation, Quaternion.Euler(0, max_angle * 2, 0), 80f * Time.fixedDeltaTime);
                wheels[1].transform.localRotation = Quaternion.RotateTowards(wheels[1].transform.localRotation, Quaternion.Euler(0, max_angle * 2, 0), 80f * Time.fixedDeltaTime);

            }
            else
            {
                //Return car to intial rotation
                rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, Quaternion.Euler(0, 0, 0), 20f * Time.fixedDeltaTime);

                //Rotate tires
                wheels[0].transform.localRotation = Quaternion.RotateTowards(wheels[0].transform.localRotation, Quaternion.Euler(0, 0, 0), 80f * Time.fixedDeltaTime);
                wheels[1].transform.localRotation = Quaternion.RotateTowards(wheels[1].transform.localRotation, Quaternion.Euler(0, 0, 0), 80f * Time.fixedDeltaTime);
            }

            //Add car stability
            rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0), 75f * Time.fixedDeltaTime);

            //Car turned more than max
            if(!(transform.eulerAngles.y < 70 || transform.eulerAngles.y > 360-70))
            {
                working = false;                
            }

            //Max speed limit
            if (speedometer > maxSpeed)
            {
                rb.AddForce(fw * -acceleration, ForceMode.Acceleration);
            }

            //Min speed limit
            if (speedometer < minSpeed)
            {
                rb.AddForce(fw * acceleration, ForceMode.Acceleration);
            }
        }
      

        //Tires rotation animation
        foreach (GameObject o in wheels)
        {
            o.transform.Rotate(new Vector3(speedometer, 0, 0));
        }

        //Make camera follow player
        Camera.main.transform.position = new Vector3(0, Mathf.Lerp(Camera.main.transform.position.y, 3.536f, 4f * Time.fixedDeltaTime), gameObject.transform.position.z - 7);     
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 30), "Speed: " + speedometer);
        GUI.Label(new Rect(10, 30, 300, 30), "F3 to restart");
        GUI.Label(new Rect(10, 50, 300, 30), "P to change mode");
        GUI.Label(new Rect(10, 70, 300, 30), "R to repair");
    }


    List<Vector3> def = new List<Vector3>();
    
    public void Repair()
    {
        car_collider.GetComponent<DeformableMesh>().Repair();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Hitted bottom
        if (collision.collider is BoxCollider)
            if(((BoxCollider) collision.collider).size.y == 0)
            return;

        if (collision.relativeVelocity.magnitude > 3)
        {
            car_collider.GetComponent<DeformableMesh>().OnCollisionEnter(collision);
        }

        if (collision.other.tag != "Collidable")
            return;

        

        if(working)
        {
            Debug.Log(collision.other);
            //Debug.Break();
            working = false;
        }   
    }


}
