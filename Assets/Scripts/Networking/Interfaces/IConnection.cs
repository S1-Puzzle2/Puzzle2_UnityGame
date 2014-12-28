using UnityEngine;
using System.Collections;

public interface IConnection {

	void open(ClientType type, ConnectionType connType, IConnectionDef def, ConnectionDelegates.ConnectedHandler callback);
	void close();

	void send(AbstractTransferObject obj, ConnectionDelegates.SentHandler callback);
	void receive(ConnectionDelegates.ReceivedHandler callback);

}
