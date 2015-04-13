using UnityEngine;
using System.Collections;

// The platform must have a HingeJoint2D, a Collider2D, and a RigidBody2D

public class BreakingPlatform : MonoBehaviour {

	private int timer = 0;
	private bool timerStarted = false;
	public int breakTime = 50;
	public int fallTime = 80;

	// Use this for initialization
	void Start () {
		this.gameObject.rigidbody2D.fixedAngle = true;
	}

	void Update () {
		if (this.timerStarted && this.timer >= 0) {
			this.timer++;
		} 
		if (this.timer >= this.breakTime) {
			this.gameObject.rigidbody2D.fixedAngle = false;
		}
		// IF A CRAZY BUG SHOWS UP HERE, it's caused by dying between the breakTime and fallTime
		if (this.timer >= this.fallTime) {

			try {
				//this.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
			} catch ( MissingComponentException) {
			}
			try {
				this.gameObject.GetComponent<HingeJoint2D>().enabled = false;
			} catch ( MissingComponentException ) {
			}
			try {
				this.gameObject.GetComponent<Collider2D>().enabled = false;
			} catch ( MissingComponentException ) {
			}
			this.timerStarted = false;
			this.timer = 0;

		}
	}


	void OnCollisionEnter2D(Collision2D coll) 
	{
		if (coll.gameObject.tag == "Player") 
		{
			this.timerStarted = true;
		}

	}
}
