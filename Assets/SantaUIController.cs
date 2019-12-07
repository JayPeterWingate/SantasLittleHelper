using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SantaUIController : MonoBehaviour
{
    [SerializeField] ThirdPersonUserControl controller;
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i ++)
        {
            if(controller.playerNumber - 1 == i)
            {
                text = transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
            }
            transform.GetChild(i).gameObject.SetActive(controller.playerNumber - 1 == i);
            
        }
        text.text = 0.ToString();
        controller.onPointScore.AddListener(() => {
            text.text = controller.score.ToString();
        });
    }
}
