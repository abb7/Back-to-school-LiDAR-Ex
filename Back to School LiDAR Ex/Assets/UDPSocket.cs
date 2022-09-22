using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using System.IO;

[System.Serializable]
public class LIDAR
{
    public int numPoints;
    public List<Point> points;

}

[System.Serializable]
public class Point
{
    public float tx;
    public float ty;
}



public class UDPSocket : MonoBehaviour
{
    [HideInInspector] public bool isTxStarted = false;

    [SerializeField] public string IP = "127.0.0.1"; // local host
    [SerializeField] int rxPort = 8000; // port to receive data from Python on
    [SerializeField] public int txPort = 8001; // port to send data to Python on

    int i = 0; // DELETE THIS: Added to show sending data from Unity to Python via UDP

    // Create necessary UdpClient objects
    UdpClient client;
    IPEndPoint remoteEndPoint;
    Thread receiveThread; // Receiving Thread

    [SerializeField] TMP_InputField newIp;
    [SerializeField] TMP_InputField newTxPort;
    [SerializeField] TMP_InputField newRxPort;


    public VirtualClicks vc;



    void Awake()
    {
        // Create remote endpoint (to Matlab) 
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), txPort);

        // Create local client
        client = new UdpClient(rxPort);

        // local endpoint define (where messages are received)
        // Create a new thread for reception of incoming messages
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // Initialize (seen in comments window)
        print("UDP Comms Initialised");

        //StartCoroutine(SendDataCoroutine()); // DELETE THIS: Added to show sending data from Unity to Python via UDP

    }




    public void OnIpResetup()
    {

        if (newIp.text == null && newTxPort.text == null && newRxPort.text == null)
        {
            print("Ip or port have not been defined");
            return;
        }

        OnDisable();

        IP = newIp.text;
        int.TryParse(newRxPort.text, out rxPort);
        int.TryParse(newTxPort.text, out txPort);



        // Create remote endpoint (to Matlab) 
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), txPort);

        // Create local client
        client = new UdpClient(rxPort);

        // local endpoint define (where messages are received)
        // Create a new thread for reception of incoming messages
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // Initialize (seen in comments window)
        print("UDP Comms Initialised");

        //StartCoroutine(SendDataCoroutine());

    }

    // Receive data, update packets received
    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                //print(">> " + text);
                string lidar1 = JsonHelper.GetJsonObject((text), "LIDAR1");
                LIDAR p = JsonUtility.FromJson<LIDAR>(lidar1);
                //print("?? " + p.points[0].tx);
                //Virtual touch in p.point[0] tx and ty
                UnityMainThread.wkr.AddJob(() =>
                {
                    vc.ClickAt((p.points[0].tx * 1900*2), (p.points[0].ty * 1200*2));
                });
                //ProcessInput(text);
                //DecideAction(text);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
    /*
    public void DecideAction(string text)
    {
        if (text.Contains("deactive"))
        {
            string[] splitArray = text.Split(char.Parse("/"));
            string panelId = splitArray[1];
            int.TryParse(panelId, out int panelIdint);
            //deativate the targeted panel
            mediaPanalManager.DeactivatePanel(panelIdint);
        }
        else if (text.Contains("active"))
        {
            string[] splitArray = text.Split(char.Parse("/"));
            string panelId = splitArray[1];
            string mediaId = splitArray[2];

            int.TryParse(panelId, out int panelIdint);
            int.TryParse(mediaId, out int mediaIdint);

            //panelIdint -= 1;

            mediaPanalManager.RequestPlayMediaIDinPanelID(panelIdint, mediaIdint);
        }
    }

    private void ProcessInput(string input)
    {
        // PROCESS INPUT RECEIVED STRING HERE

        if (!isTxStarted) // First data arrived so tx started
        {
            isTxStarted = true;
        }
    }
*/
    //Prevent crashes - close clients and threads properly!
    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        client.Close();
    }
    
}


