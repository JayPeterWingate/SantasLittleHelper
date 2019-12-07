using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FullbeansNetworking{

	public class PlayerData: IComparable<PlayerData>
	{
		public int index;
		public List<int> data;
		public string name;
		public Color color;
		public PhoneUserIcon icon;

		public int CompareTo(PlayerData other)
		{
			if(data == null)
			{
				return -1;
			}
			if(other.data == null)
			{
				return 1;
			}
			if(this.index == other.index)
			{
				return 0;
			}
			int v = data.Count - other.data.Count;
			return v != 0? v : 1;
		}
		
	}

	public class PhonePlayerController
	{
		private SortedList<PlayerData, bool> m_players; // # Regrets with Sorted list, is garbage
		public List<PlayerData> Players { get { return new List<PlayerData>(m_players.Keys); } }
		public int PlayerCount { get { return m_players.Count; } }
		public bool m_isFirstScramble = true;

		public PhonePlayerController()
		{
			m_players = new SortedList<PlayerData,bool>();
		}
		

		public void AddPlayer(PlayerData a_playerData)
		{
			m_players.Add(a_playerData, false);
		}

		public void RemovePlayer(int id)
		{
            //Debug.Log(m_players.Count + " Sadness");
			PlayerData removedPlayer = m_players.First(data => data.Key.index == id).Key;
			int playerIndex = 0;
			for(int i = 0; i < m_players.Count; i++)
			{
				if (m_players.Keys[i].index == removedPlayer.index) // I fucking hate these sorted lists
				{
					playerIndex = i;
				}
			}
			m_players.RemoveAt(playerIndex);
        }
	}
}

