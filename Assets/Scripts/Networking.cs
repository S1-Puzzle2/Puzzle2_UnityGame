using UnityEngine;
using System.Collections;
using System.Net.Sockets;

public class Networking : MonoBehaviour {

	public string connectionIP = "127.0.0.1";
	public int connectionPort = 4712;

	public bool connected;
	public TcpClient clientSocket;

	// Use this for initialization
	void Start () {
		clientSocket = new TcpClient ();
		clientSocket.Connect (connectionIP, connectionPort);
	}
	
	// Update is called once per frame
	void Update () {
	
		if (clientSocket.Connected) {
			byte[] str = getBytes(Time.time.ToString() + "\n");
			clientSocket.GetStream().Write(str, 0, str.Length);
			clientSocket.GetStream().Flush();
		}

	}

	byte[] getBytes(string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy (str.ToCharArray (), 0, bytes, 0, bytes.Length);
		return bytes;
	}
}
