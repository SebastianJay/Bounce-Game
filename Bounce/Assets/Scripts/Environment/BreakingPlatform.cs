using UnityEngine;
using System.Collections;

public class BreakingPlatform : MonoBehaviour {

	public int timer = 20;
	private bool timerStarted = false;

	// Use this for initialization
	void Start () {

	}

	void Update () {
		if (this.timerStarted && this.timer > 0) {
			this.timer--;
		}
	}


	void OnCollisionEnter2D(Collision2D coll) 
	{
		if (coll.gameObject.tag == "Player") 
		{
			this.timerStarted = true;
		}
		if (this.timer <= 0) {
			this.gameObject.rigidbody2D.fixedAngle = false;
		}
	}
}
