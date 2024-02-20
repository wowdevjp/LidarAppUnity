using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class View : MonoBehaviour
{
    [SerializeField] private GameObject AverageThOb;
    [SerializeField] private TMP_InputField AverageThInput;
    [SerializeField] private TextMeshProUGUI AverageUI;
    [SerializeField] private TextMeshProUGUI PeakParam;
    [SerializeField] TMP_Dropdown ButtonDropDown;
    [SerializeField] UnityEngine.UI.Button SetButton;
    [SerializeField] private List<GameObject> EdgeList;

    private int _selectedEdgeId;
    public delegate Vector3 OnGetVector3Delegate();
    public event OnGetVector3Delegate GetVector3Handler;
    public delegate void OnSetFloat(float _val);
    public event OnSetFloat SetAverageTh;

    // Start is called before the first frame update
    void Awake()
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
    private void updateDropdown(int _selectId){
        _selectedEdgeId = _selectId;
    }
    private void updateBtn(){
        updateEdgePos();
    }
    private void initGui(){
        AverageThInput.onValueChanged.AddListener(updateInputText);
        ButtonDropDown.onValueChanged.AddListener(updateDropdown);
        SetButton.onClick.AddListener(updateBtn);
    }
    private void updateEdgePos(){
        // // Vector3 _targetPos = EdgeList[_selectedEdgeId].transform.position;
        // Vector3 _pos = GetVector3Handler();
        EdgeList[_selectedEdgeId].transform.position = GetVector3Handler();
    }
}
