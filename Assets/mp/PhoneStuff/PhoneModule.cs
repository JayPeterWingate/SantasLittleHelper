using FullbeansNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PhoneModule : MonoBehaviour {
	
	public BlockTypes m_type;
	[HideInInspector] public int m_index;
    protected ModuleIconManager m_iconManager;
    protected bool m_locked = false;
    protected float m_lastUsed;
	protected int m_ownerIndex;
	
    protected void Parent_Start()
    {
        m_iconManager = GetComponent<ModuleIconManager>();
        m_lastUsed = Time.time - GetModuleData().usageWaitPeriod;
    }

    public void SetPlayerOwner(PlayerData player)
    {
        if (player != null)
        {
			m_ownerIndex = player.index;

			m_iconManager.SetPlayerSymbol(UISystem.Instance.GetPlayerIconSprites()[(int)player.icon], player.color);
        }
        else
        {
            m_iconManager.SetEmptyPlayerSymbol();
        }
    }

    public void SetLock(bool setting)
    {
        m_locked = setting;
        m_iconManager.SetLocked(setting);

    }

    public bool IsCurrentlyLocked()
    {
        return m_locked;
    }

    public bool IsCurrentlyActive()
    {
        return (!m_locked && m_lastUsed <= Time.time - GetModuleData().usageWaitPeriod);
    }

    public void ResetTimeLastUsed()
    {
        m_lastUsed = Time.time;
    }

    protected void Parent_Update()
    {
        if (IsCurrentlyActive())
        {
            m_iconManager.SetChargeBar(1);
        }
        else
        {
            if (IsCurrentlyLocked())
            {
                m_iconManager.SetChargeBar(0);
            }
            else
            {
                m_iconManager.SetChargeBar((Time.time - m_lastUsed) / GetModuleData().usageWaitPeriod);
            }
        }
    }

	virtual public void RecieveMessage (string message) {
		Debug.Log("I am " + m_index + ". My recieved message was: " + message);
	}



	virtual public ModuleData GetModuleData()
	{
		return new ModuleData();
	}

	abstract public ModuleData GetDefaultData();

    virtual public string GetDebugDesc()
    {
        return m_index + "-" + m_type;
    }

	internal int getId()
	{
		return m_index;
	}
}
