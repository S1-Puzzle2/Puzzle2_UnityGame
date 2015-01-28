using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System;
using Vbaccelerator.Components.Algorithms;
using System.IO;

public class NetworkController : MonoBehaviour {

	public string ip;
    public int port;
    public int timeOutSec;

    public string broadcastIP;
    public int broadcastPort;

    public GameControllerScript gameController;
    public Dictionary<System.Timers.Timer, AbstractTransferObject> timeOutTimers;

	private ConnectionManager connManager;
    private LinkedList<int> allreadyReceivedSeqID;
	private bool connected;

    private string log;

	// Use this for initialization
	void Start () {
		connected = false;
		connManager = new ConnectionManager();

        timeOutTimers = new Dictionary<System.Timers.Timer, AbstractTransferObject>();

        Thread thread = new Thread(broadCastTest);
		thread.Start();
        allreadyReceivedSeqID = new LinkedList<int>();


	}

    private void broadCastTest()
    {
        broadcastOpen();
        broadcastReceive();
        broadcastSend();
    }

    void OnApplicationQuit()
    {
        closeConn();
    }

    public void broadcastOpen()
    {
        connManager.open(ClientType.Unity, ConnectionType.Broadcast, new WLANConnectionDef(broadcastIP, broadcastPort), new ConnectionDelegates.ConnectedHandler(connectedCallback));
    }

    public void broadcastSend()
    {
        SimpleParameterTransferObject spto = new SimpleParameterTransferObject(Command.AreYouThere, null, null);
        Debug.Log(spto.toJson());
        connManager.send(spto, new ConnectionDelegates.SentHandler(sentCallback));
    }

    public void broadcastReceive()
    {
        connManager.receive(new ConnectionDelegates.ReceivedHandler(receiveCallback));
    }

    public void openConn()
    {
        Thread openConnThread = new Thread(openConnHelper);
        openConnThread.Start();
	}

    private void openConnHelper()
    {
        connManager.open(ClientType.Unity, ConnectionType.WLAN, new WLANConnectionDef(ip, port), new ConnectionDelegates.ConnectedHandler(connectedCallback));
        connManager.receive(new ConnectionDelegates.ReceivedHandler(receiveCallback));
    }

    public void sendConn(AbstractTransferObject obj)
    {
        Debug.Log("Sent command: " + obj.msgType);
		connManager.send(obj, new ConnectionDelegates.SentHandler(sentCallback));
        System.Timers.Timer timer = new System.Timers.Timer(timeOutSec * 1000);
        timer.Elapsed += timer_Elapsed;
        timer.Start();
        timeOutTimers.Add(timer, obj);
	}

    void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        Debug.Log("Packet timed out, resending");
        System.Timers.Timer timer = (System.Timers.Timer)sender;
        AbstractTransferObject obj = timeOutTimers[timer];
        timeOutTimers.Remove(timer);
        sendConn(obj);
    }


    public void closeConn()
    {
		connManager.close();
	}
	
	private void connectedCallback() {
		connected = true;
        Debug.Log("connected");

        // send images to server
        //sendImagesToServer();

        SimpleParameterTransferObject registerPackage = new SimpleParameterTransferObject(Command.Register, null, null);
        sendConn(registerPackage);
	}
	
	private void sentCallback() {
		Debug.Log("Sent message");
	}
	
	private void receiveCallback(string response) {

        //response = response.Remove(response.Length - 1);
        Debug.Log("received: " + response);
        
        JsonData responseData = JsonMapper.ToObject(response);
        long crc = 0;
        try
        {
            crc = long.Parse(responseData["checkSum"].ToString());
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        int recSeqID = (int)responseData["seqID"];

        bool checkSumCorrect = checkCheckSum(response, crc);


        if (checkSumCorrect)
        {
            Debug.Log("Checksum correct");
            if (allreadyReceivedSeqID.Contains(recSeqID))
            {
                //connManager.receive(new ConnectionDelegates.ReceivedHandler(receiveCallback));
                return;
            }
            else
            {
                allreadyReceivedSeqID.AddFirst(recSeqID);
            }

            if (responseData.Keys.Contains("appMsg"))
            {
                Debug.Log("Received appMsg: " + Base64.Base64Decode(responseData["appMsg"].ToString()));
                gameController.updateFromNetwork(responseData);
                FlagTransferObject fto = new FlagTransferObject(true, false, recSeqID);
                sendConn(fto);
            }
            else if (responseData.Keys.Contains("flags"))
            {
                foreach (System.Timers.Timer t in timeOutTimers.Keys)
                {
                    AbstractTransferObject obj = timeOutTimers[t];
                    if (obj.seqID == recSeqID)
                    {
                        t.Stop();
                        timeOutTimers.Remove(t);
                        break;
                    }
                }

            }
        }
        else
        {
            Debug.Log("CheckSum false");
            FlagTransferObject fto = new FlagTransferObject(false, false, recSeqID);
            sendConn(fto);
        }

        //connManager.receive(new ConnectionDelegates.ReceivedHandler(receiveCallback));
        
	}

    private bool checkCheckSum(string response, long crc)
    {
        string responseWithoutChecksum = response.Remove(response.IndexOf(",\"checkSum")) + "}";
        byte[] bytes = Encoding.UTF8.GetBytes(responseWithoutChecksum);
        long crcVal = CRC32.calcCrc32(bytes);
        Debug.Log("Got Checksum: " + crc + ", Calc crc: " + crcVal);

        if (crc == crcVal)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void sendImagesToServer()
    {
        Dictionary<string, object> parameters = new Dictionary<string,object>();
        parameters.Add("gameName", "hammergood puzzle");

        SimpleParameterTransferObject createPuzzlePackage = new SimpleParameterTransferObject(Command.CreatePuzzle, null, parameters);
        //sendConn(createPuzzlePackage);

        string path = "C:\\Users\\faisstm\\Desktop\\coolQuad\\";
        string[] images = Directory.GetFiles(path, "*.jpg", SearchOption.TopDirectoryOnly);
        string[] base64Data = new string[images.Length];

        for (int i = 0; i < images.Length; i++)
        {
            byte[] imageData = File.ReadAllBytes(images[i]);
            base64Data[i] = Convert.ToBase64String(imageData);

            parameters = new Dictionary<string, object>();
            parameters.Add("partOrder", i);
            parameters.Add("gameName", "hammergood puzzle");
            parameters.Add("base64Image", base64Data[i]);
            parameters.Add("barCode", System.Guid.NewGuid().ToString());
            SimpleParameterTransferObject createPiecePackage = new SimpleParameterTransferObject(Command.CreatePuzzlePart, null, parameters);
            sendConn(createPiecePackage);
        }


    }
}
