using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreKeeper : MonoBehaviour
{
	TextMeshProUGUI mesh;
	// Start is called before the first frame update
	void Start()
    {
		mesh = GetComponent<TextMeshProUGUI>();
		mesh.text = HouseDropPoint.allTheHouses.Count.ToString();
		HouseDropPoint.onChange.AddListener(() =>
		{
			mesh.text = HouseDropPoint.allTheHouses.Count.ToString();
		});
    }
	
}
