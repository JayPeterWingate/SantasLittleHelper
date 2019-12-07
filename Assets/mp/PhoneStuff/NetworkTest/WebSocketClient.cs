using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;


public class WebSocketClient: MonoBehaviour {
	bool m_connected;
	WebSocket m_ws;
	Queue<Message> m_messages;
	public void Connect(string connectionString)
	{
		m_messages = new Queue<Message>();
        //Debug.Log(connectionString);
		m_ws = new WebSocket(connectionString);
		m_ws.OnMessage += (sender, e) => AddToQueue(e.Data);
		
		m_ws.Connect();
	}
	public void Close()
	{
		m_ws.Close();
	}
	public void Send(string data)
	{
		m_ws.Send(data);
	}

	public void AddToQueue(string message)
	{
		//Debug.Log(message);
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
			case MessageTypes.ModuleMessage:
				m = JsonUtility.FromJson<ModuleMessage>(message);
				break;
			case MessageTypes.LockdownResponse:
				m = JsonUtility.FromJson<LockdownResponse>(message);
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
