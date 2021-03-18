using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    #region Fields

    public int X;
    public int Z;

    #endregion



    #region Ctor

    public Point(int x, int z)
    {
        X = x;
        Z = z;
    }

    #endregion
}
