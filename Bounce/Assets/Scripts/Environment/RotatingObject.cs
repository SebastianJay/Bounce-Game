using UnityEngine;
using System.Collections;

public class RotatingObject : MonoBehaviour {

	public float rotationalVelocity = 10f;

	// Use this for initialization
	void Start () {
		if (rigidbody2D != null)
			rigidbody2D.angularVelocity = rotationalVelocity;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//if (rigidbody2D != null)
		//	rigidbody2D.MoveRotation (rigidbody2D.rotation + rotationalVelocity * Time.deltaTime);

		if (rigidbody2D != null)
			rigidbody2D.angularVelocity = rotationalVelocity;
		else
			transform.RotateAround(transform.position, new Vector3(0f, 0f, 1f), rotationalVelocity*Time.deltaTime);
	}
}
