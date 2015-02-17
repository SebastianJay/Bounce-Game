using UnityEngine;
using System.Collections;

//for the slow fall powerup
public class Parachuter : MonoBehaviour {

	//vars for animation of a parachute

	//physics vars
	public float dampingCoefficient = -20f;
	public float maxDampingForce = 100f;	//for y-component only

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//stuff with animation
		Vector2 velocity = rigidbody2D.velocity;
		if (velocity.y < 0) {
			Vector2 dampingForce = new Vector2(0f, velocity.y * dampingCoefficient);
			if (dampingForce.sqrMagnitude > maxDampingForce * maxDampingForce)
				dampingForce = dampingForce.normalized * maxDampingForce;
			rigidbody2D.AddForce(dampingForce);
			Debug.Log (dampingForce);
		}
	}

	public void ForceQuit() {

	}
}
