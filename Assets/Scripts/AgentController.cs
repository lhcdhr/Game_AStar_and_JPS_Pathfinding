using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Haochen Liu
 * 260917834
 * COMP521 A3
 * 
 * The control unit of agents.
 * Containing A* and JPS pathfinding algorithms and necessary help functions.
 */
public class AgentController : MonoBehaviour
{
    // source and destination
    public Vector3 source;
    public Vector3 destination;
    // generated path
    Stack<Point> path = new Stack<Point>();
    // coords occupied by obstacles
    public List<Vector3> obstacleCoords;
    // the grid-based map
    List<Vector3> gridMap = new List<Vector3>();
   
    List<Point> open = new List<Point>();
    List<Point> closed = new List<Point>();
    // the coord that occupied by another agent, this agent needs to 
    // avoid this coord
    public Vector3 toAvoid = new Vector3(0, 0, 0);
    // for teleport, but I did not finish the implementation of teleport
    public bool teleportCoolDown = false;
    public int teleportIndicator = 0;

    // statistics for Q6
    public int totalPathNum;
    public float totalAlgoRunTime;
    public int repathNum;
    public int abandonedNum;

    // Start is called before the first frame update
    void Start()
    {
        teleportIndicator = Random.Range(0, 1);
        obstacleCoords = new List<Vector3>(ObstacleGenerator.occupiedCoords);
        GenerateGridMap();

        source = gameObject.transform.position;
        destination = GenerateDestination();
        // based on the switch of manager, 
        // decide whether use A* or JPS
        if (!AgentManager.algoSwitch)
        {
            GenerateAStarPath();
        }
        else {
            GenerateJPSPath();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    // have a look at the first step
    public Vector3 PeekNextWalk() {
        if (path.Count != 0) {
            Point next = path.Peek();
            return next.pointCoord;
        }
        return new Vector3(-1, -1, -1);
        
    }
    // walk one step.
    public void walk()
    {
        if (path.Count != 0)
        {
            Point next = path.Pop();
            gameObject.transform.position = next.pointCoord;
            //Debug.Log(next.pointCoord);
        }
    }



    // Generate the grid-based map.
    // 
    void GenerateGridMap() {
        // area 1
        for (int i = 40; i < 61; i++) {
            for (int j = 0; j < 41; j++) {
                Vector3 coord = new Vector3(i, 0, j);
                gridMap.Add(coord);

            }
        }
        // area 2
        for (int i = 81; i < 101; i++) {
            for (int j = 0; j < 41; j++) {
                Vector3 coord = new Vector3(i, 0, j);
                // The agents not intended to use teleporter
                // will not enter teleport area. So I excluded
                // these points from its grid.
                if (teleportIndicator == 0)
                {
                    if (TeleportAreaCheck(coord))
                    {
                        gridMap.Add(coord);
                    }
                }
                else
                {
                    gridMap.Add(coord);
                }
            }
        }
        // area 3
        for (int i = 91; i < 101; i++) {
            for (int j = 0; j < 41; j++) {
                Vector3 coord = new Vector3(i, 29, j);
                // The agents not intended to use teleporter
                // will not enter teleport area. So I excluded
                // these points from its grid.
                if (teleportIndicator == 0)
                {
                    if (TeleportAreaCheck(coord))
                    {
                        gridMap.Add(coord);
                    }
                }
                else
                {
                    gridMap.Add(coord);
                }
            }
        }
        // blue bridge
        for (int i = 61; i < 81; i++) {
            gridMap.Add(new Vector3(i, 0, 20));
        }
        // two red bridges(slope)
        int height1 = 0;
        for (int i = 61; i < 91; i++) {
            gridMap.Add(new Vector3(i, height1, 10));
            height1++;

        }
        int height2 = 0;
        for (int i = 61; i < 91; i++) {
            gridMap.Add(new Vector3(i, height2, 30));
            height2++;
        }
    }
    // check whether this coord is good for pathing
    bool ValidCoordCheck(Vector3 toCheck)
    {
        return ExistInGridMap(toCheck) && ObstacleCheck(toCheck) && AgentCollisionCheck(toCheck);
    }
    // checking whether the neighbors of this point is valid
    // if so, add them to the neighbor list and return it.
    List<Point> GetNeighborPoints(Point p)
    {
        List<Point> neighbors = new List<Point>();
        Vector3 coord = p.pointCoord;
        // c1 to c4 are horizontal and vertical neighbors.
        Vector3 c1 = new Vector3(coord.x - 1, coord.y, coord.z);
        if (ValidCoordCheck(c1))// 
        {
            Point n1 = new Point(c1, destination);
            neighbors.Add(n1);
        }

        Vector3 c2 = new Vector3(coord.x + 1, coord.y, coord.z);
        if (ValidCoordCheck(c2))//
        {
            Point n2 = new Point(c2, destination);
            neighbors.Add(n2);
        }

        Vector3 c3 = new Vector3(coord.x, coord.y, coord.z - 1);
        if (ValidCoordCheck(c3))//
        {
            Point n3 = new Point(c3, destination);
            neighbors.Add(n3);
        }

        Vector3 c4 = new Vector3(coord.x, coord.y, coord.z + 1);
        if (ValidCoordCheck(c4))// 
        {
            Point n4 = new Point(c4, destination);
            neighbors.Add(n4);
        }
        // c5 c6 for checking whether can walk on the bridge
        Vector3 c5 = new Vector3(coord.x + 1, coord.y + 1, coord.z);
        if (ValidCoordCheck(c5))//
        {
            Point n5 = new Point(c5, destination);
            neighbors.Add(n5);
        }

        Vector3 c6 = new Vector3(coord.x - 1, coord.y - 1, coord.z);
        if (ValidCoordCheck(c6))//
        {
            Point n6 = new Point(c6, destination);
            neighbors.Add(n6);
        }
        // c7 to c10 are 4 diagonal points
        Vector3 c7 = new Vector3(coord.x - 1, coord.y, coord.z - 1);
        if (ValidCoordCheck(c7))//
        {
            Point n7 = new Point(c7, destination);
            neighbors.Add(n7);
        }
        Vector3 c8 = new Vector3(coord.x + 1, coord.y, coord.z - 1);
        if (ValidCoordCheck(c8))//
        {
            Point n8 = new Point(c8, destination);
            neighbors.Add(n8);
        }
        Vector3 c9 = new Vector3(coord.x - 1, coord.y, coord.z + 1);
        if (ValidCoordCheck(c9))//
        {
            Point n9 = new Point(c9, destination);
            neighbors.Add(n9);
        }
        Vector3 c10 = new Vector3(coord.x + 1, coord.y, coord.z + 1);
        if (ValidCoordCheck(c10))//
        {
            Point n10 = new Point(c10, destination);
            neighbors.Add(n10);
        }
        // code for teleport, but I didn't finish implementing it.
        /*
        if (teleportIndicator == 1)
        {
            if (coord.y == 0 && Vector3.Distance(coord, new Vector3(96, 0, 10)) < 2)
            {
                Point n7 = new Point(new Vector3(coord.x, 29, coord.z), destination);
                neighbors.Add(n7);
            }
            if (coord.y == 0 && Vector3.Distance(coord, new Vector3(96, 0, 29)) < 2)
            {
                Point n8 = new Point(new Vector3(coord.x, 29, coord.z), destination);
                neighbors.Add(n8);
            }
            if (coord.y == 29 && Vector3.Distance(coord, new Vector3(96, 29, 10)) < 2)
            {
                Point n9 = new Point(new Vector3(coord.x, 0, coord.z), destination);
                neighbors.Add(n9);
            }
            if (coord.y == 29 && Vector3.Distance(coord, new Vector3(96, 29, 30)) < 2)
            {
                Point n10 = new Point(new Vector3(coord.x, 0, coord.z), destination);
                neighbors.Add(n10);
            }
        }
        */
        return neighbors;

    }

    // check whether walking on this coord going to result in 
    // colliding with another agent
    bool AgentCollisionCheck(Vector3 toCheck)
    {
        if (toCheck == toAvoid) {
            return false;
        }
        return true;
    }
    // checking whether this coord is occupied by obstacles
    bool ObstacleCheck(Vector3 toCheck)
    {
        foreach (Vector3 coord in obstacleCoords)
        {
            if (toCheck == coord)
            {
                return false;
            }
        }
        return true;
    }
    // checking whether this coord is in the teleport area.
    bool TeleportAreaCheck(Vector3 toCheck)
    {
        if (toCheck.y == 29 && toCheck.x >= 94 && toCheck.x <= 98 && toCheck.z >= 8 && toCheck.z <= 12)
        {
            return false;
        }
        if (toCheck.y == 29 && toCheck.x >= 94 && toCheck.x <= 98 && toCheck.z >= 28 && toCheck.z <= 32)
        {
            return false;
        }

        if (toCheck.y == 0 && toCheck.x >= 94 && toCheck.x <= 98 && toCheck.z >= 8 && toCheck.z <= 12)
        {
            return false;
        }
        if (toCheck.y == 0 && toCheck.x >= 94 && toCheck.x <= 98 && toCheck.z >= 28 && toCheck.z <= 32)
        {
            return false;
        }
        return true;
    }
    // randomly generate a destination on the plain,
    // avoiding obstacles, known possibe colliding agent, and teleporting area
    public Vector3 GenerateDestination()
    {
        Vector3 coord = new Vector3();
        while (true)
        {
            int i = Random.Range(1, 4);
            if (i == 1)
            {
                coord = new Vector3(Random.Range(40, 61), 0, Random.Range(0, 41));
            }
            else if (i == 2)
            {
                coord = new Vector3(Random.Range(81, 101), 0, Random.Range(0, 41));
            }
            else
            {
                coord = new Vector3(Random.Range(91, 101), 29, Random.Range(0, 41));
            }
            if (AgentCollisionCheck(coord) && ObstacleCheck(coord) && TeleportAreaCheck(coord) && coord != source)
            {
                break;
            }
        }
        return coord;
    }
    // Check whether this coord is included in the grid map
    bool ExistInGridMap(Vector3 toCheck) {
        foreach (Vector3 p in gridMap) {
            if (toCheck == p) {
                return true;
            }
        }
        return false;
    }

    // check whether this point is included in the open list
    bool ExistInOpen(Point toCheck) {
        foreach (Point p in closed) {
            if (toCheck.pointCoord==p.pointCoord) {
                return true;
            }
        }
        return false;
    }
    // check whether this point is included in the closed list
    bool ExistInClosed(Point toCheck) {
        foreach (Point p in closed) {
            if (toCheck.pointCoord==p.pointCoord) {
                return true;
            }
        }
        return false;        
    }
    // generate a path using A* algorithm
    // change destination if there is no available path to the current destination
    public void GenerateAStarPath()
    {
        AStar();

        while (path.Count == 0)
        {
            abandonedNum++;
            destination = GenerateDestination();
            AStar();

        }

    }
    // the body of A* algorithm.
    void AStar()
    {
        // for algorithm runtime
        float startTime = Time.realtimeSinceStartup;
        // clear path and two lists
        path.Clear();
        open.Clear();
        closed.Clear();
        Point start = new Point(source, destination);
        open.Add(start);
        // there is some potential flaw in my implementation that sometimes happens,
        // so I limit the maximum iteration to make sure it will not run into infinite loop.
        int iterCount = 0;
        while (open.Count > 0&& iterCount < 200){//  
            Point currentPoint = open[0];
            // from the open list, choose the one with lowest f cost
            for (int i = 0; i < open.Count; i++) {
                Point tmpPoint = open[i];
                if (tmpPoint.f < currentPoint.f || (tmpPoint.f==currentPoint.f && tmpPoint.h<currentPoint.h)) {
                    currentPoint = tmpPoint;
                }
            }
            // remove this point from open list, and add to closed list
            open.Remove(currentPoint);
            closed.Add(currentPoint);
            //found a route, generate the path and terminate.
            if (currentPoint.pointCoord == destination) {
                // accumulate the runtime
                totalAlgoRunTime += (Time.realtimeSinceStartup - startTime);
                totalPathNum++;
                GeneratePath(start,currentPoint);
                return;
            }
            // get neighbor of this point
            List<Point> neighbors = GetNeighborPoints(currentPoint);
            foreach (Point neighbor in neighbors) {
                if (ExistInClosed(neighbor))
                {
                    continue;
                }
                // if this point has lower g cost or it is not in the open list,
                // set current point as its parent and then add it to open list
                int newG = currentPoint.g + CalculateG(currentPoint, neighbor);
                if (newG < neighbor.g || !ExistInOpen(neighbor))
                {
                    neighbor.g = newG;
                    neighbor.getF();
                    neighbor.parent = currentPoint;
                    if (!ExistInOpen(neighbor))
                    {
                        open.Add(neighbor);
                    }
                }  
            }
            iterCount++;
        }
        // for statistics
        totalPathNum++;
        totalAlgoRunTime += (Time.realtimeSinceStartup - startTime);
    }
    // generate the path based on the result of algorithm(parents of points)
    void GeneratePath(Point s, Point d)
    {
        Point currentPoint = d;
        while (!currentPoint.parent.Equals(s))
        {
            path.Push(currentPoint);
            currentPoint = currentPoint.parent;
        }
        try
        {
            path.Push(path.Peek().parent);
        }
        catch (System.InvalidOperationException)
        {
            destination = GenerateDestination();
            GenerateAStarPath();
        }
        catch (System.StackOverflowException)
        {
            destination = GenerateDestination();
            GenerateAStarPath();
        }

    }


    // following are major methods used for JPS


    List<Point> JPSGetNeighborPoints(Point p) {
        List<Point> neighbors = new List<Point>();
        Vector3 coord = p.pointCoord;
        // for starting point, check its neighbors as regular as we do in A*
        if (p.parent == null)
        {
            Vector3 c1 = new Vector3(coord.x - 1, coord.y, coord.z);
            if (ValidCoordCheck(c1))// 
            {
                Point n1 = new Point(c1, destination);
                neighbors.Add(n1);
            }

            Vector3 c2 = new Vector3(coord.x + 1, coord.y, coord.z);
            if (ValidCoordCheck(c2))//
            {
                Point n2 = new Point(c2, destination);
                neighbors.Add(n2);
            }

            Vector3 c3 = new Vector3(coord.x, coord.y, coord.z - 1);
            if (ValidCoordCheck(c3))//
            {
                Point n3 = new Point(c3, destination);
                neighbors.Add(n3);
            }

            Vector3 c4 = new Vector3(coord.x, coord.y, coord.z + 1);
            if (ValidCoordCheck(c4))// 
            {
                Point n4 = new Point(c4, destination);
                neighbors.Add(n4);
            }
            // bridge check
            Vector3 c5 = new Vector3(coord.x + 1, coord.y + 1, coord.z);
            if (ValidCoordCheck(c5))//
            {
                Point n5 = new Point(c5, destination);
                neighbors.Add(n5);
            }

            Vector3 c6 = new Vector3(coord.x - 1, coord.y - 1, coord.z);
            if (ValidCoordCheck(c6))//
            {
                Point n6 = new Point(c6, destination);
                neighbors.Add(n6);
            }

            // 4 points of diagonal
            Vector3 c7 = new Vector3(coord.x - 1, coord.y, coord.z-1);
            if (ValidCoordCheck(c7))//
            {
                Point n7 = new Point(c7, destination);
                neighbors.Add(n7);
            }
            Vector3 c8 = new Vector3(coord.x + 1, coord.y, coord.z - 1);
            if (ValidCoordCheck(c8))//
            {
                Point n8 = new Point(c8, destination);
                neighbors.Add(n8);
            }
            Vector3 c9 = new Vector3(coord.x - 1, coord.y, coord.z + 1);
            if (ValidCoordCheck(c9))//
            {
                Point n9 = new Point(c9, destination);
                neighbors.Add(n9);
            }
            Vector3 c10 = new Vector3(coord.x + 1, coord.y, coord.z + 1);
            if (ValidCoordCheck(c10))//
            {
                Point n10 = new Point(c10, destination);
                neighbors.Add(n10);
            }
            return neighbors;
        }

        
        int xPointing = (int)(coord.x - p.parent.pointCoord.x);
        int zPointing = (int)(coord.z - p.parent.pointCoord.z);
        // the direction of move, in unit size, in x and z direciton
        int xStep = Mathf.Clamp(xPointing, -1, 1);
        int zStep = Mathf.Clamp(zPointing, -1, 1);
        // along x and z axes
        if (xPointing == 0 || zPointing == 0)
        {
            // in x axis
            if (zPointing == 0)
            {
                Vector3 coordToCheck = new Vector3(coord.x + xStep, coord.y, coord.z);
                if (ValidCoordCheck(coordToCheck))
                {
                    Point toCheck = new Point(coordToCheck, destination);
                    neighbors.Add(toCheck);
                    // checking forced neighbors
                    Vector3 obs1 = new Vector3(coord.x, coord.y, coord.z + 1);
                    Vector3 fn1 = new Vector3(coord.x + xStep, coord.y, coord.z + 1);
                    Vector3 obs2 = new Vector3(coord.x, coord.y, coord.z - 1);
                    Vector3 fn2 = new Vector3(coord.x + xStep, coord.y, coord.z - 1);
                    if ((!ValidCoordCheck(obs1)) && ValidCoordCheck(fn1))
                    {
                        Point fnP1 = new Point(fn1, destination);
                        neighbors.Add(fnP1);
                    }
                    if ((!ValidCoordCheck(obs2)) && ValidCoordCheck(fn2))
                    {
                        Point fnP2 = new Point(fn2, destination);
                        neighbors.Add(fnP2);
                    }
                }
            }
            // in z axis, i.e. xPointing == 0
            else
            {
                Vector3 coordToCheck = new Vector3(coord.x, coord.y, coord.z + zStep);
                if (ValidCoordCheck(coordToCheck))
                {
                    Point toCheck = new Point(coordToCheck, destination);
                    neighbors.Add(toCheck);
                    // checking forced neighbors
                    Vector3 obs1 = new Vector3(coord.x + 1, coord.y, coord.z);
                    Vector3 fn1 = new Vector3(coord.x + 1, coord.y, coord.z + zStep);
                    Vector3 obs2 = new Vector3(coord.x - 1, coord.y, coord.z);
                    Vector3 fn2 = new Vector3(coord.x - 1, coord.y, coord.z + zStep);
                    if ((!ValidCoordCheck(obs1)) && ValidCoordCheck(fn1))
                    {
                        Point fnP1 = new Point(fn1, destination);
                        neighbors.Add(fnP1);
                    }
                    if ((!ValidCoordCheck(obs2)) && ValidCoordCheck(fn2))
                    {
                        Point fnP2 = new Point(fn2, destination);
                        neighbors.Add(fnP2);
                    }
                }
            }
        }
        // diagonal
        else {
            Vector3 neighborXPos = new Vector3(coord.x + xStep, coord.y, coord.z);
            bool checkXPos = ValidCoordCheck(neighborXPos);
            Vector3 neighborXNeg = new Vector3(coord.x - xStep, coord.y, coord.z);
            bool checkXNeg = ValidCoordCheck(neighborXNeg);
            Vector3 neighborZPos = new Vector3(coord.x, coord.y, coord.z + zStep);
            bool checkZPos = ValidCoordCheck(neighborZPos);
            Vector3 neighborZNeg = new Vector3(coord.x, coord.y, coord.z - zStep);
            bool checkZNeg = ValidCoordCheck(neighborZNeg);
            Vector3 diagonalStep = new Vector3(coord.x + xStep, coord.y, coord.z + zStep);
            bool checkDiagonalStep = ValidCoordCheck(diagonalStep);
            if (checkZPos) {
                neighbors.Add(new Point(neighborZPos, destination));
            }
            if (checkXPos) {
                neighbors.Add(new Point(neighborXPos, destination));
            }
            if ((checkZPos || checkXPos) && checkDiagonalStep) {
                neighbors.Add(new Point(diagonalStep, destination));
            }
            // forced neighbors
            if ((!checkXNeg) && checkZPos) {
                Vector3 fn = new Vector3(coord.x - xStep, coord.y, coord.z+zStep);
                if (ValidCoordCheck(fn)) {
                    neighbors.Add(new Point(fn,destination));
                }
            }
            if ((!checkZNeg) && checkXPos) {
                Vector3 fn = new Vector3(coord.x + xStep, coord.y, coord.z - zStep);
                if (ValidCoordCheck(fn))
                {
                    neighbors.Add(new Point(fn, destination));
                }
            }
        }
        return neighbors;
    }

    // find jump points recursively with directions, and limit the
    // the recursion depth by variable depth
    Point JPSJump(Vector3 currentPos, int xStep, int yStep, int zStep, int depth) {
        // reached an invalid coord
        if (!ValidCoordCheck(currentPos)) {
            return null;
        }
        // reached max depth or destination
        if (depth == 0 || currentPos == destination) {
            return new Point(currentPos, destination);
        }
        // diagonally checking
        if (xStep != 0 && zStep != 0 && yStep == 0)
        {
            if ((ValidCoordCheck(new Vector3(currentPos.x + xStep, currentPos.y, currentPos.z - zStep))
                    && !ValidCoordCheck(new Vector3(currentPos.x, currentPos.y, currentPos.z - zStep)))
                || (ValidCoordCheck(new Vector3(currentPos.x - xStep, currentPos.y, currentPos.z + zStep))
                    && !ValidCoordCheck(new Vector3(currentPos.x - xStep, currentPos.y, currentPos.z))))
            {
                return new Point(currentPos, destination);

            }
            //forced neighbor
            if (JPSJump(new Vector3(currentPos.x + xStep, currentPos.y, currentPos.z), xStep, yStep, 0, depth - 1) != null)
            {
                return new Point(currentPos, destination);
            }
            if (JPSJump(new Vector3(currentPos.x, currentPos.y, currentPos.z + zStep), 0, yStep, zStep, depth - 1) != null)
            {
                return new Point(currentPos, destination);
            }
        }
        // in x axis
        else if (xStep != 0 && yStep == 0)
        {
            if ((ValidCoordCheck(new Vector3(currentPos.x + xStep, currentPos.y, currentPos.z + 1))
                    && !ValidCoordCheck(new Vector3(currentPos.x, currentPos.y, currentPos.z + 1)))
                || (ValidCoordCheck(new Vector3(currentPos.x + xStep, currentPos.y, currentPos.z - 1))
                    && !ValidCoordCheck(new Vector3(currentPos.x, currentPos.y, currentPos.z - 1))))
            {
                return new Point(currentPos, destination);
            }
        }
        // in z axis
        else if (zStep != 0 && yStep == 0) {
            if ((ValidCoordCheck(new Vector3(currentPos.x + 1,currentPos.y,currentPos.z+zStep))
                    && !ValidCoordCheck(new Vector3(currentPos.x + 1,currentPos.y,currentPos.z)))
                ||(ValidCoordCheck(new Vector3(currentPos.x-1,currentPos.y,currentPos.z+zStep))
                    && !ValidCoordCheck(new Vector3(currentPos.x-1,currentPos.y,currentPos.z)))) 
            {
                return new Point(currentPos, destination);
            }
        }
        // following are for jumping in two sloped bridges

        // bridge1, from low to high
        if (destination.y == 29 && currentPos == new Vector3(60, 0, 10)) {
            return new Point(currentPos, destination);
        }
        if (destination.y == 29 && currentPos == new Vector3(61, 0, 10)) {
            return new Point(new Vector3(91,29,10), destination);
        }
        // bridge1, from high to low
        if (destination.y == 0 && currentPos == new Vector3(91,29,10)) {
            return new Point(currentPos,destination);
        }
        if (destination.y == 0 && currentPos == new Vector3(90, 29, 10)) {
            return new Point(new Vector3(60, 0, 10), destination);
        }
        // bridge2, from low to high
        if (destination.y == 29 && currentPos == new Vector3(60, 0, 30))
        {
            return new Point(currentPos, destination);
        }
        if (destination.y == 29 && currentPos == new Vector3(61, 0, 30))
        {
            return new Point(new Vector3(91, 29, 30), destination);
        }
        // bridge2, from high to low
        if (destination.y == 0 && currentPos == new Vector3(91, 29, 30))
        {
            return new Point(currentPos, destination);
        }
        if (destination.y == 0 && currentPos == new Vector3(90, 29, 30))
        {
            return new Point(new Vector3(60, 0, 30), destination);
        }


        // if all above not fit, keep going along the direction.
        Vector3 next = new Vector3(currentPos.x + xStep, currentPos.y, currentPos.z + zStep);
        return JPSJump(next,xStep,yStep,zStep,depth-1);
    }
    // start jumping with the direction be from currentPos to neighborPos
    Point JPSGetJumpPoint(Vector3 currentPos, Vector3 neighborPos) {
        int xStep = (int)(neighborPos.x - currentPos.x);
        int yStep = (int)(neighborPos.y - currentPos.y);
        int zStep = (int)(neighborPos.z - currentPos.z);
        return JPSJump(neighborPos,xStep,yStep,zStep,400);
    }

    // similar to that one for A*,
    // generate a path using JPS
    // if no path found, change destination
    // and search again
    public void GenerateJPSPath()
    {
        JPS();
        while (path.Count == 0)
        {
            abandonedNum++;
            destination = GenerateDestination();
            JPS();

        }
    }


    // similar to A*, but with some changes
    void JPS() {
        float startTime = Time.realtimeSinceStartup;
        path.Clear();
        open.Clear();
        closed.Clear();
        Point start = new Point(source, destination);
        open.Add(start);        
        while (open.Count > 0) {
            Point currentPoint = open[0];
            for (int i = 0; i < open.Count; i++)
            {
                Point tmpPoint = open[i];
                if (tmpPoint.f < currentPoint.f || (tmpPoint.f == currentPoint.f && tmpPoint.h < currentPoint.h))
                {
                    currentPoint = tmpPoint;
                }
            }
            open.Remove(currentPoint);
            closed.Add(currentPoint);
            if (currentPoint.pointCoord == destination) {
                totalAlgoRunTime += (Time.realtimeSinceStartup - startTime);
                totalPathNum++;
                JPSGeneratePath(start, currentPoint);
                //GeneratePath(start,currentPoint);
                return;
            }
            List<Point> neighbors = JPSGetNeighborPoints(currentPoint);
            foreach (Point neighbor in neighbors) {
                // get jump point in the designated direction
                Point jumpPoint = JPSGetJumpPoint(currentPoint.pointCoord,neighbor.pointCoord);
                // similar to what I did in A*
                if (jumpPoint != null) {
                    if (ExistInClosed(jumpPoint)) {
                        continue;
                    }
                    int newG = currentPoint.g + CalculateG(currentPoint,jumpPoint);
                    if (newG < jumpPoint.g || !ExistInOpen(jumpPoint)) {
                        jumpPoint.g = newG;
                        jumpPoint.getF();
                        jumpPoint.parent = currentPoint;
                        if (!ExistInOpen(jumpPoint)) {
                            open.Add(jumpPoint);
                        }
                    }
                }
            }
        }
        // for statistics
        totalPathNum++;
        totalAlgoRunTime += (Time.realtimeSinceStartup - startTime);
    }
    // generate the path from the result of JPS
    // goal is to connect the jump points with  points 
    // so the agent can move continuously
    void JPSGeneratePath(Point s, Point d) {
        Point currentPoint = d;
        while (!currentPoint.parent.Equals(s)) {
            path.Push(currentPoint);
            Vector3 currentCoord = currentPoint.pointCoord;
            Vector3 parentCoord = currentPoint.parent.pointCoord;
            // The first 4 parts are for connecting the jump points
            // of two sloped bridges.
            // The jump points for them are set to the two ends,
            // so here is just connecting them consecutively.
            if (currentCoord.x == 91 && currentCoord.y == 29 && currentCoord.z == 10
                && (currentCoord.y - parentCoord.y) > 0)
            {
                path.Push(new Point(new Vector3(90, 29, 10), destination));
                for (int i = 1; i < 30; i++)
                {
                    path.Push(new Point(new Vector3(90 - i, 29 - i, 10), destination));
                }
            }
            else if (currentCoord.x == 60 && currentCoord.y == 0 && currentCoord.z == 10
                && (currentCoord.y - parentCoord.y) < 0)
            {
                path.Push(new Point(new Vector3(61, 0, 10), destination));
                for (int i = 1; i < 30; i++)
                {
                    path.Push(new Point(new Vector3(61 + i, i, 10), destination));
                }
            }
            else if (currentCoord.x == 91 && currentCoord.y == 29 && currentCoord.z == 30
                && (currentCoord.y - parentCoord.y) > 0)
            {
                path.Push(new Point(new Vector3(90, 29, 30), destination));
                for (int i = 1; i < 30; i++)
                {
                    path.Push(new Point(new Vector3(90 - i, 29 - i, 30), destination));
                }
            }
            else if (currentCoord.x == 60 && currentCoord.y == 0 && currentCoord.z == 30
                && (currentCoord.y - parentCoord.y) < 0)
            {
                path.Push(new Point(new Vector3(61, 0, 30), destination));
                for (int i = 1; i < 30; i++)
                {
                    path.Push(new Point(new Vector3(61 + i, i, 30), destination));
                }
            }
            // this part is for regular connection of jump points on the plains.
            else
            {
                //Debug.Log("regular check here");
                int xDiff = (int)(parentCoord.x - currentCoord.x);
                int xRange = Mathf.Abs(xDiff);
                int xStep = Mathf.Clamp(xDiff, -1, 1);
                int zDiff = (int)(parentCoord.z - currentCoord.z);
                int zRange = Mathf.Abs(zDiff);
                int zStep = Mathf.Clamp(zDiff, -1, 1);
                
                int range = Mathf.Max(xRange, zRange);
                //Debug.Log(range);
                for (int i = 0; i < range; i++) {
                    //Debug.Log("inside for loop ");
                    Point middlePoint = new Point(new Vector3(currentCoord.x + xStep*(i+1), currentCoord.y, currentCoord.z + zStep * (i + 1)), destination);
                    //Debug.Log(middlePoint.pointCoord);
                    path.Push(middlePoint);
                }

            }
            currentPoint = currentPoint.parent;
        }
        if (path.Count != 0)
        {
            path.Peek().parent = s;
        }
        try
        {
            path.Push(path.Peek().parent);
        }
        catch (System.InvalidOperationException)
        {
            destination = GenerateDestination();
            GenerateJPSPath();
        }
        catch (System.StackOverflowException)
        {
            destination = GenerateDestination();
            GenerateJPSPath();
        }
    }
    // calculate the g cost using Euclidean distance
    public int CalculateG(Point p1, Point p2)
    {

        //Code for teleport, but I did not finish the implementation of teleport
        /*
        if (Vector3.Distance(p1.pointCoord, new Vector3(96, 0, 10)) < 2 && Vector3.Distance(p2.pointCoord, new Vector3(96, 30, 10)) < 2)
        {
            return 0;
        }
        if (Vector3.Distance(p1.pointCoord, new Vector3(96, 29, 10)) < 2 && Vector3.Distance(p2.pointCoord, new Vector3(96, 0, 10)) < 2)
        {
            return 0;
        }
        if (Vector3.Distance(p1.pointCoord, new Vector3(96, 0, 30)) < 2 && Vector3.Distance(p2.pointCoord, new Vector3(96, 30, 30)) < 2)
        {
            return 0;
        }
        if (Vector3.Distance(p1.pointCoord, new Vector3(96, 29, 10)) < 2 && Vector3.Distance(p2.pointCoord, new Vector3(96, 30, 10)) < 2)
        {
            return 0;
        }
        */     
        return (int) Vector3.Distance(p1.pointCoord, p2.pointCoord);
    }
}
