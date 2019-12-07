using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.5f;
		[SerializeField] Animator anim;
		[SerializeField] Rigidbody rigid;
		[SerializeField] CapsuleCollider collide;
 		Rigidbody m_Rigidbody;
		Animator m_Animator;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;

		void Start()
		{
			m_Animator = anim;
			m_Rigidbody = rigid;
			m_Capsule = collide;
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;
			m_Animator.applyRootMotion = false;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}


		public void Move(Vector3 move, bool crouch, bool jump,bool forwards)
		{

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation();
            
			if (forwards)
				m_Rigidbody.velocity = transform.forward * m_MoveSpeedMultiplier + m_Rigidbody.velocity;
            // control and velocity handling is different when grounded and airborne:
            
            if (m_IsGrounded)
			{
				HandleGroundedMovement(crouch, jump);
				
			}
			else
			{
				HandleAirborneMovement();
			}

			// ScaleCapsuleForCrouching(crouch);
			// PreventStandingInLowHeadroom();
            
            // send input and other state parameters to the animator
            UpdateAnimator(move, jump);
		}


		void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
				}
			}
		}
		public void Pickup()
		{
			m_Animator.SetBool("Pickup", true);
		}

        float lastMove = 0.0f, lastTurn = 0.0f;
        bool lastGrounded = false;
        int speedHash = Animator.StringToHash("Speed");
        int turnHash = Animator.StringToHash("Turn");
        int onGroundHash = Animator.StringToHash("OnGround");
        void UpdateAnimator(Vector3 move, bool jump)
		{
			// update the animator parameters
            if(Mathf.Abs(lastMove - m_ForwardAmount) > 0.5f)
            {
                m_Animator.SetFloat(speedHash, m_ForwardAmount);
                lastMove = m_ForwardAmount;
            }
			if(Mathf.Abs(lastTurn - m_TurnAmount) > 0.5f)
            {
                m_Animator.SetFloat(turnHash, m_TurnAmount);
                lastTurn = m_TurnAmount;
            }
			/*
            if(lastGrounded != m_IsGrounded)
            {
                m_Animator.SetBool(onGroundHash, m_Crouching);
                lastGrounded = m_IsGrounded;
            }*/
            
			if (jump)
			{
				m_Animator.SetBool("Jump", true);
			}
            
			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(
					m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
			

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}


		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump)
			{
				// jump!
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_GroundCheckDistance = 0.1f;
			}
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			//if (m_IsGrounded && Time.deltaTime > 0)
			//{
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;
			//}
		}


		void CheckGroundStatus()
		{

            m_IsGrounded = true;// - 1f < m_Rigidbody.velocity.y && m_Rigidbody.velocity.y < 1f;
			
		}
	}
}
