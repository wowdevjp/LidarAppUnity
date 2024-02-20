using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Urg;

public class UrgController : MonoBehaviour
{
    
    public UrgSensor urg;

    private float[] rawDistances;
    private List<DetectedLocation> locations = new List<DetectedLocation>();
    private List<List<int>> clusterIndices;
    private AffineConverter affineConverter;
    private List<GameObject> debugObjects;

    // Start is called before the first frame update
    void Awake()
    {
        // stopwatch = new System.Diagnostics.Stopwatch();
        // stopwatch.Start();


        // // delegate method to be triggered when the new data is received from sensor.
        // urg.OnDistanceReceived += Urg_OnDistanceReceived;

        // // uncomment if you need some filters before clustering
        // urg.AddFilter(new TemporalMedianFilter(3));
        // urg.AddFilter(new SpatialMedianFilter(3));
        // urg.AddFilter(new DistanceFilter(2.25f));
        // // urg.SetClusterExtraction(new EuclidianClusterExtraction(0.1f));
        // // cluster = new EuclidianClusterExtraction(0.1f);

        // var cam = Camera.main;
        // var plane = new Plane(Vector3.up, Vector3.zero);

        // var sensorCorners = new Vector2[4];
        // sensorCorners[0] = new Vector2(1.5f, 1f);
        // sensorCorners[1] = new Vector2(1.5f, -1f);
        // sensorCorners[2] = new Vector2(0.2f, -1f);
        // sensorCorners[3] = new Vector2(0.2f, 1f);

        // var worldCorners = new Vector3[4];
        // worldCorners[0] = Screen2WorldPosition(new Vector2(0, Screen.height), cam, plane);
        // worldCorners[1] = Screen2WorldPosition(new Vector2(Screen.width, Screen.height), cam, plane);
        // worldCorners[2] = Screen2WorldPosition(new Vector2(Screen.width, 0), cam, plane);
        // worldCorners[3] = Screen2WorldPosition(new Vector2(0, 0), cam, plane);
        // affineConverter = new AffineConverter(sensorCorners, worldCorners);

        // debugObjects = new List<GameObject>();
        // // for (var i = 0; i < detectNum; i++)
        // // {
        // //     var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // //     obj.transform.parent = transform;
        // //     obj.transform.localScale = 0.3f * Vector3.one;
        // //     debugObjects.Add(obj);
        // // }
        // // StartCoroutine(RepeatFunc());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
