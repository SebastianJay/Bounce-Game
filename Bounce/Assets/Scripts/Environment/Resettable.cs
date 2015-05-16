using UnityEngine;
using System.Collections;

public class Resettable : MonoBehaviour {
	// original position values
	private float posX;
	private float posY;
	private float posZ;

	// original rotation values
	private float rotW;
	private float rotX;
	private float rotY;
	private float rotZ;

	
	private Transform transformRef;

	// Use this for initialization
	void Start () {
		// set original position values
		posX = this.transform.position.x;
		posY = this.transform.position.y;
		posZ = this.transform.position.z;

		rotW = this.transform.rotation.w;
		rotX = this.transform.rotation.x;
		rotY = this.transform.rotation.y;
		rotZ = this.transform.rotation.z;

		transformRef = this.gameObject.transform;
	}

	// calls 
	public void Reset() {
	
		ResetPosition ();
		if (GetComponent<HingeJoint2D>() != null)
			ResetHingeJoint ();
		if (GetComponent<Collider2D>() != null)
			ResetCollider ();
		if (rigidbody2D != null)
			ResetRigidbody ();
		if (GetComponent<MovingPlatform>() != null){
			GetComponent<MovingPlatform>().Reset();
		}
		else if (transform.childCount > 0 && transform.GetChild(0).GetComponent<MovingPlatform>() != null){
			transform.GetChild(0).GetComponent<MovingPlatform>().Reset();
		}
		if (GetComponent<BreakingPlatform>() != null) {
			GetComponent<BreakingPlatform>().Reset();
		}
	}

	void ResetPosition () {
		transformRef.position = new Vector3(posX, posY, posZ);
		transformRef.rotation = new Quaternion(rotX, rotY, rotZ, rotW);
		//velocity = new Vector3 (0f, 0f, 0f);
	}

	void ResetCollider() {
		this.gameObject.GetComponent<Collider2D>().enabled = true;
	}

	void ResetHingeJoint () {
		this.gameObject.GetComponent<HingeJoint2D>().enabled = true;
	}

	void ResetRigidbody () {
		this.gameObject.rigidbody2D.fixedAngle = true;
	}
}
