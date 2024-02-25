using UnityEngine;
using OscJack;
using Common;
using System.Collections.Generic;
using Common;
using System.Linq;

public class OscJack_Client : MonoBehaviour
{
    [SerializeField] private string ipAddress = "192.168.1.35";
	[SerializeField] private int Port = 8006;
    [SerializeField] private OscClient client;
	[SerializeField] private string[] addrList = new string[Constants.BTNNUM];
	// [SerializeField] private List<string> addrList = new List<string>();
	// [SerializeField] private List<string> addrList = new List<string>(Constants.BTNNUM);
    //0123を送信する
    private string _addrStr = "/zone3/hobbering";
    private bool isActive = false;
    void Awake()
    {
        for (int i = 0; i < Constants.BTNNUM; i++)
        {
            Debug.Log($"addrList[{i}] = {addrList}");
            addrList[i] = _addrStr + (i+1).ToString();
            // addrList.Append( _addrStr + i.ToString());
        }
    }
    void OnEnable()
    {
        client = new OscClient(ipAddress, Port);
        isActive = true;
    }

    void OnDisable()
    {
        client.Dispose();
    }

    public void Send(int[] _btnList)
    {
        
        if (!isActive)
        {
            return;
        }
        for (int i = 0; i < Constants.BTNNUM; i++)
        {
            Debug.LogWarning($"addr:{addrList[i]}, state:{_btnList[i]}");
            client.Send(addrList[i], _btnList[i]);
            // client.Send(addrList[i], 777);
        }
    }
}
