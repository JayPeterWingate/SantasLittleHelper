using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
	[SerializeField] Transform target;
	[SerializeField] float offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
		transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime);
		transform.GetChild(0).LookAt(target.position + new Vector3(0, offset, 0));
    }
}
