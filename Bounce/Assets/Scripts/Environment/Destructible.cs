using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {
	// Threshold velocity value
	public float vThres = 5f;
	// Use this for initialization
	void OnCollisionEnter2D (Collision2D col){
		// Must be player
		if (col.gameObject.tag == "Player") {
			// Must match the necessary speed
						if (col.gameObject.rigidbody2D.velocity.magnitude >= vThres) {
				//Destroys the object
								Destroy (gameObject);
						}
				}
	}
}