using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.Events;

public class PressieSpawner : MonoBehaviour
{
	[SerializeField] float range;
	[SerializeField] GameObject pressiePrefab;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] TMPro.TextMeshProUGUI victoryText;
    private int remainingPressieCount;
    // Start is called before the first frame update
    void Start()
    {
        victoryScreen.SetActive(false);
        remainingPressieCount = HouseDropPoint.allTheHouses.Count;
		foreach(HouseDropPoint house in HouseDropPoint.allTheHouses)
		{
			GameObject newPressie = Instantiate(pressiePrefab, GetSpawnPoint(), new Quaternion(), transform);
			newPressie.GetComponent<Pressie>().setHouse(house);
		}
        FullbeansNetworking.PlayerData mockData = new FullbeansNetworking.PlayerData();
        HouseDropPoint.onChange.AddListener(OnScore);
    }
    
    private void OnScore()
    {
        print(remainingPressieCount);
        remainingPressieCount -= 1;

        print(remainingPressieCount);
        if (remainingPressieCount == 0)
        {
            int winnerId;
            string winner;
            victoryScreen.SetActive(true);
            try
            {
                winnerId = ThirdPersonUserControl.players.Aggregate((m, p) => m.score < p.score ? m : p).playerNumber;
                winner = PhoneNetworkManager.manager.GetPlayers().Where(p => p.index == winnerId).First().name;
            } catch
            {
                winner = "Dave";
            }
            victoryText.text = "MVP: " + winner;


        }
    }
	private Vector3 GetSpawnPoint()
	{
		return transform.position + new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(0, range));
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
