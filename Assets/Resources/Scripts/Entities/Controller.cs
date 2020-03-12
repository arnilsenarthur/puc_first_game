using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Controller PLAYER;

    public Texture2D aim;
    public GameObject gun;
    public GameObject bullet;
    public GameObject gun_exit;

    GameObject[] wheels;
    WheelCollider[] wheel_colliders;
    GameObject collider;

    public GameObject plane;

    public bool working = true;

    public bool car_mode = true;

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

        if(Input.GetKeyDown(KeyCode.R))
        {
            if(car_mode)
            {
                GetComponent<CarController>().Repair();
            }
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        if(Input.GetMouseButtonDown(0))
        {
            Shot();
        }
    }



    public void ToPlaneMode()
    {
        wheels = gameObject.GetComponent<CarController>().wheels;
        collider = gameObject.GetComponent<CarController>().car_collider;
        wheel_colliders = gameObject.GetComponent<CarController>().wheels_colliders;

        Destroy(gameObject.GetComponent<CarController>());
        Rigidbody rb = GetComponent<Rigidbody>();
      
        rb.mass = 1f;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        rb.velocity = new Vector3(0,0, rb.velocity.z);
        // rb.AddForce(new Vector3(0, 500f, 0), ForceMode.Force);
        car_mode = false;

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

        car_mode = true;
        gameObject.AddComponent<CarController>();

        gameObject.GetComponent<CarController>().wheels = wheels;
        gameObject.GetComponent<CarController>().car_collider = collider;
        gameObject.GetComponent<CarController>().wheels_colliders = wheel_colliders;

        foreach (GameObject o in wheels)
        {
            o.transform.parent.GetComponent<WheelCollider>().enabled = true;
        }

        rb.velocity = new Vector3(0, 0, f);

    }

    Vector3 aim_position;

    public void Shot()
    {
        GameObject o = Instantiate(bullet);
        o.transform.position = gun_exit.transform.position;
        Debug.Log(gun.transform.forward);
        o.GetComponent<Rigidbody>().AddForce(gun_exit.transform.forward * 1200f,ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        if(aim_position != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(aim_position, 0.5f);
        }
    }

    private void OnGUI()
    {
        //Update game aim
        if(working)
        {
            float max_aim_distance = 20f;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit inf;

            int layerMask = ~LayerMask.GetMask("Gun");
            layerMask = ~LayerMask.GetMask("Player");

            if (Physics.Raycast(ray,out inf,max_aim_distance,layerMask))
            {
                aim_position = inf.point;
            }
            else
            {
                aim_position = ray.origin + ray.direction * max_aim_distance;
            }

            //Limit aim height
            float distance = Vector2.Distance(new Vector2(aim_position.x, aim_position.z), new Vector2(transform.position.x, transform.position.z));
            aim_position.y = Mathf.Max(aim_position.y, this.transform.position.y + 0.25f + gun.transform.localPosition.y - distance/1.5f);
           
         
            GUI.DrawTexture(new Rect(Input.mousePosition.x - 20, (Screen.height - Input.mousePosition.y) - 20, 40, 40), aim);
            gun.transform.LookAt(aim_position);
        }
    }
}
