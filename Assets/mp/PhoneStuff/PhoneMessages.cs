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
public struct ModuleData
{
	public int ownerId;
	public int id;
	public Vector4 rect;
	public BlockTypes type;
	public string data;
    public float usageWaitPeriod;

	public static implicit operator List<object>(ModuleData v)
	{
		throw new NotImplementedException();
	}
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

public class LevelInfoMessage : Message
{
	public ExportableLevelDataForPhones map;
	public LevelInfoMessage()
	{
		type = MessageTypes.LevelInfo;
	}
	public int current;
}

[Serializable]
public class ScrambleModulesMessage : Message
{
	public ScrambleModulesMessage()
	{
		type = MessageTypes.ScrambleModules;
	}
	public int id;
	public ModuleData[] moduleData;
	public int totalModuleCount;
	public bool isFirst;
}

public class ModuleMessage : Message
{
	public ModuleMessage()
	{
		type = MessageTypes.ModuleMessage;
	}
	public int id;
	public string data;
}

public class ModuleUpdate : Message
{
	public ModuleUpdate()
	{
		type = MessageTypes.ModuleUpdate;
	}
	public int target; // Player who owns the module
	public int id;
	public string data;
}

public class LockdownInitiated : Message
{
	public LockdownInitiated()
	{
		type = MessageTypes.TriggerLockdown;
	}
}
public class LockdownMessage : Message
{
	public LockdownMessage()
	{
		type = MessageTypes.Lockdown;
	}
	public int id;
	public Lockdown.PuzzleFailReasons lastAttempt;
	public Lockdown.LockdownPuzzleType typeOfPuzzle;
	public string data;
}
public class LockdownResponse : Message
{
	public LockdownResponse()
	{
		type = MessageTypes.LockdownResponse;
	}
	public string data;
}
public class LockdownResolved : Message
{
	public LockdownResolved()
	{
		type = MessageTypes.LockdownResolved;
	}
}