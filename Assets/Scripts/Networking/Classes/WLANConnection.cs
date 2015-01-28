using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using LitJson;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class WLANConnection : ConnectionDelegates, IConnection {

	private Socket socket;

	public bool Open {
		get; private set;
	}
	
	public class StateObject {
		public Socket workSocket = null;
		public const int bufferSize = 1024;
		public byte[] buffer = new byte[bufferSize];
		public StringBuilder sb = new StringBuilder();
	}
	
	private ManualResetEvent connected = new ManualResetEvent(false);
	private ManualResetEvent sent = new ManualResetEvent(false);
	private ManualResetEvent received = new ManualResetEvent(false);

    public ReceivedHandler receivedCallback;
    private bool callbackSet = false;
    private string log = "";
    private string bufferLog = "";
	
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
	
	public void send(AbstractTransferObject obj, ConnectionDelegates.SentHandler callback) {
		if(socket.Connected) {	
			Sent += callback;
			String jsonString = obj.toJson() + "\n";
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
	
	public void receive(ConnectionDelegates.ReceivedHandler callback) {

        if (callbackSet == false)
        {
            receivedCallback = callback;
            Received += receivedCallback;
            callbackSet = true;
        }
        /*
		StateObject state = new StateObject();
		state.workSocket = socket;
		socket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveCallback), state);
         * */

        var responseListener = new SocketAsyncEventArgs();
        responseListener.Completed += responseListener_Completed;

        var responseBuffer = new byte[1024];
        responseListener.SetBuffer(responseBuffer, 0, 1024);

        socket.ReceiveAsync(responseListener);
	}

    private string trailingMessage;
    void responseListener_Completed(object sender, SocketAsyncEventArgs e)
    {
        var message = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);

        var bufferWasPreviouslyFull = !string.IsNullOrEmpty(trailingMessage);
        if (bufferWasPreviouslyFull)
        {
            message = trailingMessage + message;
            trailingMessage = null;
        }

        //asdfLog += message + "\r\n\r\n";
        //File.WriteAllText("C:\\Users\\faisstm\\Desktop\\buffer.txt", asdfLog);

        var lines = new LinkedList<String>(message.Split(new char[]{'\0'}, StringSplitOptions.None));
        var lastLine = lines.Last.Value;
        var isBufferFull = !string.IsNullOrEmpty(lastLine);

            if (isBufferFull)
            {
                trailingMessage = lastLine;
                lines.Remove(lastLine);
            }

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                OnReceived(line);
            }

        receive(receivedCallback);
    }


	/*
	private void receiveCallback(IAsyncResult result) {
		StateObject state = (StateObject) result.AsyncState;
		Socket client = state.workSocket;
        string response = "";

        int bytesRead = client.EndReceive(result);

        if (bytesRead > 0)
        {
            string[] responseSplit = Encoding.ASCII.GetString(state.buffer, 0, bytesRead).Split(new char[] {'\0'});

            bufferLog += Encoding.UTF8.GetString(state.buffer);

            File.WriteAllText("C:\\Users\\faisstm\\Desktop\\buffer.txt", bufferLog);

            log += "Length: " + responseSplit.Length + "\r\n";
            if (responseSplit.Length == 0)
            {
                log += "WTF????????";
            }
            foreach (string s in responseSplit)
            {
                log += s;
            }
            log += "\r\n\r\n";

            File.WriteAllText("C:\\Users\\faisstm\\Desktop\\log2.txt", log);
            if (responseSplit.Length == 1)
            {
                state.sb.Append(responseSplit[0]);
                client.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveCallback), state);
            }
            else
            {
                for (int i = 0; i < responseSplit.Length; i++)
                {
                    if (i == responseSplit.Length - 1)
                    {
                        if (responseSplit[i].Length != 0)
                        {
                            state.sb.Length = 0;
                            state.sb.Append(responseSplit[i]);
                            client.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveCallback), state);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        state.sb.Append(responseSplit[i]);
                        response = state.sb.ToString();
                        received.Set();
                        OnReceived(response);
                        state.sb.Length = 0;
                    }
                }
            }
            /*state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
            if (state.sb.ToString().Contains("\0"))
            {
                response = state.sb.ToString();
                received.Set();
                OnReceived(response);
                //Received -= receivedCallback;
            }
            else
            {
                client.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveCallback), state);
            }

        }
        else
        {
            if (state.sb.Length > 1)
            {
                response = state.sb.ToString();
                received.Set();
                OnReceived(response);
                //Received -= receivedCallback;
            }
        }

       
		int bytesRead = client.EndReceive(result);
		if(bytesRead < StateObject.bufferSize || bytesRead == 0) {
			state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
			string response = null;
			if(state.sb.Length > 1) {
				response = state.sb.ToString();
			}
			
			OnReceived(response);
			received.Set();
			state = new StateObject();
			state.workSocket = socket;
			
		} else {
			state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
		}
		
		client.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveCallback), state);
	}
    */

}
