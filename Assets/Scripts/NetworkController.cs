using UnityEngine;
using System.Collections;
using System.Threading;
using LitJson;
using System.Collections.Generic;
using System.Text;
using System;

public class NetworkController : MonoBehaviour {

	public string ip;
    public int port;

    public string broadcastIP;
    public int broadcastPort;

    public GameControllerScript gameController;

	private ConnectionManager connManager;
	private bool connected;

	// Use this for initialization
	void Start () {
		
		connected = false;
		connManager = new ConnectionManager();

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
        connManager.open(ClientType.Unity, ConnectionType.WLAN, new WLANConnectionDef(ip, port), new ConnectionDelegates.ConnectedHandler(connectedCallback));
		connManager.receive(new ConnectionDelegates.ReceivedHandler(receiveCallback));
	}

    public void sendConn(AbstractTransferObject obj)
    {
		connManager.send(obj, new ConnectionDelegates.SentHandler(sentCallback));
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

        JsonData responseData = JsonMapper.ToObject(response);
        
	}
}
