using UnityEngine;
using System.Collections;

//for the slow fall powerup
public class Parachuter : MonoBehaviour {

	//vars for animation of a parachute

	//physics vars
	/*
	public float dampingCoefficient = -20f;
	public float maxDampingForce = 100f;	//for y-component only
	public float dampingDebug = 30f;
	public float velocityThreshDebug = -2f;

	public float startForce = 10f;
	public float endForce = 20f;
	public float transitionTime = 3f;
	*/

	//public float gravityScale = 0.6f;
	public float gravityFloor = 0.25f;
	public float gravityCeil = 0.05f;
	public float gravityTime = 2.0f;

	private float originalScale = 1.0f;
	private float currentGravity;
	private float timer = 0f;

	// Use this for initialization
	void Start () {
		originalScale = rigidbody2D.gravityScale;
	}
	
	// Update is called once per frame
	void Update () {
		//stuff with animation
		Vector2 velocity = rigidbody2D.velocity;
		if (velocity.y < 0f) {
			timer += Time.deltaTime;
			currentGravity = Mathf.Clamp (Mathf.Lerp(gravityFloor, gravityCeil, (timer*timer)/(gravityTime*gravityTime)), 
			                              gravityCeil, gravityFloor);
			rigidbody2D.gravityScale = currentGravity;

			//Vector2 dampingForce = new Vector2(0f, Mathf.Lerp(startForce, endForce, Mathf.Clamp(timer/transitionTime, 0f, 1f)));
			//Vector2 dampingForce = new Vector2(0f, dampingDebug);
			//Vector2 dampingForce = new Vector2(0f, velocity.y * dampingCoefficient);
			//if (dampingForce.sqrMagnitude > maxDampingForce * maxDampingForce)
			//	dampingForce = dampingForce.normalized * maxDampingForce;
			//rigidbody2D.AddForce(dampingForce);
			//Debug.Log (velocity);
			//Debug.Log (dampingForce);
		} else {
			rigidbody2D.gravityScale = originalScale;
			currentGravity = gravityFloor;
			timer = 0f;
		}
	}

	public void ForceQuit() {
		timer = 0f;
		currentGravity = gravityFloor;
		rigidbody2D.gravityScale = originalScale;
	}
}
