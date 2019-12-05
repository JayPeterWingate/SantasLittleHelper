using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaScript : MonoBehaviour
{
	[SerializeField] Animator anim;
	[SerializeField] Rigidbody rigid;
	[SerializeField] CapsuleCollider collide;
	[SerializeField] float speed;
	[SerializeField] float rotateSpeed;
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		float v = Input.GetAxis("Vertical");
		float h = Input.GetAxis("Horizontal");

		anim.SetFloat("Speed", v);

		rigid.AddForce(v * speed * rigid.transform.forward);
		rigid.AddTorque(new Vector3(0, h * rotateSpeed, 0));

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 1, 0), Time.deltaTime);


	}
}
