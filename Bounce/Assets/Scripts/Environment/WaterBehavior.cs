using UnityEngine;
using System.Collections;

public class WaterBehavior : MonoBehaviour {
	private Vector2 waterForce = new Vector2(0, 450);
	private Vector2 entryVelocity;
	private Vector2 dampingForce = new Vector2(0, -1300);

	void OnTriggerEnter2D(Collider2D c) {
		entryVelocity = c.gameObject.rigidbody2D.velocity;
		Debug.Log (c.gameObject.rigidbody2D.velocity.y);
		if (-entryVelocity.y < 4) {					
						dampingForce.y = -1000;
						waterForce.y = 400;
						c.gameObject.rigidbody2D.AddForce (waterForce);
				} else if (-entryVelocity.y < 3 && -entryVelocity.y > 0) {
			c.gameObject.rigidbody2D.velocity = new Vector2(c.gameObject.rigidbody2D.velocity.x, 0);
				} else {
						c.gameObject.rigidbody2D.AddForce (waterForce);
				}

		}

	void OnTriggerStay2D(Collider2D c) {
		if (c.gameObject.tag == "Player") { 
						c.gameObject.rigidbody2D.AddForce (waterForce);
			if (-entryVelocity.y < 3 && -entryVelocity.y > 0) {
								c.gameObject.rigidbody2D.velocity = new Vector2 (c.gameObject.rigidbody2D.velocity.x, 0);
						}
				}
		}

	void OnTriggerExit2D(Collider2D c) {
		if (c.gameObject.tag == "Player") 
			c.gameObject.rigidbody2D.AddForce(dampingForce);
				
		}
}
