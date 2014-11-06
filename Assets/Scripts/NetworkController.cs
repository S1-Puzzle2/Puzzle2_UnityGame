using UnityEngine;
using System.Collections;
using System.Threading;
using LitJson;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour {

	public string ip;
	public int port;
	
	private ConnectionManager connManager;
	private bool connected;

	// Use this for initialization
	void Start () {
		
		connected = false;
		connManager = new ConnectionManager();
		
		Thread thread = new Thread(openConn);
		thread.Start();
	}
	
	private void openConn() {
		connManager.open(ClientType.Unity, ConnectionType.WLAN, new WLANConnectionDef(ip, port), new ConnectionDelegates.ConnectedHandler(connectedCallback));
		connManager.receive(new ConnectionDelegates.ReceivedHandler(receiveCallback));
	}
	
	private void sendConn(TransferObject obj) {
		connManager.send(obj, new ConnectionDelegates.SentHandler(sentCallback));
	}
	
	public void testSend() {
		TransferObject obj = new TransferObject(Command.testCommand, "asdf", 47);
		sendConn(obj);
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
		Debug.Log(response);
	}
}
