using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator
{
    public static float GenerateBuilding(WorldGeneratorTest w, int size, int side, float z_position, GameObject chunk_parent)
    {
        /*
         * Random building data
         */
        float h = Random.Range(0, 4) * 4f + 8;
        float separator_size = Random.Range(5, 10) * 0.1f;


        /*
         * Make build
        */
        GameObject p = GameObject.Instantiate(w.building_level_prefab);
        p.transform.eulerAngles = new Vector3(-90f, 180f, 0f);
        p.transform.localScale = new Vector3((side == 0 ? 1 : -1) * 50f * size, 50f * size, 50f * h);
        p.transform.position = new Vector3(side == 0 ? (-Building.space_from_center_of_road - size / 2f) : (Building.space_from_center_of_road + size / 2f), h / 2, z_position + size / 2f);
        p.GetComponent<Renderer>().material.color = new Color(
              Random.Range(0f, 1f),
              Random.Range(0f, 1f),
              Random.Range(0f, 1f)
          );
        p.transform.parent = chunk_parent.transform;

        if (side == 0)
            for (int i = 0; i <= h / 2; i++)
            {
                {
                    GameObject c = GameObject.Instantiate(w.building_corner_prefab);

                    c.transform.eulerAngles = new Vector3(-90f, 0f, 180f);
                    c.transform.localScale = new Vector3(400, 400, 400);
                    c.transform.localPosition = new Vector3(-11.75f, i * 2f, p.transform.position.z - (size / 2f) + 2f);
                    c.transform.parent = p.transform;
                }

                {
                    GameObject c = GameObject.Instantiate(w.building_corner_prefab);

                    c.transform.eulerAngles = new Vector3(-90f, 0f, 180f);
                    c.transform.localScale = new Vector3(400, -400, 400);
                    c.transform.localPosition = new Vector3(-11.75f, i * 2f, p.transform.position.z + (size / 2f) - 2f);
                    c.transform.parent = p.transform;
                }

                if (i % 2 == 0 && i != 0)
                {
                    GameObject c = GameObject.Instantiate(w.building_level_separator_prefab);


                    c.transform.localPosition = new Vector3(p.transform.position.x, i * 2, p.transform.position.z);
                    c.transform.localEulerAngles = new Vector3(0, 0, 0);
                    c.transform.parent = chunk_parent.transform;
                    c.transform.localScale = new Vector3(50f * (size + 1f), separator_size * 50f, 50f * (size + 0.5f));
                }

            }
        else
            for (int i = 0; i <= h / 2; i++)
            {
                {
                    GameObject c = GameObject.Instantiate(w.building_corner_prefab);

                    c.transform.eulerAngles = new Vector3(-90f, 0f, 180f);
                    c.transform.localScale = new Vector3(-400, 400, 400);
                    c.transform.localPosition = new Vector3(11.75f, i * 2f, p.transform.position.z - (size / 2f) + 2f);
                    c.transform.parent = p.transform;
                }

                {
                    GameObject c = GameObject.Instantiate(w.building_corner_prefab);

                    c.transform.eulerAngles = new Vector3(-90f, 0f, 180f);
                    c.transform.localScale = new Vector3(-400, -400, 400);
                    c.transform.localPosition = new Vector3(11.75f, i * 2f, p.transform.position.z + (size / 2f) - 2f);
                    c.transform.parent = p.transform;
                }

                if (i % 2 == 0 && i != 0)
                {
                    GameObject c = GameObject.Instantiate(w.building_level_separator_prefab);

                  
                    c.transform.localPosition = new Vector3(p.transform.position.x, i * 2, p.transform.position.z);
                    c.transform.localEulerAngles = new Vector3(0, 0, 0);
                    c.transform.parent = chunk_parent.transform;
                    c.transform.localScale = new Vector3(50f * (size + 1f), separator_size * 50f, 50f * (size + 0.5f));
                }

            }

        return size;
    }
}
