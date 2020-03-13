using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneratorTest : MonoBehaviour
{
    public static WorldGeneratorTest WRLD;
    public static float[] lanes_positon = {-3.75f, -1.25f, 1.25f, 3.75f};

    //Global
    public GameObject road_prefab;
    public GameObject cube_prefab;
    public GameObject ramp_prefab;
    public GameObject kilometer_sign_prefab;
    public GameObject[] shop_sign_prefab;
    public GameObject[] alphabet_prefab;
    public GameObject build_prefab;
    public GameObject tunnel_prefab;

    float size_of_each = 10f;
    float start_at = -3;



    float f = 0;
    float camera_walk = 2;
  

    public int colliders_amount = 0;
    
    //Building info
    public float[] building_z = {0,0};
    public bool try_to_build_in_both_sides = false;
    public float last_both_side_building = -1;
   

    bool start_to_delete = false;
    List<GameObject> roads = new System.Collections.Generic.List<GameObject>();

    public SortedDictionary<int,int[]> can_pass = new SortedDictionary<int,int[]>();
    public List<int> can_pass_keys = new System.Collections.Generic.List<int>();
    public int last_can_pass = 0;


    // Start is called before the first frame update
    void Start()
    {
        f = start_at * size_of_each;

        building_z[0] = f;
        building_z[1] = f;

        WRLD = this;
        for (int i = 0; i < 16; i ++)
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
        //Generate Road
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

        //Generate obstacles
        if (i % 8 == 0)
        {
            int type = (int) Random.Range(2, 7);
            // B--B
            if (type == 2)
            {
                createObstacle(o, 0);
                createObstacle(o, 3);

                can_pass[i] = new int[] { 1,2 };          
            }

            // -BB-
            if (type == 3)
            {
                createObstacle(o, 1);
                createObstacle(o, 2);

                can_pass[i] = new int[] { 0, 3 };
            }

            // BRRB
            if (type == 4)
            {
                createObstacle(o, 0);
                createObstacle(o, 3);
                createRamp(o, 1);
                createRamp(o, 2);

                can_pass[i] = new int[] { 1, 2 };
            }

            // RBBR
            if (type == 5)
            {
                createObstacle(o, 1);
                createObstacle(o, 2);
                createRamp(o, 0);
                createRamp(o, 3);

                can_pass[i] = new int[] { 0, 3 };
            }

            //R--R
            if (type == 6)
            {
                createRamp(o, 0);
                createRamp(o, 3);

                can_pass[i] = new int[] { 0,1,2,3 };
            }

            //-RR-
            if (type == 7)
            {
                createRamp(o, 1);
                createRamp(o, 2);

                can_pass[i] = new int[] { 0, 1, 2, 3 };
            }


            can_pass_keys.Add(i);
            last_can_pass = i;

            foreach(int j in can_pass[i])
            {
                GenerateCoinsInLane(j, o, true);
            }
        }
        else
        {
          
        }

        //Generate decoration
        //Kilometer sign
        if(i%50 == 0)
        {
            GameObject sign_1 = InstantiateAndSetParent(kilometer_sign_prefab,o);
            sign_1.transform.localPosition = new Vector3(-7.5f, 1, 10);
            SetSignNumber(sign_1, i/10);

            GameObject sign_2 = InstantiateAndSetParent(kilometer_sign_prefab,o);
            sign_2.transform.localPosition = new Vector3(7.5f, 1, 10);
            SetSignNumber(sign_2, i/10);

        }
        else if(i%((int) Random.Range(3,7)) == 0)
        {
            bool a = Random.Range(0, 10) % 2 == 0;

            bool b = Random.Range(0, 10) % 2 == 0;

          
            if(a)
            {
                GameObject sign = InstantiateAndSetParent(shop_sign_prefab[(int) Random.Range(0,shop_sign_prefab.Length - 1)], o);
                sign.transform.localPosition = new Vector3(-7.5f, 1.6f, 10);
            }

            if (b)
            {
                GameObject sign = InstantiateAndSetParent(shop_sign_prefab[(int)Random.Range(0, shop_sign_prefab.Length - 1)], o);
                sign.transform.localPosition = new Vector3(7.5f, 1.6f, 10);
            }

        }

        //Generate building if need 
        if (try_to_build_in_both_sides)
        {
            if (f >= building_z[0] && f >= building_z[1])
            {
                if (building_z[0] == building_z[1])
                {
                    //Generate same building 
                    Debug.Log("Same Size");

                    //Do the build
                    float f = doBothSideBuilding(building_z[0],i);
                    building_z[0] += f;
                    building_z[1] += f;
                    try_to_build_in_both_sides = false;

                    last_both_side_building = building_z[0];
                   
                }
                else if (building_z[0] > building_z[1])
                {
                    float dif = Mathf.Abs(building_z[0] - building_z[1]);

                    float ra = Mathf.Round(Random.Range(16, 30 - dif));
                    float rb = Mathf.Round(ra + Random.Range(1, dif));

                    Debug.Log("Left is bigger... Rearranjing...");
                  //  Debug.Log("--------------------");
                   // Debug.Log("Distance: " + dif);
                   // Debug.Log("Pre BSize: " + building_z[0] + " " + building_z[1]);
                   

                    building_z[0] += doBuildFixedSize(Mathf.Min(ra, rb), 0, building_z[0], o);
                    building_z[1] += doBuildFixedSize(Mathf.Max(ra, rb), 1, building_z[1], o);

                  //  Debug.Log("Pos BSize: " + building_z[0] + " " + building_z[1]);
                   // Debug.Log("--------------------");


                }
                else
                {
                    float dif = Mathf.Abs(building_z[0] - building_z[1]);

                    float ra = Mathf.Round(Random.Range(16, 30 - dif));
                    float rb = Mathf.Round(ra + Random.Range(1, dif));

                    Debug.Log("Right is bigger... Rearranjing...");
                   // Debug.Log("--------------------");
                   // Debug.Log("Distance: " + dif);
                   // Debug.Log("Pre BSize: " + building_z[0] + " " + building_z[1]);

                    building_z[0] += doBuildFixedSize(Mathf.Max(ra, rb), 0, building_z[0], o);
                    building_z[1] += doBuildFixedSize(Mathf.Min(ra, rb), 1, building_z[1], o);

                 //   Debug.Log("Pos BSize: " + building_z[0] + " " + building_z[1]);
                  //  Debug.Log("--------------------");
                }
            }
        }
        else
        for (int j = 0; j < 2; j ++)
        {
            while (f >= building_z[j])
            {
                Debug.Log("Needs to generate: " + j + " " + i);

                //Create a build
                float size = doBuild(16, 30,j, building_z[j],o);
             
                building_z[j] += size;
            }
        }

        if(Mathf.Abs(f - last_both_side_building) >= 100 && !try_to_build_in_both_sides && (i % 50 != 0))
        {
            try_to_build_in_both_sides = true;
        }

        //Parent to both side buildings
        for (int i = unparented_both_side_building.Count - 1; i >= 0; i--)
        {
            if (unparented_both_side_building[i].transform.position.y >= f)
            {
                unparented_both_side_building[i].transform.parent = o.transform;
                unparented_both_side_building.RemoveAt(i);
            }
        }
    }

    public List<GameObject> unparented_both_side_building = new System.Collections.Generic.List<GameObject>();

    float doBothSideBuilding(float z_position,int current_chunk_index)
    {
        float pz = 0;

        int i = Random.Range(1, 10);
        int j = 0;
        while(i > 0 && (current_chunk_index + j)%50 != 0)
        {
            i--;
            j++;

            GameObject p = Instantiate(tunnel_prefab);
            p.transform.position = new Vector3(0, 5.69f, z_position + pz + 5f);

            unparented_both_side_building.Add(p);
            pz += 10;
        }

        return pz;
    }

    float doBuild(int min_size,int max_size,int side,float z_position,GameObject chunk_parent)
    {
        float size = Random.Range(min_size, max_size);

        doBuildFixedSize(size, side, z_position, chunk_parent);

        return size;
    }

    float doBuildFixedSize(float size, int side, float z_position, GameObject chunk_parent)
    {
        float h = Random.Range(0, 8) + 10;
        GameObject p = Instantiate(build_prefab);
        p.transform.localScale = new Vector3(1f * size, h, 1f * size);
        p.transform.position = new Vector3(side == 1 ? (-Building.space_from_center_of_road - size / 2f) : (Building.space_from_center_of_road + size / 2f), h / 2, z_position + size / 2f);
        p.GetComponent<Renderer>().material.color = new Color(
              Random.Range(0f, 1f),
              Random.Range(0f, 1f),
              Random.Range(0f, 1f)
          );
        p.transform.parent = chunk_parent.transform;

        return size;
    }

    void SetSignNumber(GameObject o,int number)
    {

        InstantiateAndSetParentReset(alphabet_prefab[Mathf.FloorToInt(number / 100)], o.transform.Find("number_0").gameObject);
        InstantiateAndSetParentReset(alphabet_prefab[Mathf.FloorToInt(number / 10)%10], o.transform.Find("number_1").gameObject);
        InstantiateAndSetParentReset(alphabet_prefab[number%10], o.transform.Find("number_2").gameObject);

    }

    GameObject InstantiateAndSetParentReset(GameObject prefab, GameObject parent)
    {
        GameObject o = Instantiate(prefab);
        o.transform.parent = parent.transform;
        o.transform.localPosition = Vector3.zero;
        o.transform.localEulerAngles = Vector3.zero;
        o.transform.localScale = Vector3.one;
        return o;
    }

    GameObject InstantiateAndSetParent(GameObject prefab,GameObject parent)
    {
        GameObject o = Instantiate(prefab);
        o.transform.parent = parent.transform;
        return o;
    }

    void GenerateCoinsInLane(int lane,GameObject h,bool ignore_pos)
    {
        for(int i = 0; i < size_of_each; i ++)
        {
            if (!ignore_pos || Mathf.Abs(i - size_of_each/2) > 0.5f)
            {
                GameObject c = Instantiate(Game.game.prefab_coin);
                c.transform.parent = h.transform;
                c.transform.localPosition = new Vector3(lanes_positon[lane], 1f, (i-size_of_each/2) * 3f);
            }
        }
    }


    void createObstacle(GameObject h,int lane)
    {
        GameObject ob = Instantiate(cube_prefab);
        ob.transform.parent = h.transform;
        ob.transform.eulerAngles = new Vector3(0, Random.Range(0,360), 0);
        ob.transform.localPosition = new Vector3(lanes_positon[lane], 0.06F, 0);
    }

    void createRamp(GameObject h, int lane)
    {
        GameObject ob = Instantiate(ramp_prefab);
        ob.transform.parent = h.transform;
        ob.transform.localPosition = new Vector3(lanes_positon[lane], 0, 0);
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
