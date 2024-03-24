using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TcpListenSocketBehaviour : MonoBehaviour
{
    private TcpListener listener;
    [HideInInspector] public bool isReady;
    [Tooltip("The port the service is running on")] public int port = 9021;


    private void Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        listener.BeginAcceptSocket(SocketConnected, listener);
        isReady = true;
    }

    private void OnDestroy()
    {
        listener?.Stop();
        listener = null;
    }

    private void SocketConnected(IAsyncResult asyncResult)
    {
        if (asyncResult.IsCompleted)
        {
            Socket socket = (asyncResult.AsyncState as TcpListener).EndAcceptSocket(asyncResult);
            StateObject state = new StateObject(socket);
            socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, SocketReceived, state);
        }
    }

    private void SocketReceived(IAsyncResult asyncResult)
    {
        if (asyncResult.IsCompleted)
        {
            StateObject state = asyncResult.AsyncState as StateObject;
            int bytesIn = state.Socket.EndReceive(asyncResult);

            if (bytesIn > 0)
            {
                string msg = Encoding.ASCII.GetString(state.Buffer, 0, bytesIn);
                print($"From client: {msg}");
            }

            StateObject newState = new StateObject(state.Socket);
            state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, SocketReceived, newState);
        }
    }
}
