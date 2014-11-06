using UnityEngine;
using System.Collections;

public enum ClientType {
	Unity,
	Mobile,
	Configurator
}

public interface IConnection {

	void open(ClientType type, ConnectionType connType, IConnectionDef def, ConnectionDelegates.ConnectedHandler callback);
	void close();

	void send(TransferObject obj, ConnectionDelegates.SentHandler callback);
	void receive(ConnectionDelegates.ReceivedHandler callback);

}
