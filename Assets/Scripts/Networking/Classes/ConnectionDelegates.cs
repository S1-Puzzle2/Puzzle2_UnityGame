using UnityEngine;
using System.Collections;

public class ConnectionDelegates  {

	public delegate void ConnectedHandler();
	public event ConnectedHandler Connected;
	
	public delegate void SentHandler();
	public event SentHandler Sent;
	
	public void OnConnected() {
		Connected();
	}
	
	public void OnSent() {
		Sent();
	}
	

}
