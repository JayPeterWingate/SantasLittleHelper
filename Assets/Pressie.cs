using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pressie : MonoBehaviour
{
	Transform pickUpLocation = null;
	Rigidbody m_rigid;
	HouseDropPoint house;
	// Start is called before the first frame update
	void Start()
	{
		m_rigid = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		if(pickUpLocation != null)
		{
			m_rigid.position = pickUpLocation.position;
		}
	}
	public void pickUp(Transform newPosition)
	{
		print("FIRE");
		pickUpLocation = newPosition;
		m_rigid.isKinematic = true;
		GetComponent<BoxCollider>().isTrigger = true;
		
	}

	internal void setHouse(HouseDropPoint house)
	{
		this.house = house;
	}
	public HouseDropPoint getHouse()
	{
		return house;
	}
	public void drop()
	{
		GetComponent<BoxCollider>().isTrigger = false;
		print("OH SHIT");
		m_rigid.isKinematic = false;
		pickUpLocation = null;
	}

}

