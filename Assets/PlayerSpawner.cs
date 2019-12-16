using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] int minPlayerCount;
    // Start is called before the first frame update
    void Start()
    {
        int playerCount = GetPlayerCount();
        for ( int i = 1; i < playerCount + 1; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            newPlayer.transform.position = transform.position + new Vector3(2 * i, 0, 0);
            Camera cam = newPlayer.GetComponentInChildren<Camera>();
			cam.cullingMask = (1 << 0) | (1<<5) | (1<<13) | (1<<(i + 8));
			cam.rect = GetPlayerRect(i, playerCount);
            ThirdPersonUserControl controller = newPlayer.GetComponentInChildren<ThirdPersonUserControl>();
            controller.playerNumber = i;
			
        }
    }
    
    int GetPlayerCount()
    {
        return PhoneNetworkManager.manager.PlayerCount();
    }

    Rect GetPlayerRect(int index, int playerCount)
    {
        float x = index % 2 == 1 ? 0 : 0.5f;
        float y = 3 - index > 0 ? 0 : 0.5f;

        float width = playerCount <= 1 ? 1 : 0.5f;
        float height = playerCount <= 2 ? 1 : 0.5f;
        return new Rect(x, y, width, height);
    }
}