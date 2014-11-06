using UnityEngine;
using System.Collections;

public class ConnectionDelegates  {

	public delegate void ConnectedHandler();
	public event ConnectedHandler Connected;
	
	public delegate void SentHandler();
	public event SentHandler Sent;
	
	public delegate void ReceivedHandler(string response);
	public event ReceivedHandler Received;
	
	public void OnConnected() {
		Connected();
	}
	
	public void OnSent() {
		Sent();
	}
	
	public void OnReceived(string response) {
		Received(response);
	}
	

}
