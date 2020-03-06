using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{

    public Texture2D car_indicator;

    /*
    * Main variables
    */
    public GameObject prefab;
    Rigidbody rb;
    float acceleration = 27f;
    float maxSpeed = 30f;
    float speedometer = 0;
    float max_angle = 15f;
    float brake = 0.5f;

    float turn_speed = 25f;

    bool working = true;

    /*
     * Raycast System
     */
    bool raycast_front = false;
    bool raycast_right = false;
    bool raycast_left = false;

    bool raycast_lleft = false;
    bool raycast_lright = false;

    float raycast_distance = 2f;

    /*
     * Car parts
     */
    public GameObject car_collider;
    public GameObject[] wheels;
    public WheelCollider[] wheels_colliders;

    float f = 2;

    float pos_x = 0;

    /*
     * Start method
     */
    void Start()
    {
        pos_x = transform.position.x;
        rb = GetComponent<Rigidbody>();
        //.timeScale = 0.5f;
        // Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;

        int i = Random.Range(0, 100);
        if (i > 75)
            f = 6;
        else if (i > 50)
            f = 4;
        else
            f = 5;
    }

    /*
     * Lane system
     */
    bool turning_left = false;
    bool turning_right = false;

    int lane = 1;
    bool sp = false;
    bool dest = false;
    int current_quad = -10;

    void FixedUpdate()
    {
        if(Controller.PLAYER.transform.position.z - transform.position.z > 10)
        {
            if(!working)
            {
                dest = true;
                if (Controller.PLAYER.working)
                    StartCoroutine("SpawnNewEnemyCar", 1f);
               

            }

            transform.localEulerAngles = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = new Vector3(transform.position.x, transform.position.y, Controller.PLAYER.transform.position.z - 10f);

        }
        else if(transform.position.y < -10)
        {
            if (working)
            {
                dest = true;
                if (Controller.PLAYER.working)
                    StartCoroutine("SpawnNewEnemyCar", 2f);
            }

        }

        //Change lane if needed
        int quad = WorldGeneratorTest.WRLD.NextChunk(transform.position.z);
        if (quad != current_quad)
        {
            current_quad = quad;
            int[] possibilities = WorldGeneratorTest.WRLD.NextChunkLanes(transform.position.z);
            if (possibilities != null)
            {
                lane = possibilities[(int)Random.Range(0, possibilities.Length - 1)];
                Debug.Log("Change lane: " + lane);
            }
        }

        //Update car speedometer
        speedometer = transform.InverseTransformDirection(rb.velocity).z;

        //Car forward direction
        Vector3 fw = new Vector3(1 * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad), 0, 1 * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad));


        if (working)
        {
            //Update raycasts
            {
                int layer_mask = LayerMask.GetMask("AIAvoid");

                Vector3 pos = transform.position + new Vector3(0, 1f, 0);
                Vector3 right = new Vector3(1 * Mathf.Sin((transform.eulerAngles.y + 45) * Mathf.Deg2Rad), 0, 1 * Mathf.Cos((transform.eulerAngles.y + 45) * Mathf.Deg2Rad));
                Vector3 left = new Vector3(1 * Mathf.Sin((transform.eulerAngles.y - 45) * Mathf.Deg2Rad), 0, 1 * Mathf.Cos((transform.eulerAngles.y - 45) * Mathf.Deg2Rad));
                Vector3 front = pos + fw * 1.2f;
                Vector3 lateral = new Vector3(1 * Mathf.Sin((transform.eulerAngles.y + 90) * Mathf.Deg2Rad), 0, 1 * Mathf.Cos((transform.eulerAngles.y + 90) * Mathf.Deg2Rad));

                raycast_front = (Physics.Raycast(front + lateral * 0.3f, fw, raycast_distance * 2.5f,layer_mask)) || (Physics.Raycast(front + lateral * -0.3f, fw, raycast_distance * 2.5f, layer_mask));
                raycast_right = (Physics.Raycast(front, right, raycast_distance * 2f,layer_mask));
                raycast_left = (Physics.Raycast(front, left, raycast_distance * 2f, layer_mask));

                Vector3 rotl = new Vector3(1 * Mathf.Sin((transform.eulerAngles.y + 60) * Mathf.Deg2Rad), 0, 1 * Mathf.Cos((transform.eulerAngles.y + 60) * Mathf.Deg2Rad));
                Vector3 rotr = new Vector3(1 * Mathf.Sin((transform.eulerAngles.y - 60) * Mathf.Deg2Rad), 0, 1 * Mathf.Cos((transform.eulerAngles.y - 60) * Mathf.Deg2Rad));

                raycast_lleft = (Physics.Raycast(pos + lateral, rotl, raycast_distance/1.5f, layer_mask));
                raycast_lright = (Physics.Raycast(pos - lateral, rotr, raycast_distance / 1.5f, layer_mask));
            }

            turning_left = false;
            turning_right = false;

            //See if need to move in lanes
            float target_x = WorldGeneratorTest.lanes_positon[lane];

            float turn_speed = this.turn_speed;

            if (transform.position.x - target_x > 0.3f)
            {
                turning_left = true && ((wheels_colliders[0].isGrounded && wheels_colliders[1].isGrounded) || (wheels_colliders[2].isGrounded && wheels_colliders[3].isGrounded));
                if (!raycast_lleft)
                {      
                    turn_speed *= (transform.position.x - target_x) / 10f;
                    rb.AddForce(new Vector3(1.5f, 0, 0) * (transform.position.x - target_x) / 10f * 8f, ForceMode.Acceleration);
                }
            }
            else if (transform.position.x - target_x < -0.3f)
            {
                turning_right = true && ((wheels_colliders[0].isGrounded && wheels_colliders[1].isGrounded) || (wheels_colliders[2].isGrounded && wheels_colliders[3].isGrounded));
                if (!raycast_lright)
                { 
                    turn_speed *= (transform.position.x - target_x) / -10f;
                    rb.AddForce(new Vector3(1.5f, 0, 0) * (transform.position.x - target_x) / -10f * 8f, ForceMode.Acceleration);
                }
            }

         
            float pos_z = Controller.PLAYER.transform.position.z;
            float front_offset = f;
            //Accelerate to follow player front
            if(this.transform.position.z < pos_z - 5f)
            {

                rb.AddForce(fw * acceleration * 2f, ForceMode.Acceleration);
            }
            
            if (raycast_front )
            {
                rb.AddForce(fw * -acceleration * 2f, ForceMode.Acceleration);
               
            }

           else
           if (turning_left || turning_right)
            {

               
                if (raycast_lright || raycast_lleft || raycast_left || raycast_right)
                {
                    rb.AddForce(fw * -acceleration * 0.5f, ForceMode.Acceleration);

                    if (raycast_lright || raycast_lleft)
                    {
                        turning_left = false;
                        turning_right = false;
                        rb.AddForce(fw * acceleration * 0.25f, ForceMode.Acceleration);
                    }
                }
                else
                {
                    rb.AddForce(fw * acceleration * 0.5f, ForceMode.Acceleration);
                    rb.AddForce(new Vector3(0, 0, 1) * 10f, ForceMode.Acceleration);
                }



            }
            else if (pos_z + front_offset > this.transform.position.z)
            {
                rb.AddForce(fw * acceleration, ForceMode.Acceleration);
            }
            else if (pos_z + front_offset * 3 < this.transform.position.z)
            {
                rb.AddForce(fw * -acceleration, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(fw * -acceleration * brake, ForceMode.Acceleration);
            }



            if (turning_left)
            {
                //Brake car for safe rotation
                rb.AddForce(new Vector3(0, 0, -speedometer * 1.5f), ForceMode.Acceleration);

                //Rotate car
                rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, Quaternion.Euler(0, -max_angle, 0), turn_speed * Time.fixedDeltaTime);

                //Stability
                rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0), 100f * Time.fixedDeltaTime);

                //Rotate tires
                wheels[0].transform.localRotation = Quaternion.RotateTowards(wheels[0].transform.localRotation, Quaternion.Euler(0, -max_angle * 2, 0), 80f * Time.fixedDeltaTime);
                wheels[1].transform.localRotation = Quaternion.RotateTowards(wheels[1].transform.localRotation, Quaternion.Euler(0, -max_angle * 2, 0), 80f * Time.fixedDeltaTime);

            }
            else if (turning_right)
            {
                //Brake car for safe rotation
                rb.AddForce(new Vector3(0, 0, -speedometer * 1.5f), ForceMode.Acceleration);

                //Rotate car
                rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, Quaternion.Euler(0, max_angle, 0), turn_speed * Time.deltaTime);

                //Stability
                rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0), 100f * Time.fixedDeltaTime);

                //Rotate tires
                wheels[0].transform.localRotation = Quaternion.RotateTowards(wheels[0].transform.localRotation, Quaternion.Euler(0, max_angle * 2, 0), 80f * Time.fixedDeltaTime);
                wheels[1].transform.localRotation = Quaternion.RotateTowards(wheels[1].transform.localRotation, Quaternion.Euler(0, max_angle * 2, 0), 80f * Time.fixedDeltaTime);

            }
            else
            {
                //Return car to intial rotation
                rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, Quaternion.Euler(0, 0, 0), 80f * Time.fixedDeltaTime);

                //Rotate tires
                wheels[0].transform.localRotation = Quaternion.RotateTowards(wheels[0].transform.localRotation, Quaternion.Euler(0, 0, 0), 80f * Time.fixedDeltaTime);
                wheels[1].transform.localRotation = Quaternion.RotateTowards(wheels[1].transform.localRotation, Quaternion.Euler(0, 0, 0), 80f * Time.fixedDeltaTime);
            }


            //Add car stability
            rb.transform.localRotation = Quaternion.RotateTowards(rb.transform.localRotation, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 0), 300f * Time.fixedDeltaTime);


            //Max speed limit
            if (speedometer > maxSpeed)
            {
                rb.AddForce(fw * -acceleration, ForceMode.Acceleration);
            }

            if(rb.velocity.z < 0)
            {
                rb.AddForce(new Vector3(0,0, -rb.velocity.z), ForceMode.Acceleration);
            }

            if (speedometer < 0)
            {
                rb.AddForce(fw * acceleration, ForceMode.Acceleration);
            }




        }

        //Max speed limit
        if (speedometer > maxSpeed)
        {
            rb.AddForce(fw * -acceleration, ForceMode.Acceleration);
        }
    }

    private void OnDrawGizmos()
    {
        if (!working)
            return;

        Vector3 pos = transform.position + new Vector3(0, 1f, 0);  
        Vector3 fw = new Vector3(1 * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad), 0, 1 * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad));

        Vector3 right = new Vector3(1 * Mathf.Sin((transform.eulerAngles.y + 45) * Mathf.Deg2Rad), 0, 1 * Mathf.Cos((transform.eulerAngles.y + 45) * Mathf.Deg2Rad));
        Vector3 left = new Vector3(1 * Mathf.Sin((transform.eulerAngles.y - 45) * Mathf.Deg2Rad), 0, 1 * Mathf.Cos((transform.eulerAngles.y - 45) * Mathf.Deg2Rad));


        Vector3 front = pos + fw * 1.2f;
       

        Gizmos.color = raycast_front ? Color.red : Color.green;
        Gizmos.DrawLine(front, front + fw * raycast_distance);
        Gizmos.color = raycast_right ? Color.red : Color.green;
        Gizmos.DrawLine(front, front + right * raycast_distance);
        Gizmos.color = raycast_left ? Color.red : Color.green;
        Gizmos.DrawLine(front, front + left * raycast_distance);


        Vector3 lateral = new Vector3(1 * Mathf.Sin((transform.eulerAngles.y + 90) * Mathf.Deg2Rad), 0, 1 * Mathf.Cos((transform.eulerAngles.y + 90) * Mathf.Deg2Rad));
        Vector3 rotl = new Vector3(1 * Mathf.Sin((transform.eulerAngles.y + 60) * Mathf.Deg2Rad), 0, 1 * Mathf.Cos((transform.eulerAngles.y + 60) * Mathf.Deg2Rad));
        Vector3 rotr = new Vector3(1 * Mathf.Sin((transform.eulerAngles.y - 60) * Mathf.Deg2Rad), 0, 1 * Mathf.Cos((transform.eulerAngles.y - 60) * Mathf.Deg2Rad));

        Gizmos.color = raycast_lleft ? Color.red : Color.green;
        Gizmos.DrawLine(pos + lateral, pos + lateral + rotl * raycast_distance / 1.5f);

        Gizmos.color = raycast_lright ? Color.red : Color.green;
        Gizmos.DrawLine(pos - lateral, pos - lateral + rotr * raycast_distance/1.5f);

        if(lane >= 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(new Vector3(WorldGeneratorTest.lanes_positon[lane], 0, WorldGeneratorTest.WRLD.GetChunkSize() * current_quad),0.5f);
      
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 3)
        {
            car_collider.GetComponent<DeformableMesh>().OnCollisionEnter(collision);
        }

        if (collision.other.tag != "Collidable")
            return;

        //Hitted bottom
        if (collision.collider is BoxCollider)
            return;

        //On crash
        if (working)
        {
            working = false;
        }
       
    }

    public IEnumerator SpawnNewEnemyCar(float time)
    {
        if (!sp)
        {
            working = false;
            yield return new WaitForSeconds(time);
            GameObject o = Instantiate(prefab);
            o.transform.position = new Vector3(pos_x, 0, Controller.PLAYER.transform.position.z - 10f);
            o.GetComponent<CarAI>().prefab = prefab;
            o.GetComponent<CarAI>().car_indicator = car_indicator;
            o.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 1) * acceleration * 6f, ForceMode.Acceleration);

            if (dest)
            {
                Destroy(gameObject);
            }
            sp = true;
        }
        yield return null;

    }

    public void OnGUI()
    {
        //Show next to spawn
        if (gameObject.transform.position.z < Camera.main.transform.position.z + 5f)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            GUI.DrawTexture(new Rect(screenPos.x - 20, Screen.height - 58, 40, 48), car_indicator);
        }
    }
}
