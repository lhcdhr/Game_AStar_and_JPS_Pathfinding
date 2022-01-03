using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Haochen Liu
 * 260917834
 * COMP521 A3
 * 
 * Randomly generating 10 obstacles in the 3 plain area.
 */
public class ObstacleGenerator : MonoBehaviour
{
    public GameObject o1;//single cube, not used
    public GameObject o2;//3*1 rec
    public GameObject o3;//1*3 rec
    public GameObject o4;//3*3
    public GameObject o5;//3*3 cross
    public GameObject o6;//5*1 rec
    public GameObject o7;//1*5 rec
    public GameObject o8;//5*3 cross, not used
    public GameObject o9;//3*5 cross
    public GameObject o10;//5*5 cross

    List<Vector3> obstacleCenters = new List<Vector3>();
    public static List<Vector3> occupiedCoords = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        // area1, 5 obstacles
        Vector3 coord1 = new Vector3(Random.Range(43, 57), 0, Random.Range(3, 38));
        GenerateO10(coord1);
        Vector3 coord2 = new Vector3(Random.Range(43, 57), 0, Random.Range(3, 38));
        while (true) {
            if (CheckOverlap(coord2)) {
                break;
            }
            coord2 = new Vector3(Random.Range(43, 57), 0, Random.Range(3, 38));
        }
        GenerateO6(coord2);
        Vector3 coord3 = new Vector3(Random.Range(43, 57), 0, Random.Range(3, 38));
        while (true)
        {
            if (CheckOverlap(coord3))
            {
                break;
            }
            coord3 = new Vector3(Random.Range(43, 57), 0, Random.Range(3, 38));
        }
        GenerateO7(coord3);
        
        Vector3 coord4 = new Vector3(Random.Range(43, 57), 0, Random.Range(3, 38));
        while (true)
        {
            if (CheckOverlap(coord4))
            {
                break;
            }
            coord4 = new Vector3(Random.Range(43, 57), 0, Random.Range(3, 38));
        }
        GenerateO4(coord4);
        Vector3 coord5 = new Vector3(Random.Range(43, 57), 0, Random.Range(3, 38));
        while (true)
        {
            if (CheckOverlap(coord5))
            {
                break;
            }
            coord5 = new Vector3(Random.Range(43, 57), 0, Random.Range(3, 38));
        }
        GenerateO9(coord5);
        // area2, 3 obstacles
        Vector3 coord6 = new Vector3(Random.Range(83, 97), 0, Random.Range(3, 38));
        while (true)
        {
            if (AvoidLowerTeleportArea(coord6))
            {
                break;
            }
            coord6 = new Vector3(Random.Range(83, 97), 0, Random.Range(3, 38));
        }
        GenerateO5(coord6);

        Vector3 coord7 = new Vector3(Random.Range(83, 97), 0, Random.Range(3, 38));
        while (true)
        {
            if (CheckOverlap(coord7) && AvoidLowerTeleportArea(coord7))
            {
                break;
            }
            coord7 = new Vector3(Random.Range(83, 97), 0, Random.Range(3, 38));
        }
        GenerateO2(coord7);

        Vector3 coord8 = new Vector3(Random.Range(83, 97), 0, Random.Range(3, 38));
        while (true)
        {
            if (CheckOverlap(coord8) && AvoidLowerTeleportArea(coord8))
            {
                break;
            }
            coord8 = new Vector3(Random.Range(83, 97), 0, Random.Range(3, 38));
        }
        GenerateO3(coord8);
        // area3, 2 obstacles
        Vector3 coord9 = new Vector3(Random.Range(92,98),29,Random.Range(3,38));
        while (true) {
            if (CheckOverlap(coord9) && AvoidUpperTeleportArea(coord9))
            {
                break;
            }
            coord9 = new Vector3(Random.Range(92, 98), 29, Random.Range(3, 38));
        }
        GenerateO2(coord9);

        Vector3 coord10 = new Vector3(Random.Range(92, 98), 29, Random.Range(3, 38));
        while (true)
        {
            if (CheckOverlap(coord10) && AvoidUpperTeleportArea(coord10))
            {
                break;
            }
            coord10 = new Vector3(Random.Range(92, 98), 29, Random.Range(3, 38));
        }
        GenerateO2(coord10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // avoid generating obstacles overlapping with teleport area on the upper level
    bool AvoidUpperTeleportArea(Vector3 toCheck) {
        if (toCheck.x >= 92 && toCheck.x <= 99 && toCheck.z >= 7 && toCheck.z <= 13)
        {
            return false;
        }
        if (toCheck.x >= 92 && toCheck.x <= 99 && toCheck.z >= 27 && toCheck.z <= 33)
        {
            return false;
        }
        return true;
    }
    // avoid generating obstacles overlapping with teleport area on the lower level
    bool AvoidLowerTeleportArea(Vector3 toCheck) {
        if (toCheck.x >= 92 && toCheck.x <= 99 && toCheck.z >= 7 && toCheck.z <= 13 ) {
            return false;
        }
        if (toCheck.x >= 92 && toCheck.x <= 99 && toCheck.z >= 27 && toCheck.z <= 33)
        {
            return false;
        }
        return true;

    }
    // check whether obstalces is potentially overlapping with the ones that have been generated
    bool CheckOverlap(Vector3 toCheck) {
        if (obstacleCenters.Count != 0) {
            foreach (Vector3 occupied in obstacleCenters) {
                if (Mathf.Abs(toCheck.x-occupied.x)<5 && Mathf.Abs(toCheck.z - occupied.z)<5) {
                    return false;
                }
            }
        }
        return true;
    }

    // generate obstacle at designated position,
    // and store the positions occupied by them.
    
    void GenerateO2(Vector3 coord)
    {
        obstacleCenters.Add(coord);
        occupiedCoords.Add(coord);
        occupiedCoords.Add(new Vector3(coord.x - 1, 29, coord.z));
        occupiedCoords.Add(new Vector3(coord.x + 1, 29, coord.z));
        Instantiate(o2, coord, o2.transform.rotation);
    }
    void GenerateO3(Vector3 coord)
    {
        obstacleCenters.Add(coord);
        occupiedCoords.Add(coord);
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z - 1));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z + 1));
        Instantiate(o3, coord, o3.transform.rotation);
    }
    void GenerateO4(Vector3 coord)
    {
        obstacleCenters.Add(coord);
        occupiedCoords.Add(coord);
        occupiedCoords.Add(new Vector3(coord.x - 1, 0, coord.z - 1));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z - 1));
        occupiedCoords.Add(new Vector3(coord.x + 1, 0, coord.z - 1));
        occupiedCoords.Add(new Vector3(coord.x - 1, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x + 1, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x - 1, 0, coord.z + 1));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z - 1));
        occupiedCoords.Add(new Vector3(coord.x + 1, 0, coord.z + 1));
        Instantiate(o4, coord, o4.transform.rotation);
    }
    void GenerateO5(Vector3 coord)
    {
        obstacleCenters.Add(coord);
        occupiedCoords.Add(coord);
        occupiedCoords.Add(new Vector3(coord.x - 1, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x + 1, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z - 1));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z + 1));
        Instantiate(o5, coord, o5.transform.rotation);
    }
    void GenerateO6(Vector3 coord)
    {
        obstacleCenters.Add(coord);
        occupiedCoords.Add(coord);
        occupiedCoords.Add(new Vector3(coord.x - 1, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x - 2, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x + 1, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x + 2, 0, coord.z));
        Instantiate(o6, coord, o6.transform.rotation);
    }
    void GenerateO7(Vector3 coord)
    {
        obstacleCenters.Add(coord);
        occupiedCoords.Add(coord);
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z - 1));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z - 2));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z + 1));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z + 2));
        Instantiate(o7, coord, o7.transform.rotation);
    }

    void GenerateO9(Vector3 coord)
    {

        obstacleCenters.Add(coord);
        occupiedCoords.Add(coord);
        occupiedCoords.Add(new Vector3(coord.x - 1, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x + 1, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z - 1));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z - 2));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z + 1));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z + 2));
        Instantiate(o9, coord, o9.transform.rotation);
    }
    void GenerateO10(Vector3 coord)
    {
        // a 5*5 cross
        obstacleCenters.Add(coord);
        occupiedCoords.Add(coord);
        occupiedCoords.Add(new Vector3(coord.x - 1, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x - 2, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x + 1, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x + 2, 0, coord.z));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z - 1));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z - 2));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z + 1));
        occupiedCoords.Add(new Vector3(coord.x, 0, coord.z + 2));
        Instantiate(o10, coord, o10.transform.rotation);
    }
}
