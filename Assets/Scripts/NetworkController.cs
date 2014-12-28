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

    private void broadcastOpen()
    {
        connManager.open(ClientType.Unity, ConnectionType.Broadcast, new WLANConnectionDef(broadcastIP, broadcastPort), new ConnectionDelegates.ConnectedHandler(connectedCallback));
    }

    private void broadcastSend()
    {
        SimpleParameterTransferObject spto = new SimpleParameterTransferObject(Command.AreYouThere, null);
        Debug.Log(spto.toJson());
        connManager.send(spto, new ConnectionDelegates.SentHandler(sentCallback));
    }

    private void broadcastReceive()
    {
        connManager.receive(new ConnectionDelegates.ReceivedHandler(receiveCallback));
    }

	private void openConn() {
        connManager.open(ClientType.Unity, ConnectionType.WLAN, new WLANConnectionDef(ip, broadcastPort), new ConnectionDelegates.ConnectedHandler(connectedCallback));
		connManager.receive(new ConnectionDelegates.ReceivedHandler(receiveCallback));
	}
	
	private void sendConn(AbstractTransferObject obj) {
		connManager.send(obj, new ConnectionDelegates.SentHandler(sentCallback));
	}
	
	
	private void closeConn() {
		connManager.close();
	}
	
	private void connectedCallback() {
		connected = true;
		Debug.Log("Connection succesfull");
	}
	
	private void sentCallback() {
		Debug.Log("Sent message");
	}
	
	private void receiveCallback(string response) {
		Debug.Log("Response: " + response);
	}
}
