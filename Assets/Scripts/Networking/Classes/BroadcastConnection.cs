using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class BroadcastConnection : ConnectionDelegates, IConnection
{

    private Socket client;
    private IPEndPoint ep;
    private IPEndPoint receiveEP;

    private byte[] resData;

    private ManualResetEvent sent = new ManualResetEvent(false);
    private ManualResetEvent received = new ManualResetEvent(false);

    public void open(ClientType type, ConnectionType connType, IConnectionDef def, ConnectionDelegates.ConnectedHandler callback)
    {
        WLANConnectionDef bDef = (WLANConnectionDef)def;
        ep = new IPEndPoint(IPAddress.Parse(bDef.Ip), bDef.Port);
        receiveEP = new IPEndPoint(IPAddress.Any, 0);

        client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        client.Bind(receiveEP);
    }

    public void close()
    {
        client.Close();
    }

    public void send(AbstractTransferObject obj, ConnectionDelegates.SentHandler callback)
    {
        Sent += callback;
        String jsonString = obj.toJson() + "\n";
        byte[] jsonBA = Encoding.UTF8.GetBytes(jsonString);

        SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
        eventArgs.SetBuffer(jsonBA, 0, jsonBA.Length);
        eventArgs.RemoteEndPoint = ep;
        eventArgs.Completed += broadcast_send_completed;

        client.SendToAsync(eventArgs);
    }

    void broadcast_send_completed(object sender, SocketAsyncEventArgs e)
    {
        sent.Set();
        OnSent();
    }

    public void receive(ConnectionDelegates.ReceivedHandler callback)
    {
        Received += callback;

        SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
        resData = new byte[1024];
        eventArgs.SetBuffer(resData, 0, resData.Length);
        eventArgs.Completed += broadcast_receive_completed;

        client.ReceiveAsync(eventArgs);
    }

    private void broadcast_receive_completed(object sender, SocketAsyncEventArgs e)
    {
        OnReceived(Encoding.UTF8.GetString(e.Buffer));
        received.Set();
        client.ReceiveAsync(e);
    }
   
}
