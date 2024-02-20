using System.Collections;
using System.Collections.Generic;
using Common;
using UniRx;
using UnityEngine;

public class Model : MonoBehaviour
{

    public float Average = 0.0f;

    // 範囲検知に用いるid配列
    [SerializeField] const int EDGENUM = 8;
    [SerializeField] private int[] _edgeIdList;
    [SerializeField] private PeakParam peakParam;
    public int MaxId = 0;
    // [SerializeField] private IReactiveCollection<int> EdgeList => _edgeList;
    // private ReactiveCollection<int> _edgeList = new ReactiveCollection<int>();

    // Start is called before the first frame update
    void Awake()
    {
        _edgeIdList = new int[Constants.EDGENUM] { 0, 27, 62, 87, 116, 140, 0, 0 };
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
    // センサー解析毎に更新される
    public void SetPeakParam(PeakParam _param){
        peakParam = _param;
    }
    public void SetEdgeList(int _id){
        _edgeIdList[_id] = peakParam.ID;
        Debug.Log($"EdgeList Set → {string.Join(", ", _edgeIdList)}");
    }
    public int GetEdgePosbyId(int _id){
        Debug.Log($"GetEdgePosbyId {_id} → {_edgeIdList[_id]}");
        return _edgeIdList[_id];
    }
}
