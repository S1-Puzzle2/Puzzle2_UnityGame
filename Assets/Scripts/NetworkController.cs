using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System;
using Vbaccelerator.Components.Algorithms;
using Pathfinding.Serialization.JsonFx;

public class NetworkController : MonoBehaviour {

	public string ip;
    public int port;
    public int timeOutSec;

    public string broadcastIP;
    public int broadcastPort;

    public GameControllerScript gameController;
    public Dictionary<System.Timers.Timer, AbstractTransferObject> timeOutTimers;

	private ConnectionManager connManager;
	private bool connected;

	// Use this for initialization
	void Start () {
		connected = false;
		connManager = new ConnectionManager();

        timeOutTimers = new Dictionary<System.Timers.Timer, AbstractTransferObject>();

        Thread thread = new Thread(broadCastTest);
		thread.Start();
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
        SimpleParameterTransferObject spto = new SimpleParameterTransferObject(Command.AreYouThere, null);
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
        SimpleParameterTransferObject registerPackage = new SimpleParameterTransferObject(Command.Register, null);
        sendConn(registerPackage);
	}
	
	private void sentCallback() {
		Debug.Log("Sent message");
	}
	
	private void receiveCallback(string response) {

        response = response.Remove(response.Length - 1);
        Debug.Log("received: " + response);

        JsonReaderSettings readerSettings = new JsonReaderSettings();
        Pathfinding.Serialization.JsonFx.JsonReader reader = new Pathfinding.Serialization.JsonFx.JsonReader(response);
        Dictionary<String, System.Object> readed = (Dictionary<String, System.Object>)reader.Deserialize();
        
        long crc = (long)readed["checkSum"];
        Debug.Log(crc);

        int recSeqID = (int)readed["seqID"];
        Debug.Log(recSeqID);

        bool checkSumCorrect = checkCheckSum(response, crc);

        JsonData responseData = JsonMapper.ToObject(response);

        if (checkSumCorrect)
        {
            Debug.Log("Checksum correct");
            if (responseData.Keys.Contains("appMsg"))
            {
                Debug.Log("Received appMsg: " + Base64.Base64Decode(responseData["appMsg"].ToString()));
                gameController.updateFromNetwork(responseData);
                FlagTransferObject fto = new FlagTransferObject(true, false, false, recSeqID);
                sendConn(fto);
            }
            else if (responseData.Keys.Contains("flags"))
            {
                Debug.Log("Ack received");
                foreach (System.Timers.Timer t in timeOutTimers.Keys)
                {
                    AbstractTransferObject obj = timeOutTimers[t];
                    if (obj.seqID == recSeqID)
                    {
                        Debug.Log("Ack from " + recSeqID + " received, stoping timer");
                        t.Stop();
                        timeOutTimers.Remove(t);
                        break;
                    }
                }

            }
        }
        else
        {
            Debug.Log("checksum not correct");
            FlagTransferObject fto = new FlagTransferObject(true, false, true, recSeqID);
            sendConn(fto);
        }

        
        connManager.receive(new ConnectionDelegates.ReceivedHandler(receiveCallback));
	}

    private bool checkCheckSum(string response, long crc)
    {
        string responseWithoutChecksum = response.Remove(response.IndexOf(",\"checkSum")) + "}";
        byte[] bytes = Encoding.UTF8.GetBytes(responseWithoutChecksum);
        long crcVal = CRC32.calcCrc32(bytes);

        if (crc == crcVal)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
