using UnityEngine;
using System.Collections;
using System.Threading;
using LitJson;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour {

	ConnectionManager connManager;

	// Use this for initialization
	void Start () {
		
		connManager = new ConnectionManager();
		
		Thread thread = new Thread(openConn);
		thread.Start();
	}
	
	private void openConn() {
		connManager.open(ClientType.Unity, ConnectionType.WLAN, new WLANConnectionDef("172.16.50.73", 4711), new ConnectionDelegates.ConnectedHandler(connectedCallback));
		
		string[] testArray = new string[3];
		testArray[0] = "Hallo";
		testArray[1] = "ihr";
		testArray[2] = "Idioten";
		
		TransferObject testObj = new TransferObject(Command.testCommand, testArray);
	}
	
	private void connectedCallback() {
		Debug.Log("Connection succesfull");
	}
	
	private void sentCallback() {
		Debug.Log("Sent message");
	}
}
