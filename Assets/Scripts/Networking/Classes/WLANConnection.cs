﻿using UnityEngine;
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
	
	public class StateObject {
		public Socket workSocket = null;
		public const int bufferSize = 256;
		public byte[] buffer = new byte[bufferSize];
		public StringBuilder sb = new StringBuilder();
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
	
	private void OnConnectCallback(IAsyncResult result) {
		Socket server = (Socket)result.AsyncState;
		server.EndConnect(result);
		connected.Set();
	}

	public void close() {
		socket.Shutdown(SocketShutdown.Both);
		socket.Close();
	}
	
	public void send(TransferObject obj, ConnectionDelegates.SentHandler callback) {
		if(socket.Connected) {	
		
			Sent += callback;
			String jsonString = JsonMapper.ToJson(obj) + "\n";
			byte[] jsonBA = Encoding.UTF8.GetBytes(jsonString);
			Debug.Log(jsonString);
			socket.BeginSend(jsonBA, 0, jsonBA.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
		}
	}
	
	private void SendCallback(IAsyncResult result) {
		Socket receiver = (Socket)result.AsyncState;
		receiver.EndSend(result);
		socket.EndSend(result);
		sent.Set();
		OnSent();
	}
	
	public void receive(Socket client) {
		StateObject state = new StateObject();
		state.workSocket = client;
		client.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveCallback), state);
	}
	
	private void receiveCallback(IAsyncResult result) {
		StateObject state = (StateObject) result.AsyncState;
		Socket client = state.workSocket;
		
		int bytesRead = client.EndReceive(result);
		if(bytesRead > 0) {
			state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
			client.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveCallback), state);
		} else {
			string response = null;
			if(state.sb.Length > 1) {
				response = state.sb.ToString();
			}
			
			OnReceived(response);
			received.Set();
		}
	}




}
