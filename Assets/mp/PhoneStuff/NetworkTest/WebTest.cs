using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;


public class WebTest : MonoBehaviour {
	private int m_poolCounter = 0;
	public TextMeshProUGUI m_inputfield;
	public GameObject m_playerPrefab;
	public Transform m_playerContainer;
	private WebSocketClient m_client;
	private Dictionary<int, TestPlayerScript> m_players;

	public GameObject m_startButton;
	public GameObject m_connectButton;
	public string m_url;
	// Use this for initialization
	void Start () {

		m_client = GetComponent<WebSocketClient>();
		m_players = new Dictionary<int, TestPlayerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		//Check for anything in queue
		Message incomingMessage = m_client.GetMessage();
		if (incomingMessage != null)
		{
			RecieveMessage(incomingMessage);
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			m_client.Send("on");
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			m_client.Send("off");
		}
	}

	public void ConnectToWeb()
	{
		string code = m_inputfield.text;
		m_client.Connect("ws://"+m_url+"/" + code);
		
	}
	void RecieveMessage(Message msg)
	{
		switch (msg.type)
		{
			
		}
	}

	public void StartGame()
	{
		StartCoroutine(AuxStartGame());
	}
	private IEnumerator AuxStartGame()
	{
		string code = m_inputfield.text;
		print("http://" + m_url + "/newGame");
		UnityWebRequest myWr = UnityWebRequest.Get("http://"+m_url+"/newGame");
		yield return myWr.SendWebRequest();

		if(myWr.responseCode == 200)
		{
			print(myWr.downloadHandler.text);
		} 
	}

}
