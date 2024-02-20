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

public class Presenter : MonoBehaviour
{
    [SerializeField] private UrgController urgController;
    [SerializeField] private View view;

    void Awake()
    {
        view.GetVector3Handler += getPeakPos;
        view.SetAverageTh += setAverageTh;
    }

    void Update()
    {
    }
    private Vector3 getPeakPos(){
        Vector3 peakPos = urgController.GetPeakPos();
        Vector3 customPos = new Vector3(peakPos.x, 0, -urgController.AverageTh);
        return customPos;
    }
    private void setAverageTh(float _val){
        urgController.AverageTh = _val;
    }

}
