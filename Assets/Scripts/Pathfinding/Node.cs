using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node camefrom;
    public List<Node> connections = new List<Node>();

    public float gScore;
    public float hScore;

    void Start()
    {
        Node node = GetComponent<Node>();
        node.ConnectNearbyNodes(1.8f);
    }

    public float fScore()
    {
        return gScore + hScore;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;

    //    if (connections.Count > 0)
    //    {
    //        for (int i = 0; i < connections.Count; i++)
    //        {
    //            if (connections[i] != null)
    //            {
    //                Gizmos.DrawLine(transform.position, connections[i].transform.position);
    //            }
    //        }
    //    }
    //}

    public void ConnectNearbyNodes(float radius)
    {
        Node[] allNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        foreach (Node node in allNodes)
        {
            if (node != this && Vector3.Distance(transform.position, node.transform.position) <= radius)
            {
                if (!connections.Contains(node))
                {
                    connections.Add(node);
                }
                if (!node.connections.Contains(this))
                {
                    node.connections.Add(this);
                }
            }
        }
    }
}
