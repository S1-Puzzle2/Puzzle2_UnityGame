using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ConnectionType {
	WLAN,
	Bluetooth
}

public class ConnectionManager : IConnection {

	private IConnection currConn;

	public void open(ClientType clientType, ConnectionType connType, IConnectionDef def, ConnectionDelegates.ConnectedHandler callback) {
		switch (connType) {
		case ConnectionType.WLAN:
			currConn = new WLANConnection();
			currConn.open(clientType, connType, def, callback);
			
			break;
		
		case ConnectionType.Bluetooth:
			// bluetooth connection
			break;

		default:
			break;
		}
	}

	public void close() {
		currConn.close ();
	}
	
	public void send(TransferObject obj, ConnectionDelegates.SentHandler callback) {
		currConn.send (obj, callback);
	}

}
