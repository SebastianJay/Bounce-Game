using UnityEngine;
using System.Collections;

public class PlayerBodyControl : MonoBehaviour {

	public float moveVelocity = 15f;
	public float jumpForce = 5000f;
	public float jumpDelay = 0.4f;
	public float groundedThresholdAngle = 45f;
	public float wallHugThresholdAngle = 30f;

	private bool grounded = false;
	private float wallHug = 0f;
	private float jumpTimer = 0f;

	//Talking, speech vars
	[HideInInspector]
	public bool inConversation = false;
	[HideInInspector]
	public Transform npcTalker;
	public bool playerLock = false;
	
	// Use this for initialization
	void Start () {
		
	}

	void OnCollisionEnter2D(Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts) { 
			if (Mathf.Abs(Vector2.Angle(Vector2.up, contact.normal)) < groundedThresholdAngle)
				grounded = true;
			if (Mathf.Abs(Vector2.Angle(Vector2.right, contact.normal)) < wallHugThresholdAngle)
				wallHug = Mathf.Sign(contact.normal.x);
			if (Mathf.Abs(Vector2.Angle(-Vector2.right, contact.normal)) < wallHugThresholdAngle)
				wallHug = Mathf.Sign(contact.normal.x);

		}
	}

	void OnCollisionStay2D(Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts) { 
			if (Mathf.Abs(Vector2.Angle(Vector2.up, contact.normal)) < groundedThresholdAngle)
				grounded = true;
			if (Mathf.Abs(Vector2.Angle(Vector2.right, contact.normal)) < wallHugThresholdAngle)
				wallHug = Mathf.Sign(contact.normal.x);
			if (Mathf.Abs(Vector2.Angle(-Vector2.right, contact.normal)) < wallHugThresholdAngle)
				wallHug = Mathf.Sign(contact.normal.x);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (!playerLock) {
			float h = Input.GetAxis ("Horizontal");
			if (wallHug == 0f || Mathf.Sign(h) == wallHug)
				rigidbody2D.velocity = new Vector2 (moveVelocity * h, rigidbody2D.velocity.y);

			if (Input.GetButton("Jump") && grounded && jumpTimer >= jumpDelay) {
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
				rigidbody2D.AddForce(new Vector2(0f, jumpForce));
				jumpTimer = 0f;
			}
		}

		jumpTimer += Time.deltaTime;
		grounded = false;
		wallHug = 0f;
	}
}
