using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
    using WebSocketSharp;
#endif

public class WebSocketClient: MonoBehaviour {
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void closeSocket();
    [DllImport("__Internal")]
    private static extern void sendMessage(string msg);
    [DllImport("__Internal")] 
    private static extern void connectToSocket(string socket);
#endif
    bool m_connected;
#if UNITY_EDITOR
    WebSocket m_ws;
#endif
    Queue<Message> m_messages;
	public void Connect(string connectionString)
	{

        m_messages = new Queue<Message>();
        //Debug.Log(connectionString);
#if UNITY_EDITOR
        m_ws = new WebSocket(connectionString);
        m_ws.OnMessage += (sender, e) => AddToQueue(e.Data);

        m_ws.Connect();
#endif
#if UNITY_WEBGL
        connectToSocket(connectionString);
#endif

    }
    public void Close()
    {
#if UNITY_EDITOR
        m_ws.Close();
#endif
#if UNITY_WEBGL
        closeSocket();
#endif
    }
    public void Send(string data)
    {
#if UNITY_EDITOR
        m_ws.Send(data);
#endif
#if UNITY_WEBGL
        sendMessage(data);
#endif
    }

	public void AddToQueue(string message)
	{
		Debug.Log(message);
		// TODO - REMOVE THIS BOLLOCKS
		Message m = JsonUtility.FromJson<Message>(message);
		switch(m.type)
		{
			case MessageTypes.PlayerJoined:
				m = JsonUtility.FromJson<PlayerJoinedMessage>(message);
				break;
			case MessageTypes.PlayerLeft:
				m = JsonUtility.FromJson<PlayerLeftMessage>(message);
				break;
			case MessageTypes.PlayerMove:
				
				m = JsonUtility.FromJson<PlayerMoveMessage>(message);
				break;
			case MessageTypes.PlayerAction:
				m = JsonUtility.FromJson<PlayerActionMessage>(message);
				break;
		}
		m_messages.Enqueue(m);

	}
	public Message GetMessage()
	{
		if(m_messages != null && m_messages.Count > 0)
		{
			return m_messages.Dequeue();
		}
		return null;
	}
}
