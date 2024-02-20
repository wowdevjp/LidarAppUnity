using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Common
{
// 定数、共通する数値
public static class Constants
{
    public const int BTNNUM = 4;
    public const int EDGENUM = 8;
}
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