using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(TcpListenSocketBehaviour))]
public class TcpSocketAsyncBehaviour : MonoBehaviour
{
    private Socket socket;
    [Tooltip("The port the service is running on")] public int port = 9021;
    [SerializeField] private TcpListenSocketBehaviour listener;


    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitUntil(() => listener.isReady);

        socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(IPAddress.Parse("127.0.0.1"), port);
        var msg = Encoding.ASCII.GetBytes("Hello, from Client!");
        socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, SendCompelete, socket);
    }

    private void SendCompelete(IAsyncResult asyncResult)
    {
        if (asyncResult.IsCompleted)
        {
            Socket socket = asyncResult.AsyncState as Socket;
            var bytesSent = socket.EndSend(asyncResult);
            print($"{bytesSent} bytes sent");
        }
    }
}
