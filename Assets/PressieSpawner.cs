using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PressieSpawner : MonoBehaviour
{
	[SerializeField] float range;
	[SerializeField] GameObject pressiePrefab;
    // Start is called before the first frame update
    void Start()
    {
		foreach(HouseDropPoint house in HouseDropPoint.allTheHouses)
		{
			GameObject newPressie = Instantiate(pressiePrefab, GetSpawnPoint(), new Quaternion(), transform);
			newPressie.GetComponent<Pressie>().setHouse(house);
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
