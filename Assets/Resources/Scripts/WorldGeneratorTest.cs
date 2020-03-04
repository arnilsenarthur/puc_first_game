using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneratorTest : MonoBehaviour
{
    public static WorldGeneratorTest WRLD;

    //Global
    public GameObject road_prefab;
    public GameObject cube_prefab;
    public GameObject ramp_prefab;
    float size_of_each = 9f;
    float start_at = -3;


    float f = 0;
    float camera_walk = 2;
  

    public int colliders_amount = 0;
   

    bool start_to_delete = false;
    List<GameObject> roads = new System.Collections.Generic.List<GameObject>();

    public SortedDictionary<int,int[]> can_pass = new SortedDictionary<int,int[]>();
    public List<int> can_pass_keys = new System.Collections.Generic.List<int>();
    public int last_can_pass = 0;


    // Start is called before the first frame update
    void Start()
    {
        f = start_at * size_of_each;
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
            camera_walk += size_of_each;
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
        f += size_of_each;

        roads.Add(o);

        if(start_to_delete)
        {
            can_pass.Remove(i - roads.Count);
            can_pass_keys.Remove(i);
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


        if (i % 8 == 0)
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

            can_pass_keys.Add(i);
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

    /*
    * Get chunk size
    */
    public float GetChunkSize()
    {
        return size_of_each;
    }

    /*
     * Get number of next chunk with obstacles
     */
    public int NextChunk(float z)
    {
        foreach (int i in can_pass_keys)
        {
            if ((i + 1) * size_of_each >= z)
                return i;
        }

        return -1;
    }

    /*
     * Get free lanes of next chunk with obstacles
    */
    public int[] NextChunkLanes(float z)
    {
       foreach(int i in can_pass_keys)
       {
            if ((i + 1) * size_of_each >= z)
                return can_pass[i];
       }

       return null;
    }
}
