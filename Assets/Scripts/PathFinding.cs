using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    #region Fields

    [Range(0, 100)] public int xDimension;
    [Range(0, 100)] public int yDimension;

    public Node[,] Cells { get; private set; }

    public Point[,] points;
    public GameObject prefab;

    private HashSet<Node> openSet;
    private HashSet<Node> closedSet;

    private float heuristicMultiplayer;      

    private LineRenderer line;

    #endregion



    #region Enum

    public enum HeuristicFunction : byte
    {
        None = 0,
        ManhattanDistance = 1,
        ClassicDistance = 2
    }
    private static HeuristicFunction heuristicFunction;

    #endregion



    #region Methods

    void Start()
    {
        points = new Point[xDimension, yDimension];

        MatrixGenerator();
        RunTest();

    }


    private void MatrixGenerator()
    {
        for (int i = 0; i < xDimension; i++)
        {
            for(int j = 0; j < yDimension; j++)
            {
                GameObject Obj = Instantiate(prefab, new Vector3(i, 1, j), Quaternion.identity);
                int random = UnityEngine.Random.Range(0, 2);
                points[i, j] = new Point(random, Obj);
            }
        }
    }


    private void RunTest()
    {
        Initialize(points);

        List<Node> path = FindPath();

        if (path == null)
        {
            return;
        }

        CreateResultCurve(path);

        ShowResultCost(path);

        ShowResultPath(path);
    }


    public void Initialize(Point[,] inputPoints,
                                float _heuristicMultiplayer = 0.5f,
                                HeuristicFunction _heuristicFunction = HeuristicFunction.ClassicDistance)
    {
        heuristicMultiplayer = _heuristicMultiplayer;
        heuristicFunction = _heuristicFunction;


        int xLength = inputPoints.GetLength(0);
        int yLength = inputPoints.GetLength(1);


        Cells = new Node[xLength, yLength];

        openSet = new HashSet<Node>();
        closedSet = new HashSet<Node>();

        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                Node node = new Node(inputPoints[x, y].cell, inputPoints[x, y].pointObject);

                if (x > 0)
                {
                    CreateNeighborhood(node, Cells[x - 1, y]);
                }

                if (y > 0)
                {
                    CreateNeighborhood(node, Cells[x, y - 1]);
                }

                Cells[x, y] = node;
            }
        }

        void CreateNeighborhood(Node first, Node second)
        {
            first.Neighbours.Add(second);
            second.Neighbours.Add(first);
        }
    }


    private List<Node> FindPath()
    {
        int xLength = Cells.GetLength(0);
        int yLength = Cells.GetLength(1);


        Node start = Cells[0, 0];
        Node end = Cells[xLength - 1, yLength - 1];

        if (TryFindPath(start, end, out List<Node> path))
        {
            return path;
        }
        else
        {
            Debug.Log($"Unable to find a path from point startPoint to point endPoint.");

            return default;
        }
    }


    private bool TryFindPath(Node from, Node to, out List<Node> path)
    {
        path = default;

        from.SetNodeWeight(0, GetHeuristicPathLength(from, to));

        Node currentNode = from;

        openSet.Add(from);

        while (openSet.Count > 0)
        {
            if (currentNode.Position == to.Position)
            {
                path = GetFinalPathForNode(currentNode, true);
                return true;
            }

            CalculateNeighboursWeight(currentNode, to);

            closedSet.Add(currentNode);
            openSet.Remove(currentNode);

            currentNode = FindNodeWithMinWeight();
        }

        return false;
    }


    private static List<Node> GetFinalPathForNode(Node targetNode, bool isReversed)
    {
        List<Node> result = new List<Node>();

        Node currentNode = targetNode;

        while (currentNode != null)
        {
            result.Add(currentNode);
            currentNode = currentNode.previousNode;
        }

        if (isReversed)
        {
            result.Reverse();
        }

        return result;
    }


    private Node FindNodeWithMinWeight()
    {
        Node minNode = null;

        foreach (Node node in openSet)
        {
            if (minNode == null || node.Weight < minNode.Weight)
            {
                minNode = node;
            }
        }

        return minNode;
    }


    private void CalculateNeighboursWeight(Node current, Node target)
    {
        foreach (Node node in current.Neighbours)
        {
            if (!node.IsLocked &&
                !closedSet.Contains(node) &&
                !openSet.Contains(node))
            {
                float g = current.G + CalculateStepCost(node, current);
                float h = GetHeuristicPathLength(current, target);

                node.SetNodeWeight(g, h);

                node.previousNode = current;

                openSet.Add(node);
            }
        }
    }


    private float GetHeuristicPathLength(Node from, Node to)
    {
        float result = 0.0f;

        switch (heuristicFunction)
        {
            case HeuristicFunction.ManhattanDistance:
                result = Math.Abs(to.Position.transform.position.x - from.Position.transform.position.x) +
                         Math.Abs(to.Position.transform.position.z - from.Position.transform.position.z);
                break;

            case HeuristicFunction.ClassicDistance:
                result = (float)Math.Sqrt(Math.Pow(to.Position.transform.position.x - from.Position.transform.position.x, 2) +
                                          Math.Pow(to.Position.transform.position.z - from.Position.transform.position.z, 2));
                break;

            default:
                Debug.LogWarning($"This type ({heuristicFunction}) of heuristic function is not supported.");
                break;
        }

        return result * heuristicMultiplayer;
    }


    private int CalculateStepCost(Node from, Node to) =>
        from.Value ^ to.Value;


    private void CreateResultCurve(List<Node> path)
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.red;
        line.endColor = Color.green;
        line.widthMultiplier = 0.2f;
        line.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            line.SetPosition(i, path[i].Position.transform.position);
        }
    }


    private void ShowResultCost(List<Node> path) =>
     Debug.Log($"Cost: {path[path.Count - 1].G}");


    private void ShowResultPath(List<Node> path)
    {
        const string arrowString = " -> ";

        string result = string.Empty;

        foreach (Node node in path)
        {
            result += $"[{node.Position.transform.position.x}, {node.Position.transform.position.z}] {arrowString}";
        }

        result = result.Remove(result.Length - arrowString.Length, arrowString.Length);

        Debug.Log($"Path: {result}");
    }
    #endregion
}