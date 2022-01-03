using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Haochen Liu
 * 260917834
 * COMP521 A3
 * 
 * Point in the map.
 * Stores f,g, and h cost
 * and the parent point.
 */
public class Point
{
    public Vector3 pointCoord;
    public int f;
    public int g; // steps taken from start to this point
    public int h; // estimate distance from this point to destination
    public Point parent;
    public Point(Vector3 coord,Vector3 dest)
    {
        this.pointCoord = coord;
        // h cost can be calculated with destination given
        // Euclidean distance
        this.h = (int)Vector3.Distance(coord, dest);    
    }
    // calculate f cost
    public int getF()
    {
        f = g + h;
        return f;
    }
    // used, but not used often
    public bool Equals(Point toCompare)
    {
        return this.pointCoord == toCompare.pointCoord;
    }
}
