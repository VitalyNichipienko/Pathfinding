using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    #region Fields

    public Vector3 currentPosition;
    public Vector3 targetPosition;

    public Node previousNode;

    public int F;  // F = G + H
    public int G;  // Distance from start to node
    public int H;  // Distance from node to target
    public int Hm; // Heuristic multiplier

    #endregion



    #region Ctor

    public Node(int g, int value, Vector3 currentPosition, Vector3 targetPosition, Node previousNode)
    {
        G = g;
        this.currentPosition = currentPosition;
        this.targetPosition = targetPosition;
        this.previousNode = previousNode;
        Hm = value;

        H = (int)(Mathf.Abs(this.targetPosition.x - this.currentPosition.x) + Mathf.Abs(this.targetPosition.z - this.currentPosition.z));
        F = G + H * Hm;
    }

    #endregion
}
