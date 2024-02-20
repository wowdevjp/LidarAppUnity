using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Common;

public class View : MonoBehaviour
{
    [SerializeField] private GameObject AverageThOb;
    [SerializeField] private TMP_InputField AverageThInput;
    [SerializeField] private TextMeshProUGUI AverageUI;
    [SerializeField] private TextMeshProUGUI PeakParam;
    [SerializeField] TMP_Dropdown ButtonDropDown;
    [SerializeField] UnityEngine.UI.Button SetButton;
    [SerializeField] private GameObject edgeBarPrefab;
    [SerializeField] private GameObject[] EdgeObjList;
    // [SerializeField] private List<GameObject> EdgeObjList;

    public delegate Vector3 OnGetVector3Delegate();
    public event OnGetVector3Delegate GetVector3Handler;
    public delegate Vector3 OnGetVector3_intDelegate(int _id);
    public event OnGetVector3_intDelegate GetInitEdgePos;
    public delegate void OnSetFloat(float _val);
    public event OnSetFloat SetAverageTh;
    public delegate void OnSetInt(int _val);
    public event OnSetInt SetEdgeId;
    private int _selectedEdgeId;

    // Start is called before the first frame update
    void Awake()
    {
    }
    void Start()
    {
        initGui();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void updateInputText(string _th){
        float _average = 0;
        bool success = float.TryParse(_th, out _average);
        if (success)
        {
            Debug.Log($"Converted '{_th}' to {_average}.");
            // presenterを介して代入
            SetAverageTh(_average);
        }
        else
        {
            Debug.LogWarning($"Attempted conversion of '{_th ?? "<null>"}' failed.");
        }
        AverageThOb.transform.position = new Vector3(0,0,-_average);
    }
    private void updateThLineObj(float _z){
        AverageThOb.transform.position = new Vector3(0,0,_z);
    }
    // dropdownが選択されたときにそのＩＤを取得
    private void updateDropdown(int _selectId){
        _selectedEdgeId = _selectId;
    }
    private void updateBtn(){
        if (_selectedEdgeId == Constants.EDGENUM)
        {
            InitEdgeObj();
        }else{
            updateEdgePos();
            SetEdgeId(_selectedEdgeId);
        }
    }
    private void initGui(){
        AverageThInput.onValueChanged.AddListener(updateInputText);
        ButtonDropDown.onValueChanged.AddListener(updateDropdown);
        SetButton.onClick.AddListener(updateBtn);
    }
    public void InitEdgeObj(){
        EdgeObjList = new GameObject[Constants.EDGENUM];
        for (int i = 0; i < Constants.EDGENUM; i++)
        {
            // EdgeObjList[i] = Instantiate(edgeBarPrefab, Vector3.zero, Quaternion.identity);
            EdgeObjList[i] = Instantiate(edgeBarPrefab,  GetInitEdgePos(i), Quaternion.identity);
        }
    }
    private void updateEdgePos(){
        // // Vector3 _targetPos = EdgeObjList[_selectedEdgeId].transform.position;
        // Vector3 _pos = GetVector3Handler();
        EdgeObjList[_selectedEdgeId].transform.position = GetVector3Handler();
    }
    // 平均を可視化
    public void UpdateAverageText(float _val){
        AverageUI.text = _val.ToString();
    }
    // ピークのパラメタを表示
    public void UpdatePeakText(string _str){
        PeakParam.text = _str;
    }
}
