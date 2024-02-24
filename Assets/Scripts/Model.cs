using System.Collections;
using System.Collections.Generic;
using Common;
using UniRx;
using UnityEngine;

public class Model : MonoBehaviour
{

    public float Average = 0.0f;
    private const int BTNBUF = 10;
    private int updateCnt = 0;
    private bool startCalc = false;

    // 範囲検知に用いるid配列
    [SerializeField] private int[] _edgeIdList;
    [SerializeField] private PeakParam peakParam;
    private List<List<int>> btnFlagList = new List<List<int>>();
    public int MaxId = 0;
    // [SerializeField] private IReactiveCollection<int> EdgeList => _edgeList;
    // private ReactiveCollection<int> _edgeList = new ReactiveCollection<int>();

    // Start is called before the first frame update
    void Awake()
    {
        _edgeIdList = new int[Constants.EDGENUM] { 0, 30, 69, 120, 150, 200, 210, 229 };
        for (int i = 0; i < Constants.BTNNUM; i++)
        {
            btnFlagList.Add(new List<int>(BTNBUF));
        }
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
    // センサー解析毎に更新される
    public void SetPeakParam(PeakParam _param){
        if (_param.ID <= 0)
        {
            resetBtnState();
        }else{
            peakParam = _param;
            calcPosition();
        }
    }
    public void SetEdgeList(int _id){
        _edgeIdList[_id] = peakParam.ID;
        Debug.Log($"EdgeList Set → {string.Join(", ", _edgeIdList)}");
    }
    public int GetEdgePosbyId(int _id){
        Debug.Log($"GetEdgePosbyId {_id} → {_edgeIdList[_id]}");
        return _edgeIdList[_id];
    }
    // ボタンの位置特定関数
    private void calcPosition(){
        // 指定バッファ分たまるかチェック
        if (updateCnt < BTNBUF)
        {
            updateCnt += 1;
        }else{
            startCalc = true;
        }
        // 各ボタン内にpeakIDが存在するかどうか
        for (int i = 0; i < Constants.BTNNUM; i++)
        {
            int addFlag = Constants.LOW;
            int edgeId = i*2;
            if (peakParam.ID >= _edgeIdList[edgeId] && peakParam.ID <= _edgeIdList[edgeId+1])
            {
                addFlag = Constants.HIGH;;
            }else{
                addFlag = Constants.LOW;
            }
            //一定バッファたまるかどうか
            if (startCalc)
            {
                btnFlagList[i].RemoveAt(0);
                btnFlagList[i].Insert(BTNBUF-1, addFlag);
            }else{
                btnFlagList[i].Add(addFlag);
            }
        }
    }

    private int checkBtnState(){
        int topCnt = -1;
        int btnId = -1;
        if (btnFlagList.Count>0)
        {
        for (int i = 0; i < Constants.BTNNUM; i++)
        {
            int cnt = 0;
            foreach (var flag in btnFlagList[i])
            {
                if (flag == Constants.HIGH)
                {
                    cnt++;
                }
            }
            if (cnt != 0 && cnt > topCnt)
            {
                btnId = i;
                topCnt = cnt;
            }
        }
        }
        return btnId;
    }

    public  ActiveBtnState GetBtnState(){
        ActiveBtnState _btnState;
        int btnId = checkBtnState();
        if (btnId != -1)
        {
            _btnState = new ActiveBtnState(true, btnId);
        }else{
            _btnState = new ActiveBtnState(false, btnId);
        }
        return _btnState;
    }

    private void resetBtnState(){
        updateCnt = 0;
        startCalc = false;
        if(btnFlagList.Count > 0){
        for (int i = 0; i < Constants.BTNNUM; i++)
        {
            btnFlagList[i].Clear();
        }
        }
    }

}
