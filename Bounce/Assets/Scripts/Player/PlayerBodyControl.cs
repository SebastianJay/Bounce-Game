using UnityEngine;
using System.Collections;

public class PlayerBodyControl : MonoBehaviour {

	public float moveVelocity = 15f;
	public float jumpForce = 5000f;
	public float jumpDelay = 0.4f;
	public float groundedThresholdAngle = 45f;

	private bool grounded = false;
	private float jumpTimer = 0f;

	// Use this for initialization
	void Start () {
		
	}

	void OnCollisionEnter2D(Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts) { 
			if (Mathf.Abs(Vector2.Angle(Vector2.up, contact.normal)) < groundedThresholdAngle)
				grounded = true;
		}
	}

	void OnCollisionStay2D(Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts) { 
			if (Mathf.Abs(Vector2.Angle(Vector2.up, contact.normal)) < groundedThresholdAngle)
				grounded = true;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		float h = Input.GetAxis ("Horizontal");
		rigidbody2D.velocity = new Vector2 (moveVelocity * h, rigidbody2D.velocity.y);

		if (Input.GetButton("Jump") && grounded && jumpTimer >= jumpDelay) {
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
			rigidbody2D.AddForce(new Vector2(0f, jumpForce));
			jumpTimer = 0f;
		}

		jumpTimer += Time.deltaTime;
		grounded = false;
	}
}
