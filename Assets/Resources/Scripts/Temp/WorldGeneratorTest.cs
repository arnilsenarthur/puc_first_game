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
    public GameObject lightpost_prefab;

    public GameObject building_level_prefab;
    public GameObject building_corner_prefab;
    public GameObject building_level_separator_prefab;


    public static float size_of_each = 10f;
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
            GenerateNewChunk();

        start_to_delete = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //Camera.main.transform.position += new Vector3(0, 0, Time.fixedDeltaTime * 0f);
        
        if(Camera.main.transform.position.z >= camera_walk)
        {
            camera_walk += size_of_each;
            GenerateNewChunk();
        }
    }
    int i = -3;

    void GenerateNewChunk()
    {
        //Generate Road
        i++;
        GameObject o = BuildingUtil.GenerateRoad(this, i);     
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

        //Generate road obstacles
        int[] pass = BuildingUtil.GenerateObstacles(this, i, o);
        if (pass != null)
        {
            can_pass[i] = pass;
            can_pass_keys.Add(i);
            last_can_pass = i;
        }

        //Generate sidewalk decoration
        BuildingUtil.GenerateSideWalkDecoration(this, i, o);

        //Generate building if need 
        if (try_to_build_in_both_sides)
        {
            if (f >= building_z[0] && f >= building_z[1])
            {
                if (building_z[0] == building_z[1])
                {
                    //Do the build
                    float f = BuildingUtil.GenerateBothSideBulding(this,building_z[0],i);
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

                    building_z[0] += BuildingUtil.GenerateBuildWithFixedSize(this,(int)Mathf.Min(ra, rb), 0, building_z[0], o);
                    building_z[1] += BuildingUtil.GenerateBuildWithFixedSize(this,(int)Mathf.Max(ra, rb), 1, building_z[1], o);


                }
                else
                {
                    float dif = Mathf.Abs(building_z[0] - building_z[1]);
                    float ra = Mathf.Round(Random.Range(16, 30 - dif));
                    float rb = Mathf.Round(ra + Random.Range(1, dif));

                    building_z[0] += BuildingUtil.GenerateBuildWithFixedSize(this, (int)Mathf.Max(ra, rb), 0, building_z[0], o);
                    building_z[1] += BuildingUtil.GenerateBuildWithFixedSize(this, (int)Mathf.Min(ra, rb), 1, building_z[1], o);
                }
            }
        }
        else
        for (int j = 0; j < 2; j ++)
        {
            while (f >= building_z[j])
            {
                //Create a build      
                building_z[j] += BuildingUtil.GenerateBuild(this,16,30,j,building_z[j],o);
            }
        }

        if(Mathf.Abs(f - last_both_side_building) >= 100 && !try_to_build_in_both_sides && (i % 50 != 0))
        {
            try_to_build_in_both_sides = true;
        }

        //Parent to both side buildings
        for (int i = unparented_both_side_building.Count - 1; i >= 0; i--)
        {
            if (unparented_both_side_building[i].transform.position.z <= f)
            {
                unparented_both_side_building[i].transform.parent = o.transform;
                unparented_both_side_building.RemoveAt(i);
            }
        }
    }

    public List<GameObject> unparented_both_side_building = new System.Collections.Generic.List<GameObject>();

   

    

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
