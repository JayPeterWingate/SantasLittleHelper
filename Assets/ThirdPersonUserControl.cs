using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof (ThirdPersonCharacter))]
public class ThirdPersonUserControl : MonoBehaviour
{
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

		// get the third person character ( this should never be null due to require component )
		m_Character = GetComponent<ThirdPersonCharacter>();
	}


	private void Update()
	{
		if (!m_Jump)
		{
			m_Jump = Input.GetAxis("Jump" + playerNumber) == -1;
		}
	}


	// Fixed update is called in sync with physics
	private void FixedUpdate()
	{
		if(controlable == false) { return; }
		// read inputs
		float h = Input.GetAxis("Horizontal"+ playerNumber);
		float v = Input.GetAxis("Vertical" + playerNumber);
		bool crouch = Input.GetKey(KeyCode.C);

		if (Input.GetAxis("Fire" + playerNumber) != 0)
		{
			Ray forward = new Ray(transform.position, transform.forward);
			RaycastHit hit;
			print("FIRE");
			if ( pressie != null)
			{
				print("SAD");
				DropPresent();
			}
			else if (Physics.Raycast(forward, out hit, 1.0f) && hit.collider.tag == "pressie")
			{
				PickupPresent(hit.collider.GetComponent<Pressie>());
			}

			
		}

		// calculate camera relative direction to move:
		m_CamForward = m_Cam.forward;//Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
		m_Move = v*m_CamForward + h*m_Cam.right;

		// pass all parameters to the character control script
		m_Character.Move(m_Move, crouch, m_Jump, v != 0);
		m_Jump = false;
	}
	void PickupPresent(Pressie pressie)
	{ 
		pressie.pickUp(pressiePoint);
		this.pressie = pressie;
		m_Character.Pickup();

		StartCoroutine(MakePresentDroppable());
		if (pressie.getHouse())
		{
			pressie.getHouse().OnPressieCollect(this);
		}
		controlable = false;
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
	IEnumerator MakePresentDroppable()
	{
		yield return new WaitForSeconds(1f);
		controlable = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		print("HIT "+ other.gameObject);
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
}
