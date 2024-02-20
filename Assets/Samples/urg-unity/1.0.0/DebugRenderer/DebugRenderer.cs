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

namespace Urg
{
    public class DebugRenderer : MonoBehaviour
    {
        public UrgSensor urg;
        LineRenderer linerend;
        // [SerializeField] public LineRenderer linerend2;

        [SerializeField] private float AverageTh;
        [SerializeField] private GameObject AverageThOb;
        [SerializeField] private TMP_InputField AverageThInput;
        [SerializeField] private TextMeshProUGUI AverageUI;
        [SerializeField] private TextMeshProUGUI PeakParam;
        [SerializeField] TMP_Dropdown ButtonDropDown;
        [SerializeField] UnityEngine.UI.Button SetButton;
        [SerializeField] private List<GameObject> EdgeList;
        private int selectedEdgeId = 0;
        private int minId;
        private float peakDistance;
        private int peakId;
        // [SerializeField] List<string> ButtonStrList;
        // デフォルトでは2
        [SerializeField] private int detectTh = 3;
        [SerializeField] private int detectNum = 10;
        // private [SerializeField] int detectTh;
        private float[] rawDistances;
        private List<DetectedLocation> locations = new List<DetectedLocation>();
        private List<List<int>> clusterIndices;
        private AffineConverter affineConverter;
        private List<GameObject> debugObjects;
        private Object syncLock = new Object();
        private System.Diagnostics.Stopwatch stopwatch;
        // EuclidianClusterExtraction cluster;

        //一定間隔でセンサーのrebootを行う    
        // private float _repeatSpan = 60*3;      //繰り返す間隔
        private float _repeatSpan = 60*5;      //繰り返す間隔
        private float _timeElapsed;     //経過時間

        public List<GameObject> testObjList;
        // 時系列処理用フィルター
        private Filter1d filter = new Filter1d();
        private const int WINDOWSIZE = 5;
        private const int LENGTH = 2161;
        private const float DISTANCEAVE = 2.30f;

        void Awake()
        {
            linerend = gameObject.AddComponent<LineRenderer>();
            // linerend.SetWidth(0.03f, 0.03f);
            linerend.SetWidth(0.003f, 0.003f);
            // linerend2 = gameObject.AddComponent<LineRenderer>();
            // linerend2.SetWidth(0.01f, 0.01f);
            Camera mainCamera = GetComponent<Camera>();
            // camera
            Application.targetFrameRate = 30;
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();


            // delegate method to be triggered when the new data is received from sensor.
            urg.OnDistanceReceived += Urg_OnDistanceReceived;

            // uncomment if you need some filters before clustering
            urg.AddFilter(new TemporalMedianFilter(3));
            urg.AddFilter(new SpatialMedianFilter(3));
            urg.AddFilter(new DistanceFilter(2.25f));
            // urg.SetClusterExtraction(new EuclidianClusterExtraction(0.1f));
            // cluster = new EuclidianClusterExtraction(0.1f);

            var cam = Camera.main;
            var plane = new Plane(Vector3.up, Vector3.zero);

            var sensorCorners = new Vector2[4];
            sensorCorners[0] = new Vector2(1.5f, 1f);
            sensorCorners[1] = new Vector2(1.5f, -1f);
            sensorCorners[2] = new Vector2(0.2f, -1f);
            sensorCorners[3] = new Vector2(0.2f, 1f);

            var worldCorners = new Vector3[4];
            worldCorners[0] = Screen2WorldPosition(new Vector2(0, Screen.height), cam, plane);
            worldCorners[1] = Screen2WorldPosition(new Vector2(Screen.width, Screen.height), cam, plane);
            worldCorners[2] = Screen2WorldPosition(new Vector2(Screen.width, 0), cam, plane);
            worldCorners[3] = Screen2WorldPosition(new Vector2(0, 0), cam, plane);
            affineConverter = new AffineConverter(sensorCorners, worldCorners);

            debugObjects = new List<GameObject>();
            // for (var i = 0; i < detectNum; i++)
            // {
            //     var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //     obj.transform.parent = transform;
            //     obj.transform.localScale = 0.3f * Vector3.one;
            //     debugObjects.Add(obj);
            // }
            // StartCoroutine(RepeatFunc());

            initGui();
            
        }

        void Update()
        {
            if (urg == null)
            {
                return;
            }

            if (rawDistances != null && rawDistances.Length > 0)
            {
                List<Vector3> positions = new List<Vector3>();
                List<Vector3> positionsEdge = new List<Vector3>();
                List<float> distanceList = new List<float>();
                // List<float> clippedData = new List<float>();
                float[] clippedData = new float[LENGTH];
                List<int> maxIndexList = new List<int>();
                float[] gradList = new float[LENGTH];
                
                //頂点データ確認用string
                string peakParam = "";
                
                //中心から10度、－10度の検知角度になるように調整
                minId = rawDistances.Length/2 - rawDistances.Length/27;
                int maxId = rawDistances.Length/2 + rawDistances.Length/27;
                int clippedLength = maxId - minId;
                // クリッピングしたデータ
                clippedData = filter.clippingData(rawDistances, minId, maxId);
                // 移動平均
                filter.avarageFilter(ref clippedData, clippedData.Length, 6);
                float distanceAverage = filter.getAverage(clippedData, clippedData.Length);
                Debug.Log($"Average:{distanceAverage}");
                // gradList = filter.gradFilter(clippedData, clippedData.Length, 14);
                // maxIndexList = filter.getMaxList(clippedData, gradList, clippedData.Length, 14, distanceAverage - distanceAverage/6);
                Debug.Log($"Length:{clippedData.Length}");
                positions.Add(urg.transform.position);
                peakId = filter.getMaxIndex(clippedData, clippedData.Length, AverageTh);
                peakParam += peakId.ToString() + ",";
                peakParam += clippedData[peakId].ToString("f2") + ",";
                for (int i = 0; i < clippedLength; i++)
                {
                    float distance = clippedData[i];
                    Vector3 pos = convertPos(distance, i);
    #if UNITY_EDITOR
                    Debug.DrawRay(urg.transform.position, pos, Color.blue);
    #endif              
                    if (peakId != 0 && i == peakId)
                    {
                        peakDistance = distance;
                        positions.Add(new Vector3(pos.x, pos.y, urg.transform.position.z));
                        peakParam += "{" + pos.x.ToString("f2") + "," + pos.y.ToString("f2") + "," + pos.z.ToString("f2") +  "}";
                    }else{
                            positions.Add((Vector3)pos);
                    }
                }
                positions.Add(urg.transform.position);
                // for (int i = 0; i < maxIndexList.Count; i++)
                // {
                //     float distance = clippedData[maxIndexList[i]];
                //     float angle = urg.StepAngleRadians * i + urg.OffsetRadians;
                //     var cos = Mathf.Cos(angle);
                //     var sin = Mathf.Sin(angle);
                //     var dir = new Vector3(cos, 0, sin);
                //     var pos = distance * dir;
                //     // positions.Add(new Vector3(0, 0, gradList[i]));
                //     positions.Add((Vector3)pos);
                // }
                linerend.positionCount = positions.Count;
                // 線を引く場所を指定する
                linerend.SetPositions(positions.ToArray());

                AverageUI.text = distanceAverage.ToString("f6");
                PeakParam.text = peakParam;

            }

            if (locations == null)
            {
                return;
            }
            //----------------------クラスターdetect----------------------
            // clusterIndices = cluster.ExtractClusters(locations);

            // var locs = this.locations;
            // int index = 0;
            // for (var i = 0; i < clusterIndices.Count; i++)
            // {
            //     if (clusterIndices[i].Count < detectTh)
            //     {
            //         continue;
            //     }
            //     //Debug.Log(clusterIndices[i].Count);
            //     Vector2 center = Vector2.zero;
            //     foreach (var j in clusterIndices[i])
            //     {
            //         center += locations[j].ToPosition2D();
            //     }
            //     center /= (float)clusterIndices[i].Count;

            //     Vector3 worldPos = new Vector3(0, 0, 0);
            //     var inRegion = affineConverter.Sensor2WorldPosition(center, out worldPos);
            //     if (inRegion && index < debugObjects.Count)
            //     {
            //         //Gizmos.DrawCube(worldPos, new Vector3(0.1f, 0.1f, 0.1f));
            //         debugObjects[index].transform.position = worldPos;
            //         index++;
            //     }
            // }

            // for (var i = index; i < debugObjects.Count; i++)
            // {
            //     debugObjects[i].transform.position = new Vector3(100, 100, 100);
            // }
            //----------------------クラスターdetect----------------------
        }
        public void updateThSlider(string _th){
            bool success = float.TryParse(_th, out AverageTh);
            if (success)
            {
                Debug.Log($"Converted '{_th}' to {AverageTh}.");
            }
            else
            {
                Debug.LogWarning($"Attempted conversion of '{_th ?? "<null>"}' failed.");
            }
            AverageThOb.transform.position = new Vector3(0,0,-AverageTh);
        }
        private void updateDropdown(int _selectId){
            selectedEdgeId = _selectId;
        }
        private void updateBtn(){
            updateUI();
        }
        private void initGui(){
            AverageThInput.onValueChanged.AddListener(updateThSlider);
            ButtonDropDown.onValueChanged.AddListener(updateDropdown);
            SetButton.onClick.AddListener(updateBtn);
        }
        private void updateUI(){
            Vector3 peakPos = convertPos(peakDistance, peakId);
            Vector3 customPos = new Vector3(peakPos.x, 0, -AverageTh);
            // Vector3 _targetPos = EdgeList[selectedEdgeId].transform.position;
            EdgeList[selectedEdgeId].transform.position = customPos;
        }

        private Vector3 convertPos(float _distance, int id){
            float angle = urg.StepAngleRadians * (minId + id) + urg.OffsetRadians;
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);
            var dir = new Vector3(cos, 0, sin);
            var pos = _distance * dir;
            return pos;
        }

        void Urg_OnDistanceReceived(DistanceRecord data)
        {
            // Debug.LogFormat("distance received: SCIP timestamp={0} unity timer={1}", data.Timestamp, stopwatch.ElapsedMilliseconds);
            // Debug.LogFormat("cluster count: {0}", data.ClusteredIndices.Count);
            this.rawDistances = data.RawDistances;
            this.locations = data.FilteredResults;
            this.clusterIndices = data.ClusteredIndices;
        }

        private void setEdgePosition(){

        }

        private static Vector3 Screen2WorldPosition(Vector2 screenPosition, Camera camera, Plane basePlane)
        {
            var ray = camera.ScreenPointToRay(screenPosition);
            var distance = 0f;

            if (basePlane.Raycast(ray, out distance))
            {
                var p = ray.GetPoint(distance);
                return p;
            }
            return Vector3.negativeInfinity;
        }

        private IEnumerator RepeatFunc(){
            while (true)
            {
                _timeElapsed += Time.deltaTime;
                if (_timeElapsed >= _repeatSpan)
                {
                    _timeElapsed = 0;
                    urg.RebootScanning();
                    urg.ForceClose();
                    urg.Restart();
                    // Debug.Log($"Reboot! : ");
                }
                yield return null;
            }
        }
    }
}
