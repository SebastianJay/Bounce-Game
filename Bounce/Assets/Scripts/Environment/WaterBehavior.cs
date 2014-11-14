using UnityEngine;
using System.Collections;

public class WaterBehavior : MonoBehaviour {
	private Vector2 waterForce;
	private Vector2 entryVelocity;
	private Vector2 dampingForce = new Vector2(0, -900);

	void OnTriggerEnter2D(Collider2D c) {
		entryVelocity = c.gameObject.rigidbody2D.velocity;
		Debug.Log (c.gameObject.rigidbody2D.velocity.y);
		if (-entryVelocity.y > 7) {
						waterForce = new Vector2 (0, 450);
						dampingForce = new Vector2 (0, -1200);
				}
				else if (entryVelocity.y > 5 && -entryVelocity.y < 7)
						waterForce = new Vector2 (0, 300);
				else if (-entryVelocity.y > 3 && -entryVelocity.y < 5)
						waterForce = new Vector2 (0, 290);
				//else if (-entryVelocity.y < 3) {
				//		c.gameObject.rigidbody2D.velocity = new Vector2 (c.gameObject.rigidbody2D.velocity.x, 0);
				//}
				else
						waterForce = new Vector2 (0, 450);
		}

	void OnTriggerStay2D(Collider2D c) {
		if (c.gameObject.tag == "Player") {
			//if (!(-entryVelocity.y > 3))
			c.gameObject.rigidbody2D.AddForce(waterForce);
				}
		}

	void OnTriggerExit2D(Collider2D c) {
		if (c.gameObject.tag == "Player") {
			dampingForce = new Vector2(0, -900);
			c.gameObject.rigidbody2D.AddForce(dampingForce);
				}
		}
}
