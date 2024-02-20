using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Common
{
public struct PeakParam{
    public int ID;
    public float Distance;
    public Vector3 Pos;
    public PeakParam(int id, float distance, Vector3 pos)
    {
        ID = id;
        Distance = distance;
        Pos = pos;
    }
}
}