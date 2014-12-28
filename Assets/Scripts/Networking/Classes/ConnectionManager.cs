using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ConnectionType {
	WLAN,
	Bluetooth,
    Broadcast
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

       case ConnectionType.Broadcast:
            currConn = new BroadcastConnection();
            currConn.open(clientType, connType, def, callback);
            break;
		default:
			break;
		}
	}

	public void close() {
		currConn.close ();
	}
	
	public void send(AbstractTransferObject obj, ConnectionDelegates.SentHandler callback) {
		currConn.send (obj, callback);
	}
	
	public void receive(ConnectionDelegates.ReceivedHandler callback) {
		currConn.receive(callback);
	}

}
