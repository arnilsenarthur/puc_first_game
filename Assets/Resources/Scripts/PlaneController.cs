using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    float acceleration = 18f;
    float maxSpeed = 25f;
    float minSpeed = 10f;
    public float speedometer = 0;
    public bool working = true;

    public bool controlable = false;

    Rigidbody rb;

    public float max_y_move = 10f;
    public float max_barrel = 25f;

 
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

   
    void Update()
    {
        //Update car speedometer
        speedometer = transform.InverseTransformDirection(rb.velocity).z;
        //speedometer = rb.velocity.magnitude;

        //Car forward direction
        Vector3 fw = Vector3.forward;

        if (controlable)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, -max_barrel), 30f * Time.fixedDeltaTime);
                //rb.AddForce(Vector3.down * (transform.localRotation.x * transform.localRotation.x)/900f * -30f, ForceMode.Acceleration);
                rb.AddForce(new Vector3(5, 0, 0), ForceMode.Acceleration);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.z, max_barrel), 30f * Time.fixedDeltaTime);
                //  rb.AddForce(Vector3.down * (transform.localRotation.x * transform.localRotation.x) / 900f * 30f, ForceMode.Acceleration);
                rb.AddForce(new Vector3(-5, 0, 0), ForceMode.Acceleration);
            }
            else
            {
                rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(transform.localEulerAngles.x, rb.transform.localEulerAngles.y, 0), 50f * Time.fixedDeltaTime);
                rb.AddForce(new Vector3(-rb.velocity.x * 3f, 0, 0), ForceMode.Acceleration);
            }

            if (Input.GetKey(KeyCode.UpArrow) && (rb.transform.position.y > 4.5f))
            {
                rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(max_y_move, transform.localEulerAngles.y, transform.localEulerAngles.z), 30f * Time.fixedDeltaTime);
                rb.AddForce(new Vector3(0, -5, 0), ForceMode.Acceleration);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(-max_y_move, transform.localEulerAngles.y, transform.localEulerAngles.z), 30f * Time.fixedDeltaTime);
                rb.AddForce(new Vector3(0, 5, 0), ForceMode.Acceleration);
            }
            else
            {
                rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(0, rb.transform.localEulerAngles.y, transform.localEulerAngles.z), 50f * Time.fixedDeltaTime);
                rb.AddForce(new Vector3(0, -rb.velocity.y * 3f, 0), ForceMode.Acceleration);
            }

            rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(rb.transform.localEulerAngles.x, 0f, transform.localEulerAngles.z), 50f * Time.fixedDeltaTime);

            //  transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            if(rb.transform.position.y < 4.5f)
            {
                rb.AddForce(new Vector3(0, 8f, 0), ForceMode.Acceleration);
            }

            Camera.main.transform.position = new Vector3(0, Mathf.Lerp(Camera.main.transform.position.y, transform.position.y + 1.536f, 2f * Time.fixedDeltaTime), gameObject.transform.position.z - 7);

        }
        else
        {
            if (rb.transform.position.y < 3f)
            {
                rb.AddForce(new Vector3(0, 8f, 0), ForceMode.Acceleration);
            }
            else if (rb.transform.position.y < 5f)
            {
                rb.AddForce(new Vector3(0, 4f, 0), ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(new Vector3(0, -rb.velocity.y * 3f, 0), ForceMode.Acceleration);
            }
            rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(-max_y_move, 0, 0), 50f * Time.fixedDeltaTime);
            Camera.main.transform.position = new Vector3(0, Mathf.Lerp(Camera.main.transform.position.y, transform.position.y + 1.536f, 4f * Time.fixedDeltaTime), gameObject.transform.position.z - 7);

        }


        //Max speed limit
        if (speedometer > maxSpeed)
        {
            rb.AddForce(fw * -acceleration, ForceMode.Acceleration);
        }

        //Min speed limit
        if (speedometer < maxSpeed)
        {
            rb.AddForce(fw * acceleration, ForceMode.Acceleration);
        }

        
       
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 30), "Speed: " + speedometer);
    }
}
