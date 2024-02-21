using UnityEngine;
using OscJack;
using Common;
using System.Collections.Generic;
using Common;
using System.Linq;

public class OscJack_Client : MonoBehaviour
{
    [SerializeField] private string ipAddress = "127.0.0.1";
	[SerializeField] private int Port = 7001;
    [SerializeField] private OscClient client;
	[SerializeField] private string[] addrList = new string[Constants.BTNNUM];
	// [SerializeField] private List<string> addrList = new List<string>();
	// [SerializeField] private List<string> addrList = new List<string>(Constants.BTNNUM);
    private string _addrStr = "/Btn";
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
