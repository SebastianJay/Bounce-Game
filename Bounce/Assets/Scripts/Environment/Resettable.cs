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
	
	// Update is called once per frame
	void Update () {
	
	}

	// calls 
	public void Reset() {
	
		ResetPosition ();
		ResetHingeJoint ();
		ResetCollider ();
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
		try {
			this.gameObject.GetComponent<Collider2D>().enabled = true;
		} catch ( MissingComponentException ) {
			Debug.Log ("Resettable: Missing component: Collider2D");

		}
	}

	void ResetHingeJoint () {
		try {
			this.gameObject.GetComponent<HingeJoint2D>().enabled = true;
		} catch ( MissingComponentException ) {
			Debug.Log ("Resettable: Missing component: HingeJoint2D");
		}
	}

	void ResetRigidbody () {
		if (rigidbody2D != null)
			this.gameObject.rigidbody2D.fixedAngle = true;
	}
}
