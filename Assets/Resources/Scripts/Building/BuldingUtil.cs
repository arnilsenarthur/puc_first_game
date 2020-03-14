using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUtil
{
    public static GameObject GenerateRoad(WorldGeneratorTest w, int current_chunk_index)
    {
        return GameObject.Instantiate(w.road_prefab);
    }

    public static void GenerateSideWalkDecoration(WorldGeneratorTest w, int current_chunk_index, GameObject chunk_parent)
    {
        if(current_chunk_index % 2 == 0)
        {
            {
                GameObject lp1 = InstantiateAndSetParent(w, w.lightpost_prefab, chunk_parent);
                lp1.transform.localPosition = new Vector3(8.16832f, 3.11f ,- 4.7f);
            }

            {
                GameObject lp1 = InstantiateAndSetParent(w, w.lightpost_prefab, chunk_parent);
                lp1.transform.localPosition = new Vector3(-8.16832f, 3.11f ,- 4.7f);
                lp1.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        
        }

        if (current_chunk_index % 50 == 0)
        {
            GameObject sign_1 = InstantiateAndSetParent(w,w.kilometer_sign_prefab, chunk_parent);
            sign_1.transform.localPosition = new Vector3(-7.5f, 1, 10);
            SetSignNumber(w,sign_1, current_chunk_index / 10);

            GameObject sign_2 = InstantiateAndSetParent(w,w.kilometer_sign_prefab, chunk_parent);
            sign_2.transform.localPosition = new Vector3(7.5f, 1, 10);
            SetSignNumber(w,sign_2, current_chunk_index / 10);

        }
        else if (current_chunk_index % ((int)Random.Range(3, 7)) == 0)
        {
            bool a = Random.Range(0, 10) % 2 == 0;

            bool b = Random.Range(0, 10) % 2 == 0;


            if (a)
            {
                GameObject sign = InstantiateAndSetParent(w,w.shop_sign_prefab[(int)Random.Range(0,w.shop_sign_prefab.Length - 1)], chunk_parent);
                sign.transform.localPosition = new Vector3(-7.5f, 1.6f, 10);
            }

            if (b)
            {                
                GameObject sign = InstantiateAndSetParent(w,w.shop_sign_prefab[(int)Random.Range(0, w.shop_sign_prefab.Length - 1)], chunk_parent);
                sign.transform.localPosition = new Vector3(7.5f, 1.6f, 10);
            }

        }
    }

    public static int[] GenerateObstacles(WorldGeneratorTest w,int current_chunk_index,GameObject chunk_parent)
    {
        
        if (current_chunk_index % 8 == 0)
        {
            int[] arr = { 0, 1, 2, 3 };
            int type = (int)Random.Range(2, 7);
            // B--B
            if (type == 2)
            {
                createObstacle(w,chunk_parent, 0);
                createObstacle(w,chunk_parent, 3);

                arr = new int[] { 1, 2 };
            }

            // -BB-
            if (type == 3)
            {
                createObstacle(w, chunk_parent, 1);
                createObstacle(w, chunk_parent, 2);

                arr = new int[] { 0, 3 };
            }

            // BRRB
            if (type == 4)
            {
                createObstacle(w, chunk_parent, 0);
                createObstacle(w, chunk_parent, 3);
                createRamp(w, chunk_parent, 1);
                createRamp(w, chunk_parent, 2);

                arr = new int[] { 1, 2 };
            }

            // RBBR
            if (type == 5)
            {
                createObstacle(w, chunk_parent, 1);
                createObstacle(w, chunk_parent, 2);
                createRamp(w, chunk_parent, 0);
                createRamp(w, chunk_parent, 3);

                arr = new int[] { 0, 3 };
            }

            //R--R
            if (type == 6)
            {
                createRamp(w, chunk_parent, 0);
                createRamp(w, chunk_parent, 3);

                arr = new int[] { 0, 1, 2, 3 };
            }

            //-RR-
            if (type == 7)
            {
                createRamp(w, chunk_parent, 1);
                createRamp(w, chunk_parent, 2);

                arr = new int[] { 0, 1, 2, 3 };
            }


           

            foreach (int j in arr)
            {
                GenerateCoinsInLane(j, chunk_parent, true);
            }

            return arr;
        }

        return null;
    }

    public static float GenerateBuild(WorldGeneratorTest w, int min_size, int max_size, int side, float z_position, GameObject chunk_parent) 
    {
        float size = Random.Range(min_size, max_size);

        GenerateBuildWithFixedSize(w,(int) size, side, z_position, chunk_parent);

        return size;
    }

    public static float GenerateBuildWithFixedSize(WorldGeneratorTest w, int size, int side, float z_position, GameObject chunk_parent)
    {
        //Prédio
        return BuildingGenerator.GenerateBuilding(w, size, side, z_position, chunk_parent);
    }

    public static float GenerateBothSideBulding(WorldGeneratorTest w, float z_position, int current_chunk_index)
    {
        float pz = 0;

        int i = Random.Range(1, 10);
        int j = 0;
        while (i > 0 && (current_chunk_index + j) % 50 != 0)
        {
            i--;
            j++;

            GameObject p = GameObject.Instantiate(w.tunnel_prefab);
            p.transform.position = new Vector3(0, 5.69f, z_position + pz + 5f);

            w.unparented_both_side_building.Add(p);
            pz += 10;
        }

        return pz;
    }

    /*
     * 
     * 
     * 
     * 
    */

    static void GenerateCoinsInLane(int lane, GameObject h, bool ignore_pos)
    {
        for (int i = 0; i < WorldGeneratorTest.size_of_each; i++)
        {
            if (!ignore_pos || Mathf.Abs(i - WorldGeneratorTest.size_of_each / 2) > 0.5f)
            {
                GameObject c = GameObject.Instantiate(Game.game.prefab_coin);
                c.transform.parent = h.transform;
                c.transform.localPosition = new Vector3(WorldGeneratorTest.lanes_positon[lane], 1f, (i - WorldGeneratorTest.size_of_each / 2) * 3f);
            }
        }
    }


    static void createObstacle(WorldGeneratorTest w, GameObject chunk_parent, int lane)
    {
        GameObject ob = GameObject.Instantiate(w.cube_prefab);
        ob.transform.parent = chunk_parent.transform;
        ob.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        ob.transform.localPosition = new Vector3(WorldGeneratorTest.lanes_positon[lane], 0.06F, 0);
    }

    static void createRamp(WorldGeneratorTest w, GameObject chunk_parent, int lane)
    {
        GameObject ob = GameObject.Instantiate(w.ramp_prefab);
        ob.transform.parent = chunk_parent.transform;
        ob.transform.localPosition = new Vector3(WorldGeneratorTest.lanes_positon[lane], 0, 0);
    }

    static void SetSignNumber(WorldGeneratorTest w, GameObject o, int number)
    {

        InstantiateAndSetParentReset(w,w.alphabet_prefab[Mathf.FloorToInt(number / 100)], o.transform.Find("number_0").gameObject);
        InstantiateAndSetParentReset(w,w.alphabet_prefab[Mathf.FloorToInt(number / 10) % 10], o.transform.Find("number_1").gameObject);
        InstantiateAndSetParentReset(w,w.alphabet_prefab[number % 10], o.transform.Find("number_2").gameObject);

    }

    static GameObject InstantiateAndSetParentReset(WorldGeneratorTest w, GameObject prefab, GameObject parent)
    {
        GameObject o = GameObject.Instantiate(prefab);
        o.transform.parent = parent.transform;
        o.transform.localPosition = Vector3.zero;
        o.transform.localEulerAngles = Vector3.zero;
        o.transform.localScale = Vector3.one;
        return o;
    }

    static GameObject InstantiateAndSetParent(WorldGeneratorTest w, GameObject prefab, GameObject parent)
    {
        GameObject o = GameObject.Instantiate(prefab);
        o.transform.parent = parent.transform;
        return o;
    }
}
