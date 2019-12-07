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
	static private string url = "OA.fullbeans.studio";
	static public string URL{ get { return url; } }
	private PhonePlayerController m_playerController;
	public static bool Enabled = true;
	private int m_blocklessModuleCount = 1;
	public int m_moduleCount = -1;
	private List<PhoneModule> m_modules;
	private WebSocketClient m_client;
	private bool m_inGame;
	private bool m_connected;

	private string m_code;
	public UnityAction<string> onCodeChange;
	public UnityAction<int> onPlayerCountChange;
	public string GetCode() { return m_code;}


	private void Start() {
		Settings.InitiliseSetting("url", "OA.fullbeans.studio");
		url = Settings.Get<string>("url");
        //url = "OA.fullbeans.studio"; //Halen HACK
        //url = "192.168.1.142:3001"; //Halen HACK
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
            if (UISystem.Instance != null)
            {
                UISystem.Instance.UpdatePhoneCode();
            }
            m_client.Connect("ws://" + url + "/" + m_code);
			if(onCodeChange != null)
			{
				onCodeChange.Invoke(m_code);
			}
            if (OperationManager.Instance.m_debug)
            {
                StartCoroutine(OperationManager.Instance.WaitForMapToGenerate(() => SendLevelInfoToPhones(0)));
            }
		}
        else
        {
            Debug.Log("CONNECTION TO '" + url + "' HAS FAILED!\nReverting to non-phone gamemode if possible");
            DisablePhoneMode();
            if (!OperationManager.Instance.CanBePlayedOffline())
            {
                UISystem.Instance.SetErrorText("Failed to connect to server! Please check network connection and restart!");
            }
        }
	}

    private void DisablePhoneMode()
    {
        PhoneNetworkManager.Enabled = false;
        UISystem.Instance.HidePhoneUI();
    }

	public void StartOperation(Operation opData)
	{
		if (!m_connected) return;
		StartOperationMessage message = new StartOperationMessage();
		message.playerCap = opData.suggestedPlayerCount;
		m_client.Send(JsonUtility.ToJson(message));
		if(m_modules != null)
		{
			m_modules.Clear();
		}
	}

    public void ClearModuleList()
    {
        m_moduleCount = -1;
        //Debug.Log("CLEAR MODULES");
        m_modules = new List<PhoneModule>();
    }

	public void NextLevel(int level)
	{
		if (!m_connected) return;
		m_inGame = false;
        //m_playerController.ScrambleModules(m_modules);
        SendLevelInfoToPhones(level);
    }

    public void KickAllPlayers()
    {
        List<PlayerData> players = m_playerController.Players;
        for (int i = 0; i < players.Count; i++)
        {
            KickPlayer(players[i].index);
        }
    }

    public void SendLevelInfoToPhones(int level)
    {
        if (!m_connected) return;

        LevelInfoMessage message = new LevelInfoMessage();
        message.current = level;
        message.map = OperationManager.Instance.GetPhoneMap();
        //string mapString = JsonUtility.ToJson(message.map);
        //for (int i = 0; i <= (int)mapString.Length/16000; i++)
        //{
        //    Debug.Log(mapString.Substring(i*16000, Mathf.Min(16000, mapString.Length - i * 16000)));
        //}
        m_client.Send(JsonUtility.ToJson(message));
    }

    public void EndOperation(float time, int award )
	{
		if (!m_connected) return;
		m_inGame = false;
		EndOperationMessage message = new EndOperationMessage();
		message.timeTaken = time;
		message.award = award;
		m_client.Send(JsonUtility.ToJson(message));
	}
	public List<PlayerData> GetPlayers()
	{
		return m_playerController == null ? null : m_playerController.Players;
	}
	public int PlayerCount()
	{
		return m_playerController != null ? m_playerController.PlayerCount : 1;
	}
	public void WitnessMe(PhoneModule mod)
	{
		if (!PhoneNetworkManager.Enabled) { return; }
		if (m_modules == null) { m_modules = new List<PhoneModule>(); }
        m_modules.Add(mod);
        mod.m_index = m_modules.Count - 1;
        //Debug.Log(m_moduleCount + " vs " + m_modules.Count);
		if (m_moduleCount == m_modules.Count) // isCompetitive && m_currentPlayerIdxs.Count == m_modules.Count)
        {
            //Debug.Log(m_modules.Count);
			m_inGame = true;
            StartCoroutine(TriggerScramble());
		}
	}

    public void HackyNoModuleGameplaySceneStart()
    {
        m_inGame = true;
        StartCoroutine(TriggerScramble());
    }

    private IEnumerator TriggerScramble()
    {
        yield return new WaitForEndOfFrame();
        DebugShowModuleInfo(m_modules);
        m_playerController.ScrambleModules(m_modules);
		PlayerTVElementAdjust();

	}
	
    public void DebugShowModuleInfo(List<PhoneModule> a_modules)
    {
        string debugString = "MODULES: ";
        if (a_modules != null)
        {
            for (int i = 0; i < a_modules.Count; i++)
            {
                debugString += a_modules[i].GetDebugDesc() + ", ";
            }
        }
        else
        {
            debugString += "NONE!";
        }
        //Debug.Log(debugString);
    }
	
	public bool IsInOfflineMode()
    {
        return (OperationManager.Instance.CanBePlayedOffline() && GetPlayers().Count == 0);
    }

    public bool IsInOfflineOperation()
    {
        return (!OperationManager.Instance.CanBePlayedOnline());
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

    public string GetModuleDebugStringFromIdx(int idx)
    {
        return m_modules[idx].GetDebugDesc();
    }

    private void PlayerTVElementAdjust()
    {
        if(UISystem.Instance != null)
		{
			UISystem.Instance.DisplayPhonePlayers(m_playerController.Players, OperationManager.Instance.GetMaxPlayerCountMidLevel());
		}
        if (m_modules != null)
        {
            for (int i = 0; i < m_modules.Count; i++)
            {
                m_modules[i].SetPlayerOwner(GetModuleOwner(m_modules[i].m_index));
            }
        }
    }

	void RecieveMessage(Message msg)
	{
		switch (msg.type)
		{
			case MessageTypes.PlayerJoined:
                
                //print("A NEW CHALLENGER APPROACHES");
				PlayerJoinedMessage pjm = (PlayerJoinedMessage)msg;
				m_playerController.AddPlayer(new PlayerData { index = pjm.id, color = pjm.color, icon = (PhoneUserIcon)pjm.icon, name = pjm.name }, m_modules);
				PlayerTVElementAdjust();

				if (onPlayerCountChange != null)
				{
					onPlayerCountChange.Invoke(m_playerController.PlayerCount);
				}

                if (OperationManager.Instance.m_lockdownPuzzle != null && OperationManager.Instance.IsCurrentlyLockdown())
                {
                    Debug.Log("BUSH DID 911");
                    OperationManager.Instance.m_lockdownPuzzle = OperationManager.Instance.m_lockdownPuzzle.LockdownFailReset(Lockdown.PuzzleFailReasons.PlayerJoined);
                }
				
				if (UISystem.Instance != null && OperationManager.Instance.CanBePlayedOnline()) {
					
					UISystem.Instance.SetPhoneUIActive(true);
				}
				break;
			case MessageTypes.PlayerLeft:
                
                PlayerLeftMessage plm = (PlayerLeftMessage)msg;
				m_playerController.RemovePlayer(plm.id, m_modules);
				
                PlayerTVElementAdjust();
				if (onPlayerCountChange != null)
				{
					onPlayerCountChange.Invoke(m_playerController.PlayerCount);
				}

                if (OperationManager.Instance.m_lockdownPuzzle != null && OperationManager.Instance.IsCurrentlyLockdown())
                {
                    OperationManager.Instance.m_lockdownPuzzle = OperationManager.Instance.m_lockdownPuzzle.LockdownFailReset(Lockdown.PuzzleFailReasons.PlayerLeft);
                }
                if (UISystem.Instance != null && GetPlayers().Count == 0 && OperationManager.Instance.CanBePlayedOffline()) { UISystem.Instance.SetPhoneUIActive(false); }
                break;
			case MessageTypes.ModuleMessage:
				ModuleMessage mm = (ModuleMessage)msg;
				m_modules[mm.id].RecieveMessage(mm.data);
				break;
			case MessageTypes.LockdownResponse:
				print(msg);
				LockdownResponse lrm = (LockdownResponse)msg;
                // Hand Lockdown response to the validator of the puzzle
                LockdownPuzzleResult result = OperationManager.Instance.m_lockdownPuzzle.Validator(lrm.data);

                //bool response = OperationManager.Instance.m_lockdownPuzzle.Validator(lrm.data);
				if(result == LockdownPuzzleResult.Success) {
					OperationManager.Instance.EndLockdown();
				} else if (result == LockdownPuzzleResult.Failure)
				{
					print("FAIL");
					OperationManager.Instance.IntensifyLockdown();
                    OperationManager.Instance.m_lockdownPuzzle = OperationManager.Instance.m_lockdownPuzzle.LockdownFailReset(Lockdown.PuzzleFailReasons.PartyFail);
				}
				break;
		}
	}
}
