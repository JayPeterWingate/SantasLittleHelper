using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum MessageTypes
{
	PlayerJoined = 0,
	PlayerLeft = 1,
	PlayerKick = 3,

	OperationStarted = 100,
	OperationEnded = 101,
	LevelInfo = 102,

	ScrambleModules = 200,
	ModuleMessage = 201,
	ModuleUpdate = 202,

	Lockdown = 300,
	TriggerLockdown = 301,
	LockdownResponse = 302,
	LockdownResolved = 303,
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