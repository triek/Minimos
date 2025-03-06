using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();

        foreach(Node n in FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            n.gScore = float.MaxValue;
        }

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while(openSet.Count > 0)
        {
            int lowestF = default;

            for(int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fScore() < openSet[lowestF].fScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if(currentNode == end)
            {
                List<Node> path = new List<Node>();

                path.Insert(0, end);

                while(currentNode != start)
                {
                    currentNode = currentNode.camefrom;
                    path.Add(currentNode);
                }

                path.Reverse();
                return path;
            }

            foreach(Node connectedNode in currentNode.connections)
            {
                if (connectedNode == null)
                {
                    //Debug.LogError("Connected node is null.");
                    continue;
                }

                float heldGscore = currentNode.gScore + Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);

                if(heldGscore < connectedNode.gScore)
                {
                    connectedNode.camefrom = currentNode;
                    connectedNode.gScore = heldGscore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);

                    if(!openSet.Contains(connectedNode))
                    {
                         openSet.Add(connectedNode);
                    }
                }
            }
        }
        return null;
    }

    public Node FindNearestNode(Vector2 pos)
    {
        Node foundNode = null;
        float minDistance = float.MaxValue;

        foreach (Node node in FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            float currentDistance = Vector2.Distance(pos, node.transform.position);

            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                foundNode = node;
            }
        }

        return foundNode;
    }

    public Node FindFurthestNode(Vector2 pos)
    {
        Node foundNode = null;
        float maxDistance = default;

        foreach (Node node in FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            float currentDistance = Vector2.Distance(pos, node.transform.position);
            if (currentDistance > maxDistance)
            {
                maxDistance = currentDistance;
                foundNode = node;
            }
        }

        return foundNode;
    }

    public Node[] AllNodes()
    {
        return FindObjectsByType<Node>(FindObjectsSortMode.None);
    }

}
