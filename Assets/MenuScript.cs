using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;
using FullbeansNetworking;

public class MenuScript : MonoBehaviour
{
    public static MenuScript Instance;
	[SerializeField] TextMeshProUGUI link;
	[SerializeField] GameObject startBtn;
	[SerializeField] Transform PlayerContainer;
    [SerializeField] int playerCount = 1;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
		link.text = PhoneNetworkManager.manager.m_code;
		syncPlayers(0);
		PhoneNetworkManager.manager.onPlayerCountChange += syncPlayers;
    }
	private void OnDestroy()
	{
		PhoneNetworkManager.manager.onPlayerCountChange -= syncPlayers;
	}
	private void syncPlayers(int newPlayerCount)
	{
		print("SAD");
		List<PlayerData> data = PhoneNetworkManager.manager.GetPlayers();
		startBtn.SetActive(newPlayerCount != 0);
		for (int i = 0; i < 4; i++)
		{
			Transform t = PlayerContainer.GetChild(i);
			if(i < data.Count)
			{
				t.gameObject.SetActive(true);
				t.GetComponentInChildren<TextMeshPro>().text = data[i].name;
			} else
			{
				t.gameObject.SetActive(false);
			}
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }

    public void GameStart()
    {
		StartOperationMessage startMessage = new StartOperationMessage();
		PhoneNetworkManager.manager.SendPhoneMessage(startMessage);
		playerCount = PhoneNetworkManager.manager.PlayerCount();
        PlayerPrefs.SetInt("playerCount", playerCount);
        SceneManager.LoadScene(1);
    }

	internal void UpdatePhoneCode()
	{
		link.text = PhoneNetworkManager.manager.GetCode();
	}
}
