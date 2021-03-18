using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    #region Fields

    public List<Vector3> pathTotarget;
    private List<Node> ChekedNodes = new List<Node>();
    private List<Node> WaitingNodes = new List<Node>();

    public GameObject Target;
    public GameObject pointPrefab;
    private GameObject[,] points;

    public int dimension;

    private LineRenderer line;

    #endregion



    #region Methods

    private void Start()
    {
        points = new GameObject[dimension, dimension];
        Generator();
        Target = points[dimension - 1, dimension - 1];
        pathTotarget = GetPath(Target.transform.position);
        CreateCurve();
    }

    private void CreateCurve()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.green;
        line.endColor = Color.red;
        line.widthMultiplier = 0.1f;
        line.positionCount = points.Length;
        for(int i = 0; i < pathTotarget.Count; i++)
        {
            line.SetPosition(i, pathTotarget[i]);
        }
    }

    public List<Vector3> GetPath(Vector3 target)
    {
        pathTotarget = new List<Vector3>();

        var startPostion = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z));
        var targetPostion = new Vector3(Mathf.Round(Target.transform.position.x), 0, Mathf.Round(Target.transform.position.z));

        if(startPostion == targetPostion)
        {
            return pathTotarget;
        }

        Node startNode = new Node(0, Random.Range(0,10), startPostion, targetPostion, null);
        ChekedNodes.Add(startNode);
        WaitingNodes.AddRange(GetNeighboringNodes(startNode));

        while (WaitingNodes.Count > 0)
        {
            Node nodeToCheck = WaitingNodes.Where(x => x.F == WaitingNodes.Min(y => y.F)).FirstOrDefault();

            if (nodeToCheck.currentPosition == targetPostion)
            {
                return CalculatePathFromNode(nodeToCheck);
            }

            WaitingNodes.Remove(nodeToCheck);

            if (!ChekedNodes.Where(x => x.currentPosition == nodeToCheck.currentPosition).Any())
            {
                ChekedNodes.Add(nodeToCheck);
                WaitingNodes.AddRange(GetNeighboringNodes(nodeToCheck));
            }
        }

        return pathTotarget;
    }

    //private void OnDrawGizmos()
    //{
    //    foreach(var item in ChekedNodes)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawSphere(new Vector3(item.currentPosition.x, 1f, item.currentPosition.z), 0.2f);
    //    }
    //}

    public List<Vector3> CalculatePathFromNode(Node node)
    {
        var path = new List<Vector3>();
        Node currentNode = node;

        while(currentNode.previousNode != null)
        {
            path.Add(new Vector3(currentNode.currentPosition.x, 0, currentNode.currentPosition.z));
            currentNode = currentNode.previousNode;
        }

        return path;
    }

    private List<Node> GetNeighboringNodes(Node node)
    {
        var neighboringNodes = new List<Node>();

        neighboringNodes.Add(new Node(node.G + 1, Random.Range(0, 10), new Vector3(node.currentPosition.x + 1, 0, node.currentPosition.z), node.targetPosition, node));
        neighboringNodes.Add(new Node(node.G + 1, Random.Range(0, 10), new Vector3(node.currentPosition.x - 1, 0, node.currentPosition.z), node.targetPosition, node));
        neighboringNodes.Add(new Node(node.G + 1, Random.Range(0, 10), new Vector3(node.currentPosition.x, 0, node.currentPosition.z + 1), node.targetPosition, node));
        neighboringNodes.Add(new Node(node.G + 1, Random.Range(0, 10), new Vector3(node.currentPosition.x, 0, node.currentPosition.z - 1), node.targetPosition, node));

        return neighboringNodes;
    }

    private void Generator()
    {
        for(int i = 0; i < dimension; i++)
        {
            for(int j = 0; j < dimension; j++)
            {
                points[i, j] = Instantiate(pointPrefab, new Vector3(i, 0, j), Quaternion.identity) as GameObject;

                //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); 
                //sphere.transform.position = new Vector3(i, 0, j);
            }
        }
    }

    #endregion
}
