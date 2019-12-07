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
		public void AddModule(PhoneModule module)
		{
			module.SetPlayerOwner(this);
			data.Add(module.getId());
		}
		/// <summary>
		/// Replaces the module list of the player
		/// </summary>
		/// <param name="modules"> An array of Phone modules to replace currentlist</param>
		public void ReplaceModules(List<PhoneModule> modules)
		{
			data = new List<int>();
			for (int i = 0; i < modules.Count; i++)
			{
				AddModule(modules[i]);
			}
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
		public void ScrambleModules(List<PhoneModule> a_modules)
		{
            if (PlayerCount == 0)
            {
                return;
            }

			a_modules = HelperFunctions.RandomiseList(a_modules);
			List<PhoneModule>[] moduleData = new List<PhoneModule>[PlayerCount];
            for (int i = 0; i < PlayerCount; i++)
            {
                moduleData[i] = new List<PhoneModule>();
            }

            for (int i = 0; i < a_modules.Count; i++)
			{
				PhoneModule data = a_modules[i];
				moduleData[(int)(i % PlayerCount)].Add(data);
			}

            List<PlayerData> tempPlayerStorage = new List<PlayerData>();
            // Update Players Module lists
            int tempPlayerCount = PlayerCount;
            for (int i = 0; i < tempPlayerCount; i++)
			{
				PlayerData currentPlayer = m_players.Keys[0];
				m_players.RemoveAt(0); // did you say update order? 
				currentPlayer.ReplaceModules(moduleData[i]);
                tempPlayerStorage.Add(currentPlayer);
                //m_players.Add(currentPlayer, false);
            }
            for (int i = 0; i < tempPlayerStorage.Count; i++)
            {
                PlayerData currentPlayer = tempPlayerStorage[i];
                m_players.Add(currentPlayer, false);
            }
			m_isFirstScramble = true;
			SendModuleData(a_modules);
		}

		public void AddPlayer(PlayerData a_playerData, List<PhoneModule> a_modules)
		{
			if (a_modules != null && a_modules.Count > 0)
            {

				if(PlayerCount != 0)
				{
					var requiredModulesPerPlayer = Math.Ceiling(a_modules.Count / (((double)PlayerCount) + 1));
					List<int> modules = new List<int>();

					while (modules.Count != requiredModulesPerPlayer)
					{
						var playerWithMostModules = m_players.Keys[PlayerCount - 1];
						m_players.RemoveAt(PlayerCount - 1); // To update key of sorted Lists you just gotta remove and insert
																 // Give the last module to new guy 
						modules.Add(playerWithMostModules.data[playerWithMostModules.data.Count - 1]);
						// Strip module from player
						playerWithMostModules.data.RemoveAt(playerWithMostModules.data.Count - 1);
						// Reinsert playerWithMostModules back into the player pool
						m_players.Add(playerWithMostModules, false);

					}

					a_playerData.ReplaceModules(a_modules.Where(m => modules.Contains(m.getId())).ToList());

				} else // If no players, just give them all modules
				{
					a_playerData.ReplaceModules(new List<PhoneModule>(HelperFunctions.RandomiseList(a_modules)));
				}
			} 
			m_players.Add(a_playerData, false);

			if(a_modules != null && a_modules.Count > 0)
            {
				SendModuleData(a_modules);
			}
		}

		public void RemovePlayer(int id, List<PhoneModule> a_modules)
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
			if (a_modules != null && a_modules.Count > 0 && PlayerCount > 0) {

				List<int> modules = removedPlayer.data;
				for (int i = 0; i < modules.Count; i++)
				{
					var playerWithFewestModules = m_players.Keys[0]; // This is gross but it is best with what we got
					m_players.Remove(playerWithFewestModules); // To update key of sorted Lists you just gotta remove and insert
					playerWithFewestModules.AddModule(a_modules.Find(m => m.getId() == modules[i])); // Would use sorted sets but not in Mono :'(
					m_players.Add(playerWithFewestModules, false);
				}
				
				SendModuleData(a_modules);
			}
            //Debug.Log(m_players.Count + " End suffering");
        }

        private string DebugGetReadableModuleList(ModuleData[] moduleData)
        {
            string outString = "";
            for (int i = 0; i < moduleData.Length; i++)
            {
                outString += moduleData[i].id + "-" + moduleData[i].type + "-" + moduleData[i].rect + ", ";
            }
            return outString;
        }

		private void SendModuleData(List<PhoneModule> modules)
		{
			// Rebuild list 
			for (int i = 0; i < Players.Count; i++)
			{
				//Debug.Log(m_players.Keys[i].data);
				ScrambleModulesMessage currentSMM = new ScrambleModulesMessage();
				currentSMM.id = m_players.Keys[i].index;
				currentSMM.moduleData = modules.Select(m => m.GetModuleData()).ToArray();
				currentSMM.totalModuleCount = modules.Count;
				currentSMM.isFirst = m_isFirstScramble;
				//Debug.Log("Sending " + m_players.Keys[i].name + ": " + DebugGetReadableModuleList(currentSMM.moduleData));
				//Debug.Log(JsonUtility.ToJson(currentSMM));
				//Debug.Log(JsonUtility.ToJson(m_players.Keys[i].data.ToArray()));

				OperationManager.Instance.Network.SendPhoneMessage(currentSMM);
			}
			m_isFirstScramble = false;
		}
	}
}

