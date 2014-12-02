﻿using UnityEngine;
using System.Collections;

public class Spiderball : MonoBehaviour {

	public float spiderballMoveForce = 400f;
	public float maxPlayerGeneratedSpeed = 10f;
	public float stickiness = 5f;
	public float stickyTimeout = 10f; // how many frames after a collision he'll maintain a con
	public float dampingRatio = 1f; // how damped are his oscillations
	public float jumpDelay = 0.3f;	//time (in s) delay between jumps
									//not sure why multiple jumps occur, but this'll be a stop-gap
	private float jumpTimer = 0.3f;

	public bool activated = false;
	SpringJoint2D joint;

	private float gravity; 
	private bool jumpOnNextFrame = false;


	public Collision2D lastCollision = new Collision2D();

	private bool isConnected = false;
	private int framesSinceDisconnected = 0;

	void Start () {
		gravity = this.rigidbody2D.gravityScale;
	}

	void Update () {
	}

	void FixedUpdate () {

		if (isConnected && activated) {
			this.GetComponent<PlayerBallControl>().spiderball = true;

			float h = Input.GetAxis ("Horizontal");
				
			if (h != 0) {

				Vector2 relativeRight = new Vector2 ();
				relativeRight.x = lastCollision.contacts[0].normal.y;
				relativeRight.y = -lastCollision.contacts[0].normal.x;
				//Debug.Log(relativeRight);

				float currentSpeed = rigidbody2D.velocity.magnitude;
				float currentSpeedRelativeRight = currentSpeed * Mathf.Cos (Mathf.Atan2 (relativeRight.y, relativeRight.x));

				//Debug.Log (currentSpeed);
				//Debug.Log (currentSpeedRelativeRight);

				if (currentSpeed < this.maxPlayerGeneratedSpeed) {
					this.rigidbody2D.AddForce(relativeRight.normalized * h  * spiderballMoveForce);
				}
			}
		}

		if (activated) {
			if (!isConnected) {
				
				if (++framesSinceDisconnected > stickyTimeout) {
					if (joint != null) {
						Destroy (joint);
						framesSinceDisconnected = 0;
						
					}
					gameObject.GetComponent <PlayerBallControl>().spiderball = false;
				}
			} else if (activated) {
				gameObject.GetComponent<PlayerBallControl>().spiderball = true;
			}
		}
		
		jumpTimer += Time.deltaTime;
		if (Input.GetButton ("Jump") && isConnected
		    && jumpTimer >= jumpDelay) {
			
			Destroy (joint);
			isConnected = false;
			jumpOnNextFrame = true;
			framesSinceDisconnected = 0;
			jumpTimer = 0.0f;
		}
		
		if (jumpOnNextFrame) {
			this.rigidbody2D.AddForce(lastCollision.contacts[0].normal.normalized * this.GetComponent<PlayerBallControl>().jumpForce);
			jumpOnNextFrame = false;
		}
		
		if (isConnected) {
			this.rigidbody2D.gravityScale = 0;
		} else {
			this.rigidbody2D.gravityScale = gravity;
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (activated) {
			lastCollision = collision;
			if (joint == null && !isConnected) {
				joint = gameObject.AddComponent("SpringJoint2D") as SpringJoint2D;
				isConnected = true;
				framesSinceDisconnected = 0;
				
				joint.distance = 0.1f;

				joint.anchor = new Vector2(0f,0f);
				joint.connectedAnchor = collision.contacts[0].point;
				joint.collideConnected = true;
				joint.dampingRatio = dampingRatio;
				joint.frequency = stickiness;

			} else if (joint != null) {
				joint.anchor = new Vector2(0f,0f);
				joint.connectedAnchor = collision.contacts[0].point;


			}

		} else {
			Destroy (joint);
		}
		if (joint != null)
			Debug.DrawLine(new Vector3(joint.connectedAnchor.x, joint.connectedAnchor.y, 0f), transform.position, Color.red);
	}

	void OnCollisionStay2D(Collision2D collision) {
		lastCollision = collision;
		if (activated && joint != null) {
			joint.anchor = new Vector2(0f,0f);
			joint.connectedAnchor = collision.contacts[0].point;
		} else if (joint == null && activated) {
			joint = gameObject.AddComponent("SpringJoint2D") as SpringJoint2D;
			isConnected = true;
			framesSinceDisconnected = 0;
			
			joint.distance = 0.1f;
			
			joint.anchor = new Vector2(0f,0f);
			joint.connectedAnchor = collision.contacts[0].point;
			joint.collideConnected = true;
			joint.dampingRatio = dampingRatio;
			joint.frequency = stickiness;
		} else  {
			Destroy (joint);
		}
		if (joint != null)
			Debug.DrawLine(new Vector3(joint.connectedAnchor.x, joint.connectedAnchor.y, 0f), transform.position, Color.magenta);
	}

	void OnCollisionExit2D(Collision2D collision) {
		lastCollision = collision;
		isConnected = false;

	}

}