using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    static Point[,] grid = new Point[101, 41];
    
    
    void Start()
    {
        
    }
    /*
    void GenerateGrid() {
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 41; j++) {
                grid[i, j] = new Point(new Vector3(i, 0, j));
                
            }
        }
        for (int i = 11; i < 40; i++) {
            for (int j = 0; j < 10; j++) { 
                
            }
        }
    }
    */
}
