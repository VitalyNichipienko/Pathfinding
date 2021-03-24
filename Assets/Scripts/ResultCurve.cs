using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultCurve : MonoBehaviour
{
    #region Fields

    private LineRenderer line;

    #endregion



    #region Methods

    public void DrawResultCurve(List<Node> path)
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

    #endregion
}
