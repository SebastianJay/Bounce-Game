using UnityEngine;
using System.Collections;

// 

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
		if (this.timer >= this.fallTime) {
			this.gameObject.GetComponent<HingeJoint2D>().enabled = false;
			this.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
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
