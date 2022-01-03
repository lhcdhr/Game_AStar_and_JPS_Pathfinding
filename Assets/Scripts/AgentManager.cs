using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Haochen Liu
 * 260917834
 * COMP521 A3
 * 
 * The managing unit of agents.
 * Responsible for generating agents and letting them move.
 */
public class AgentManager : MonoBehaviour
{

    public GameObject agent;

    List<Vector3> obstacleCoords;

    static LinkedList<Vector3> agentCoords = new LinkedList<Vector3>();
    // default for 4 agents
    public int numAgent = 4;
    // generated agents
    static List<GameObject> generatedAgents = new List<GameObject>();
    // count the index of walked agents
    // will restore when all agents has walked one step
    int walkAgentCounter = 0;
    // false for A*, true for JPS. Can be changed in AgentManager's inspector from Editor.
    public bool FalseAstarTrueJPS = false;
    public static bool algoSwitch = false;
    
    float timer = 0f;
    // run for 2 min, which is 120 seconds
    float stop = 120f;
    public Text displayTime;
    public Text displayResult;
    // Start is called before the first frame update
    void Start()
    {
        
        displayTime.text ="remaining time: "+stop+" seconds";
        algoSwitch = FalseAstarTrueJPS;
        Invoke("GenerateAgents", 0.2f);
        timer = 0.1f / numAgent; 
    }

    // Update is called once per frame
    void Update()
    {
        // finishing the 120s running, show the stats of this run and stop the game.
        if (stop <= 0) {
            int sumTotalPathNum = 0;
            float sumTotalAlgoRuntime = 0;
            int sumRepathNum = 0;
            int sumAbandonedNum = 0;
            for (int i = 0; i < numAgent; i++) {
                GameObject agent = generatedAgents[walkAgentCounter];
                AgentController controller = agent.GetComponent<AgentController>();
                sumTotalPathNum += controller.totalPathNum;
                sumTotalAlgoRuntime += controller.totalAlgoRunTime;
                sumRepathNum += controller.repathNum;
                sumAbandonedNum += controller.abandonedNum;
                
            }
            
            displayResult.text = "Average algorithm runtime " + (float)(sumTotalAlgoRuntime / sumTotalPathNum)
                + "\n" + "total pathing num " +sumTotalPathNum
                + "\n" + "total repathing num " + sumRepathNum
                + "\n" + "total abandoned " + sumAbandonedNum;
                
            Time.timeScale = 0;
            //operations for display result here
            //Debug.Log("Finished");
        }
        if (generatedAgents.Count != 0)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            // time to walk an agent!
            if (timer < 0)
            {
                WalkAgentByNum();
                walkAgentCounter++;
                // reset the counter
                if (walkAgentCounter == generatedAgents.Count)
                {
                    walkAgentCounter = 0;
                }
                // reset timer
                timer = 0.1f/numAgent;
            }
            // displaying remaining time
            stop -= Time.deltaTime;
            displayTime.text = "remaining time: " + stop + " seconds";
        }
    }
    // check whether the next move of an agent will collide with another agent
    public static bool CheckAgentCollision(Vector3 coord) {
        foreach(GameObject agent in generatedAgents) {
            if (coord == agent.transform.position) {
                return true;
            }
        }
        return false;
    }
    // walk one agent
    // order is determined by the order of spawning
    void WalkAgentByNum()
    {
        GameObject agent = generatedAgents[walkAgentCounter];
        AgentController controller = agent.GetComponent<AgentController>();
        // the next planned step
        Vector3 nextStep = controller.PeekNextWalk();
        Debug.Log("next step"+nextStep);
        // if an agent will collide with another agent in the next move, replan the path.
        if (CheckAgentCollision(nextStep)) {
            controller.toAvoid = new Vector3(nextStep.x, nextStep.y, nextStep.z);
            // reset starting point
            controller.source = agent.transform.position;
            // this switch will deciding whether to use A* or JPS
            if (!FalseAstarTrueJPS)
            {
                controller.GenerateAStarPath();
            }
            else
            {
                controller.GenerateJPSPath();
            }
            // accumulate repathing
            controller.repathNum++;
        }

        // walk for 1 step
        controller.walk();
        // if reached destination, reset source as current location and 
        // start finding a path to a new destination.
        if (agent.transform.position == controller.destination && nextStep==new Vector3(-1,-1,-1))
        {
            // reset source
            controller.source = agent.transform.position;
            // have a new destination
            controller.destination = controller.GenerateDestination();
            if (!FalseAstarTrueJPS)
            {
                controller.GenerateAStarPath();
            }
            else
            {
                controller.GenerateJPSPath();
            }
            
        }
    }

    // check whether this coord overlaps with obstacles
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
    // check whether this coord is occupied by other agents
    bool AgentCheck(Vector3 toCheck)
    {
        if (agentCoords.Count != 0)
        {
            foreach (Vector3 coord in agentCoords)
            {
                if (toCheck == coord)
                {
                    return false;
                }
            }
        }
        return true;
    }
    // check whether this coord is inside an teleport waiting area
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
    // randomly get a available spawning coord for agents
    Vector3 GetAvailableSpawnCoord()
    {
        Vector3 coord;
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
            if (ObstacleCheck(coord) && AgentCheck(coord) && TeleportAreaCheck(coord))
            {
                break;
            }

        }
        return coord;
    }
    // generate numAgent agents in approporiate positions
    // avoiding obstacles, bridges, teleport area and other agents
    void GenerateAgents()
    {
        obstacleCoords = new List<Vector3>(ObstacleGenerator.occupiedCoords);
        for (int i = 0; i < numAgent; i++)
        {
            Vector3 spawnCoord = GetAvailableSpawnCoord();
            GameObject tmpAgent = Instantiate(agent, spawnCoord, agent.transform.rotation);
            AgentController controller = tmpAgent.GetComponent<AgentController>();
            controller.source = spawnCoord;
            tmpAgent.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            generatedAgents.Add(tmpAgent);
        }
    }
}
