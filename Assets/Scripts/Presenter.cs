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

public class Presenter : MonoBehaviour
{
    [SerializeField] private UrgController urgController;
    [SerializeField] private View view;
    private Model model;

    void Awake()
    {
        //modelをインスタンス化
        model = new Model();
        view.GetVector3Handler += getPeakPos;
        view.SetAverageTh += setAverageTh;
        // 測域センサの特定の値の変化をUniRxでサブスクライブし、ViewやModelに反映
        urgController.OnAverageChanged.Subscribe(_val =>
        {
            view.UpdateAverageText(_val);
        }
        ).AddTo(view);
        urgController.OnPeakParamChange.Subscribe(_peakParam =>
        {
            string _str = "";
            _str = _peakParam.ID.ToString() + ",";
            _str += _peakParam.Distance.ToString("f3") + ",";
            _str += "{" + _peakParam.Pos.x.ToString("f2") + "," + _peakParam.Pos.y.ToString("f2") + "," + _peakParam.Pos.z.ToString("f2") +  "}" + ",";
            view.UpdatePeakText(_str);
        }).AddTo(view);
    }

    void Update()
    {
    }
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
    

}
