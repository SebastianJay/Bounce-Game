using UnityEngine;
using System.Collections;

public class WaterBehavior : MonoBehaviour {
	private Vector2 initialPlayerVector;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void onTriggerEnter2D(Collider2D c) {
		initialPlayerVector = new Vector2 (c.rigidbody.velocity.x, c.rigidbody.velocity.y);
		Debug.Log ("entering");
		}

	void onTriggerStay2D(Collider2D c) {
		Vector2 forceVector = new Vector2 (0, -c.rigidbody.velocity.y);
		c.rigidbody.AddForce (forceVector);
		Debug.Log ("still inside");
		}
}
