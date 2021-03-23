using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultCurve : MonoBehaviour
{
    #region Fields

    private LineRenderer _line;

    #endregion



    #region Methods

    public void DrawResultCurve(List<Node> path)
    {
        _line = gameObject.AddComponent<LineRenderer>();
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.startColor = Color.red;
        _line.endColor = Color.green;
        _line.widthMultiplier = 0.2f;
        _line.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            _line.SetPosition(i, path[i].Position.transform.position);
        }
    }

    #endregion
}
