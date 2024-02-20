using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Filter.Filter1d;
using Filter;
using UnityEngine.UI;
using TMPro;
// using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
// using UnityEditorInternal;
using Urg;
using TMPro.EditorUtilities;
using UniRx.Triggers;
using UniRx;
using System.Data.Common;

public class Presenter : MonoBehaviour
{
    [SerializeField] private UrgController urgController;
    [SerializeField] private View view;
    [SerializeField] private Model model;

    void Awake()
    {
        // //modelをインスタンス化
        // model = new Model();
        //----- for view update -----
        view.GetVector3Handler += getPeakPos;
        view.SetAverageTh += setAverageTh;
        //選択したＤｒｏｐｄｏｗｎ　のＩＤを渡す
        view.SetEdgeId += model.SetEdgeList;
        view.GetInitEdgePos += getEdgePosbyId;
        //----- for view update -----
        //----- for urgController subscribe  -----
        // 測域センサの特定の値の変化をUniRxでサブスクライブし、ViewやModelに反映
        urgController.OnAverageChanged.Subscribe(_val =>
        {
            view.UpdateAverageText(_val);
        }
        ).AddTo(view);
        urgController.OnPeakParamChange.Subscribe(_peakParam =>
        {
            model.SetPeakParam(_peakParam);
            string _str = "";
            _str = _peakParam.ID.ToString() + ",";
            _str += _peakParam.Distance.ToString("f3") + ",";
            _str += "{" + _peakParam.Pos.x.ToString("f2") + "," + _peakParam.Pos.y.ToString("f2") + "," + _peakParam.Pos.z.ToString("f2") +  "}" + ",";
            view.UpdatePeakText(_str);
        }).AddTo(view);
        urgController.OnStartSensorFlag.Subscribe(_flag =>
        {
            model.MaxId = urgController.clippedLength;
            if (model.MaxId > 0)
            {
                view.InitEdgeObj();
                // 端のIDを保存
                Debug.Log($"urgController.OnStartSensorFlag.Subscribe model.MaxId:{model.MaxId}");
            }
        }).AddTo(view);
        //----- for urgController subscribe  -----
    }

    void Update()
    {
    }
    //----- for view update -----
    //viewにピークのポジションを計算して渡す
    private Vector3 getPeakPos(){
        Vector3 peakPos = urgController.GetPeakPos();
        Vector3 customPos = new Vector3(peakPos.x, 0, -urgController.AverageTh);
        return customPos;
    }
    // ViewのInputフィールドで取得した閾値を反映する
    private void setAverageTh(float _val){
        urgController.AverageTh = _val;
    }
    //Edgeの初期位置を返す

    private Vector3 getEdgePosbyId(int _id){
        Vector3 customPos = new Vector3(urgController.GetPosbyId(model.GetEdgePosbyId(_id)).x, 0, -1);
        return customPos;
    }
    //----- for view update -----
    

}
