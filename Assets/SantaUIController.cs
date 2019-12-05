using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaUIController : MonoBehaviour
{
    [SerializeField] ThirdPersonUserControl controller;
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i ++)
        {

            transform.GetChild(i).gameObject.SetActive(controller.playerNumber - 1 == i);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
