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
    public const int LOW = 0;
    public const int HIGH = 1;
}
public struct ActiveBtnState{
    public bool isFine;
    public int btnId;
    public ActiveBtnState(bool _isFine, int _btnId){
        isFine = _isFine;
        btnId = _btnId;
    }

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