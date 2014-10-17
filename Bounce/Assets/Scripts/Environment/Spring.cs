using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour {
	public float springForce = 4000f;
	
	private float orientation;
	private Vector2 direction;
	
	void Awake() {
		orientation = (this.gameObject.transform.rotation.eulerAngles.z);
		direction = new Vector2(- Mathf.Sin(orientation * Mathf.PI / 180), Mathf.Cos(orientation * Mathf.PI / 180));
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player")
			coll.gameObject.rigidbody2D.AddForce (springForce * direction);
	}
}