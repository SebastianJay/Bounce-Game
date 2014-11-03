using UnityEngine;
using System.Collections;

public class Treadmill : MonoBehaviour {
	
	//public float sidewaysSpeed;
	public float sidewaysForce = 50f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionStay2D(Collision2D c) {
		if (c.gameObject.tag == "Player") {
			//c.gameObject.rigidbody2D.velocity = new Vector2(sidewaysSpeed, c.gameObject.rigidbody2D.velocity.y);
			c.gameObject.rigidbody2D.AddForce(new Vector2(sidewaysForce, 0));
		}
	}
	
}