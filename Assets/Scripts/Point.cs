using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{ 
    #region Fields

    public int cell;

    public GameObject pointObject;

    #endregion



    #region Ctor

    public Point(int value, GameObject obj)
    {
        cell = value;
        pointObject = obj;
    }

    #endregion
}
