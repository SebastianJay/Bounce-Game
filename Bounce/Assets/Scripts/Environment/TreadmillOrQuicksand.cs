using UnityEngine;
using System.Collections;

public class TreadmillOrQuicksand : MonoBehaviour {
	
	public float downwardSpeed;
	public float sidewaysSpeed;

	/* To use as quicksand, you must check the 'Use as trigger' box
	 * within the environment object's collider that you are trying to make quicksand*/

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//called for quicksand
	void OnTriggerStay2D(Collider2D c) {
		if (c.tag == "Player") {
			Debug.Log ("in dis");
			c.gameObject.rigidbody2D.velocity = new Vector2(sidewaysSpeed, -downwardSpeed);
			
		}
	}

	//called for treadmill
	void OnCollisionStay2D(Collision2D c) {
		if (c.gameObject.tag == "Player") {
			Debug.Log("in dis too");
			c.gameObject.rigidbody2D.velocity = new Vector2(sidewaysSpeed, c.gameObject.rigidbody2D.velocity.y);
		}
	}
	
}