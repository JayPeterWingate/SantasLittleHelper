using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class HouseDropPoint : MonoBehaviour
{
	public static List<HouseDropPoint> allTheHouses = new List<HouseDropPoint>();
	public static UnityEvent onChange = new UnityEvent();
	[SerializeField] ParticleSystem system;

	private void Start()
	{
		allTheHouses.Add(this);
	}
	public void OnPressieCollect(ThirdPersonUserControl player)
	{

		system.gameObject.layer = LayerMask.NameToLayer("p"+player.playerNumber);
		system.Play();
	}

	public void OnPressieDrop()
	{
		system.Stop();
	}
	public void SatifyGoal()
	{
		allTheHouses.Remove(this);
		onChange.Invoke();
	}

	public SphereCollider GetCollider()
	{
		return GetComponent<SphereCollider>();
	}
}
