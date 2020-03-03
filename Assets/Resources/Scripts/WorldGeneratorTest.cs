using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneratorTest : MonoBehaviour
{
    public static WorldGeneratorTest WRLD;

    public GameObject road_prefab;
    public GameObject cube_prefab;
    public GameObject ramp_prefab;

    float f = -36f;
    float camera_walk = 2;
    float camera_step = 9;

    public int colliders_amount = 0;
   

    bool start_to_delete = false;
    List<GameObject> roads = new System.Collections.Generic.List<GameObject>();

    public SortedDictionary<int,int[]> can_pass = new SortedDictionary<int,int[]>();
    public int last_can_pass = 0;


    // Start is called before the first frame update
    void Start()
    {
        WRLD = this;
        for (int i = 0; i < 10; i ++)
        GenerateNewRoad();
        
        start_to_delete = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //Camera.main.transform.position += new Vector3(0, 0, Time.fixedDeltaTime * 0f);
        
        if(Camera.main.transform.position.z >= camera_walk)
        {
            camera_walk += camera_step;
            GenerateNewRoad();
        }
    }
    int i = -3;

    void GenerateNewRoad()
    {
        GameObject o = Instantiate(road_prefab);
        i++;
        o.name = "Chunk " + i;
        o.transform.position = new Vector3(0, 0,f);
        f += 9f;

        roads.Add(o);

        if(start_to_delete)
        {
            can_pass.Remove(i - roads.Count);
            Destroy(roads[0]);
            roads.RemoveAt(0);
        }



        /*
         *  BB-
         *  B-B
         *  -BB
         *  -B-
         *  
         *  RBR
         *  BRB
         *  RRR
         *  R
         *   R
         *    R
         */

        //obstacles BB-


        if (i % 11 == 0)
        {
            int type = (int) Random.Range(2, 8);
            if (type == 2)
            {
                createObstacle(o, -3);
                createObstacle(o, 0);
                can_pass[i] = new int[] { 2 };
               
            }
            if (type == 3)
            {
                createObstacle(o, -3);
                createObstacle(o, 3);
                can_pass[i] = new int[] { 1 };
            }

            if (type == 4)
            {
                createObstacle(o, 0);
                createObstacle(o, 3);
                can_pass[i] = new int[] { 0 };
            }

            //Ramp in midle
            if (type == 5)
            {
                createRamp(o,0);
                can_pass[i] = new int[] { 0,1,2};
            }

            if (type == 6)
            {
                createObstacle(o, -3);
                createRamp(o, 0);
                createObstacle(o, 3);
                can_pass[i] = new int[] {1};
            }

            if (type == 7)
            {
                createRamp(o, -3);
                createRamp(o,0);
                createRamp(o, 3);
                can_pass[i] = new int[] { 0, 1, 2 };
            }

            last_can_pass = i;
        }
    }

    void createObstacle(GameObject h,float x_pos)
    {
        GameObject ob = Instantiate(cube_prefab);
        ob.transform.parent = h.transform;
        ob.transform.localPosition = new Vector3(x_pos, 0, 0);
    }

    void createRamp(GameObject h, float x_pos)
    {
        GameObject ob = Instantiate(ramp_prefab);
        ob.transform.parent = h.transform;
        ob.transform.localPosition = new Vector3(x_pos, 0, 0);
    }
}
