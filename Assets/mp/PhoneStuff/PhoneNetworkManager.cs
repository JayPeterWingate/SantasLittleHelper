using FullbeansNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public enum LockdownPuzzleResult
{
    Success,
    Failure,
    Continue,
}

public class PhoneNetworkManager:MonoBehaviour {
	static public PhoneNetworkManager manager;
	static private string url = "OA.fullbeans.studio";
	static public string URL{ get { return url; } }
	private PhonePlayerController m_playerController;
	public static bool Enabled = true;
	private WebSocketClient m_client;
	private bool m_inGame;
	private bool m_connected;
	public bool Connected { get { return m_connected; } }

	private string m_code;
	public UnityAction<string> onCodeChange;
	public UnityAction<int> onPlayerCountChange;
	public string GetCode() { return m_code;}


	private void Start() {
		DontDestroyOnLoad(this);
		manager = this;
        url = "oa.fullbeans.studio";
        if (!PhoneNetworkManager.Enabled) { return; }
		m_inGame = false;
		m_client = gameObject.AddComponent<WebSocketClient>();

		m_playerController = new PhonePlayerController();

        StartCoroutine(EstablishConnection());
	}
	private void OnDestroy()
	{
		UnityWebRequest myWr = UnityWebRequest.Get("http://" + url + "/closeGame?code=" + m_code);
		myWr.SendWebRequest();
		m_client.Close();
		print("Destroyed the server");
	}
	private IEnumerator EstablishConnection()
	{
		
		//print("http://" + url + "/newGame");
		UnityWebRequest myWr = UnityWebRequest.Get("http://" + url + "/newGame");
		yield return myWr.SendWebRequest();
		m_connected = myWr.responseCode == 200;
		if (myWr.responseCode == 200 && myWr.downloadHandler.text.Length == 4) //Incorrect code length signifies a connection to the test server, not the correct build
		{
			m_code = myWr.downloadHandler.text;
            //Debug.Log(m_code);
            if (MenuScript.Instance != null)
            {
                MenuScript.Instance.UpdatePhoneCode();
            }
            m_client.Connect("ws://" + url + "/" + m_code);
			if(onCodeChange != null)
			{
				onCodeChange.Invoke(m_code);
			}
		}
        else
        {
            Debug.Log("CONNECTION TO '" + url + "' HAS FAILED!\nReverting to non-phone gamemode if possible");
        }
	}
    

    public void KickAllPlayers()
    {
        List<PlayerData> players = m_playerController.Players;
        for (int i = 0; i < players.Count; i++)
        {
            KickPlayer(players[i].index);
        }
    }
    
	public List<PlayerData> GetPlayers()
	{
		return m_playerController == null ? null : m_playerController.Players;
	}

	public int PlayerCount()
	{
		return m_connected ? m_playerController.PlayerCount : 1;
	}
    
	void Update()
	{
		if (!PhoneNetworkManager.Enabled) { return; }
		//Check for anything in queue
		Message incomingMessage = m_client.GetMessage();
		if (incomingMessage != null)
		{
			RecieveMessage(incomingMessage);
		}
	}
	public void SendPhoneMessage(Message message)
	{
		m_client.Send(JsonUtility.ToJson(message));
	}
	public void KickPlayer(int playerId)
	{
		PlayerKickMessage newMessage = new PlayerKickMessage();
		newMessage.id = playerId;

		SendPhoneMessage(newMessage);
	}

	public int GetModuleOwnerIndex(int id)
	{
        if (m_playerController.PlayerCount == 0)
        {
            return -1;
        }
        PlayerData player = GetModuleOwner(id);
        return player.index;
	}

    public PlayerData GetModuleOwner(int id)
    {
        if (m_playerController.PlayerCount == 0)
        {
            return null;
        }
        //THIS HACK FIXES ERRORS ON GAMEPLAY SCENE
        //if (OperationManager.Instance.m_debug) { return m_playerController.Players[0]; }
		
        return m_playerController.Players.Find(p => p.data.Contains(id));
    }

	void RecieveMessage(Message msg)
	{
		switch (msg.type)
		{
			case MessageTypes.PlayerJoined:
                
                //print("A NEW CHALLENGER APPROACHES");
				PlayerJoinedMessage pjm = (PlayerJoinedMessage)msg;
				m_playerController.AddPlayer(new PlayerData { index = pjm.id, color = pjm.color, icon = (PhoneUserIcon)pjm.icon, name = pjm.name });
				
				if (onPlayerCountChange != null)
				{
					onPlayerCountChange.Invoke(m_playerController.PlayerCount);
				}
				
				
				break;
			case MessageTypes.PlayerLeft:
                
                PlayerLeftMessage plm = (PlayerLeftMessage)msg;
				m_playerController.RemovePlayer(plm.id);
				
				if (onPlayerCountChange != null)
				{
					onPlayerCountChange.Invoke(m_playerController.PlayerCount);
				}
                
                break;
			case MessageTypes.PlayerMove:
				PlayerMoveMessage pmm = (PlayerMoveMessage)msg;
				ThirdPersonUserControl.players[pmm.pid - 1].setAxis(pmm.x, pmm.y);
				break;
		}
	}
}
