using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof (ThirdPersonCharacter))]
public class ThirdPersonUserControl : MonoBehaviour
{
	static public List<ThirdPersonUserControl> players = new List<ThirdPersonUserControl>();
	public int playerNumber;
	private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
	[SerializeField] private Transform m_Cam;                  // A reference to the main camera in the scenes transform
	private Vector3 m_CamForward;             // The current forward direction of the camera
	private Vector3 m_Move;
	private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
	[SerializeField] Transform pressiePoint;
	private Pressie pressie = null;
	private bool controlable = true;
    public UnityEvent onPointScore = new UnityEvent();
    public int score = 0;

	private void Start()
	{
		players.Add(this);
		// get the third person character ( this should never be null due to require component )
		m_Character = GetComponent<ThirdPersonCharacter>();
	}
	private void OnDestroy()
	{
		players.Remove(this);	
	}

	private void Update()
	{
		/*if (!m_Jump)
		{
			m_Jump = Input.GetAxis("Jump" + playerNumber) == -1;
		}*/
	}

	public float horizontalAxis, verticleAxis;
	public bool jump, drop;
	// Fixed update is called in sync with physics
	private void FixedUpdate()
    {
		if(controlable == false) { return; }
		// read inputs
		// if (playerNumber > 1) playerNumber = 1;
		float v, h;
		if (PhoneNetworkManager.manager.Connected)
		{
			h = horizontalAxis;
			v = verticleAxis;
			if (jump)
			{
				m_Jump = true;
			}
			if (drop && pressie != null)
			{
				DropPresent();
			}
		}
		else
		{

			h = Input.GetAxis("Horizontal" + playerNumber);
			v = Input.GetAxis("Vertical" + playerNumber);

			if (Input.GetAxis("Fire" + playerNumber) != 0)
			{
				Ray forward = new Ray(transform.position, transform.forward);
				RaycastHit hit;
				if (pressie != null)
				{
					// print("SAD");
					DropPresent();
				}
			}
		}	
		
        
        // calculate camera relative direction to move:
        m_CamForward = m_Cam.forward;//Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
		m_Move = v*m_CamForward + h*m_Cam.right;
        
		// pass all parameters to the character control script
		m_Character.Move(m_Move, false, m_Jump, v != 0);
		m_Jump = false;

		jump = false;
		drop = false;
	}
    void PickupPresent(Pressie pressie)
	{ 
		pressie.pickUp(this, pressiePoint);
		this.pressie = pressie;
		
		if (pressie.getHouse())
		{
			pressie.getHouse().OnPressieCollect(this);
		}
	}
	void DropPresent()
	{
		pressie.drop();
		if (pressie.getHouse())
		{
			pressie.getHouse().OnPressieDrop();
		}
		pressie = null;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "pressie" && pressie == null)
		{
			Pressie p = collision.gameObject.GetComponent<Pressie>();
			if (p.owner != null)
			{
				print("DROP IT!");
				p.owner.DropPresent();
			}

			PickupPresent(p);

		}
	}
	private void OnTriggerEnter(Collider other)
	{
		print("HIT " + other.gameObject);
		
		if (pressie != null && other.transform.position == pressie.getHouse().transform.position)
		{
			print("IS OUR GUY");
			Pressie thePressie = pressie;
			pressie.getHouse().SatifyGoal();
			DropPresent();
            score += 1;
            if (onPointScore != null) { onPointScore.Invoke(); }

            Destroy(thePressie.gameObject);
		}
	}

	internal void setAxis(float x, float y)
	{
		horizontalAxis = x;
		verticleAxis = y;
	}
}
