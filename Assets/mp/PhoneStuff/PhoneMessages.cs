using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct NewGameResponse
{
	public string connectionString;
	public string code;
}

[Serializable]
public enum MessageTypes
{
	PlayerJoined = 0,
	PlayerLeft = 1,
	PlayerKick = 3,

	OperationStarted = 100,
	OperationEnded = 101,

	PlayerMove = 201,
	PlayerAction = 202,
}
[Serializable]
public enum PhoneUserIcon
{
    Anon,
    ClassicSpy,
    Ninja,
    Smith,
}
[Serializable]
public class Message
{
	public MessageTypes type;
}
public class PlayerActionMessage : Message
{
	public int id;
	public int pid;
	public bool jump;
}
public class PlayerMoveMessage : Message
{
	public int id;
	public float x;
	public float y;
	public bool jmp;
	public bool drp;
	public int pid;
}
public class PlayerJoinedMessage : Message
{
	public PlayerJoinedMessage()
	{
		type = MessageTypes.PlayerJoined;
	}
	public int id;
    public string name;
    public Vector4 color;
    public int icon;
};

public class PlayerLeftMessage : Message
{
	public PlayerLeftMessage()
	{
		type = MessageTypes.PlayerLeft;
	}
	public int id;
};
public class PlayerKickMessage : Message
{
	public PlayerKickMessage()
	{
		type = MessageTypes.PlayerKick;
	}
	public int id;
}
public class StartOperationMessage : Message
{
	public int playerCap;
	public StartOperationMessage()
	{
		type = MessageTypes.OperationStarted;
	}
};

public class EndOperationMessage : Message
{
	public EndOperationMessage()
	{
		type = MessageTypes.OperationEnded;
	}
	public float timeTaken;
	public int award;
};