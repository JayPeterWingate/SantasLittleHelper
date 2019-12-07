using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public static MenuScript Instance;
    [SerializeField] int playerCount = 1;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStart()
    {
        PlayerPrefs.SetInt("playerCount", playerCount);
        SceneManager.LoadScene(1);
    }
}
