﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;

public class EthernetTransport : MonoBehaviour, ITransport
{
    public string ipAddress = "192.168.0.10";
    public int port = 10940;
    private static readonly float CONNECT_TIMEOUT = 1.0f;
    private TcpClient tcpClient;
    private NetworkStream stream;
    private TextReader reader;


    public bool IsConnected()
    {
        return tcpClient != null && tcpClient.Connected;
    }

    public bool Open()
    {
        try
        {
            tcpClient = new TcpClient();
            tcpClient.ReceiveTimeout = 5000;
            var result = tcpClient.BeginConnect(ipAddress, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(CONNECT_TIMEOUT));

            if (!success)
            {
                return false;
            }

            // we have connected
            tcpClient.EndConnect(result);

            // tcpClient.Connect(ipAddress, port);
            stream = tcpClient.GetStream();
            reader = new StreamReader(stream);
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("tcp open error {0}", e.ToString());
            return false;
        }
        return true;
    }

    public string ReadLine()
    {
        // while(!)
        return reader.ReadLine();
    }

    public void Write(byte[] bytes)
    {
        stream.Write(bytes, 0, bytes.Length);
    }
    public void Close()
    {
        reader.InitializeLifetimeService();
        tcpClient?.Close();
        tcpClient?.Dispose();
        stream?.Close();
        stream.Dispose();
    }
}
