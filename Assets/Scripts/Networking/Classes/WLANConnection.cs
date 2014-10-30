using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using LitJson;
using System.Text;

public class WLANConnection : ConnectionDelegates, IConnection {

	private Socket socket;

	public bool Open {
		get; private set;
	}
	
	private ManualResetEvent connected = new ManualResetEvent(false);
	private ManualResetEvent sent = new ManualResetEvent(false);
	private ManualResetEvent received = new ManualResetEvent(false);
		
	public void open(ClientType type, ConnectionType connType, IConnectionDef def, ConnectionDelegates.ConnectedHandler callback) {
	
		Connected += callback;
		WLANConnectionDef wlanDef = (WLANConnectionDef)def;
		IPEndPoint endPoint = new IPEndPoint (IPAddress.Parse (wlanDef.Ip), wlanDef.Port);

		socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.BeginConnect (endPoint, new System.AsyncCallback (OnConnectCallback), socket);		
		connected.WaitOne ();
		
		OnConnected();	
	}

	public void close() {
	}
	
	public void send(TransferObject obj, ConnectionDelegates.SentHandler callback) {
		if(socket.Connected) {	
		
			Sent += callback;
			String jsonString = JsonMapper.ToJson(obj);
			byte[] jsonBA = Encoding.UTF8.GetBytes(jsonString);
			Debug.Log(jsonString);
			socket.BeginSend(jsonBA, 0, jsonBA.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
		}
	}
	
	private void SendCallback(IAsyncResult result) {
		Socket resceiver = (Socket)result.AsyncState;
		resceiver.EndSend(result);
		sent.Set();
		OnSent();
	}

	private void OnConnectCallback(IAsyncResult result) {
		Socket server = (Socket)result.AsyncState;
		server.EndConnect(result);
		connected.Set();

	}


}
