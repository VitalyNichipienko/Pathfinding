using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    #region Fields

    public Node previousNode;

    #endregion



    #region Properties

    public HashSet<Node> Neighbours { get; private set; }


    public GameObject Position { get; private set; }


    public int Value { get; private set; } // Heuristic multiplier


    public float G { get; private set; }   // Distance from start to node


    public float H { get; private set; }   // Distance from node to target


    public float Weight { get; private set; }   // F = G + H


    public virtual bool IsLocked => false;

    #endregion



    #region Ctor

    public Node(int value, GameObject position)
    {
        Neighbours = new HashSet<Node>();

        Value = value;
        Position = position;
    }

    #endregion



    #region Methods

    public void SetNodeWeight(float g, float h)
    {
        G = g;
        H = h;

        Weight = G + H;
    }

    #endregion
}