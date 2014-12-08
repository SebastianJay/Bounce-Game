using UnityEngine;
using System.Collections;

public class Spiderball : MonoBehaviour {

	public float spiderballMoveForce = 400f;
	public float maxPlayerGeneratedSpeed = 10f;
	public float stickiness = 5f;
	public float stickyTimeout = 10f; // how many frames after a collision he'll maintain a con
	public float dampingRatio = 1f; // how damped are his oscillations
	public float jointDistance = 0.5f;
	public float jumpDelay = 0.3f;	//time (in s) delay between jumps
									//not sure why multiple jumps occur, but this'll be a stop-gap
	private float jumpTimer = 0.3f;

	SpringJoint2D joint;
	public Collision2D lastCollision = new Collision2D();

	private float originalGravity; 	//player's starting gravity (it will be changed during spiderball process)
	private PlayerBallControl pbc;	//reference for ease of access
	private int collisionInst = 0;	//number of collisions in a frame
	private ContactPoint2D otherCollide; //used for handling multi-collision cases

	private bool isConnected = false;
	private int framesSinceDisconnected = 0;

	void Start () {
		originalGravity = this.rigidbody2D.gravityScale;
		pbc = GetComponent<PlayerBallControl>();
	}

	void FixedUpdate () {

		//Lateral movement
		if (isConnected) {
			pbc.spiderball = true;

			float h = Input.GetAxis ("Horizontal");
			if (h != 0 && !pbc.playerLock) {

				Vector2 relativeRight = new Vector2 ();
				relativeRight.x = lastCollision.contacts[0].normal.y;
				relativeRight.y = -lastCollision.contacts[0].normal.x;
				//Debug.Log(relativeRight);

				float currentSpeed = rigidbody2D.velocity.magnitude;
				//float currentSpeedRelativeRight = currentSpeed * Mathf.Cos (Mathf.Atan2 (relativeRight.y, relativeRight.x));

				//Debug.Log (currentSpeed);
				//Debug.Log (currentSpeedRelativeRight);

				if (currentSpeed < this.maxPlayerGeneratedSpeed) {
					this.rigidbody2D.AddForce(relativeRight.normalized * Mathf.Sign(h)  * spiderballMoveForce);
				}
			}
		}

		//Jumping
		jumpTimer += Time.deltaTime;
		if (Input.GetButton ("Jump") && isConnected
		    && jumpTimer >= jumpDelay
		    && !pbc.jumpedInCurrentFrame
		    && !pbc.playerLock) {

			Destroy (joint);
			//joint.enabled = false;
			isConnected = false;
			framesSinceDisconnected = 0;
			jumpTimer = 0.0f;

			this.rigidbody2D.AddForce(lastCollision.contacts[0].normal.normalized * pbc.jumpForce);
		}

		// Detecting whether spiderball physics still apply
		if (!isConnected) {

			if (++framesSinceDisconnected > stickyTimeout) {
				if (joint != null) {
					Destroy (joint);
					//joint.enabled = false;
					framesSinceDisconnected = 0;
					//Debug.Log ("Destroying joint here");
				}
				pbc.spiderball = false;
			}
		} else {
			pbc.spiderball = true;
		}
		if (joint != null && joint.enabled) {
			this.rigidbody2D.gravityScale = 0;
		} else {
			this.rigidbody2D.gravityScale = originalGravity;
		}

		//Reset the collision instance counter every frame
		collisionInst = 0;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (enabled)
			SpiderCollisionCheck(collision);
	}

	void OnCollisionStay2D(Collision2D collision) {
		if (enabled)
			SpiderCollisionCheck(collision);
	}

	void OnCollisionExit2D(Collision2D collision) {
		if (enabled)
			lastCollision = collision;
	}

	void SpiderCollisionCheck(Collision2D collision)
	{
		if (collision.gameObject.GetComponent<OneWay>() != null
		    || collision.gameObject.GetComponent<Spring>() != null)
		{	//avoids the weird behavior of certain objects by ignoring them
			//crawling on a spring or one-way platform should be undefined anyway
			ForceQuit();
			return;
		}
		collisionInst++;
		float h = Input.GetAxis("Horizontal");
		if (collisionInst == 1 || shouldShiftJoint(h, collision.contacts[0], otherCollide))
		{
			if (collisionInst == 1)
				otherCollide = collision.contacts[0];

			lastCollision = collision;
			if (joint == null && !isConnected) {
				joint = gameObject.AddComponent("SpringJoint2D") as SpringJoint2D;
				isConnected = true;
				framesSinceDisconnected = 0;

				joint.distance = jointDistance;

				joint.anchor = new Vector2(0f,0f);
				joint.connectedAnchor = collision.contacts[0].point;
				joint.collideConnected = true;
				joint.dampingRatio = dampingRatio;
				joint.frequency = stickiness;

			} else if (joint != null) {
				joint.enabled = true;
				joint.anchor = new Vector2(0f,0f);
				joint.connectedAnchor = collision.contacts[0].point;
				framesSinceDisconnected = 0;
			}
		}
		if (joint != null)
			Debug.DrawLine(new Vector3(joint.connectedAnchor.x, joint.connectedAnchor.y, 0f), transform.position, Color.red);
	}

	// Used when two collision events fire on the same frame (haven't tested > 2)
	//  determines whether this contact is the desirable one, based on player movement
	// h - direction of horizontal input (from Input.GetAxis)
	// contact - the collision point to test
	// other - the other collision point detected in the same frame
	bool shouldShiftJoint(float h, ContactPoint2D contact, ContactPoint2D other)
	{
		return (h > 0 && ((contact.point.x > transform.position.x && other.normal.y > 0) ||
		              	  (contact.point.x < transform.position.x && other.normal.y < 0) ||
		                  (contact.point.y < transform.position.y && other.normal.x > 0) ||
		                  (contact.point.y > transform.position.y && other.normal.x < 0)))
			|| (h < 0 && ((contact.point.x < transform.position.x && other.normal.y > 0) ||
			              (contact.point.x > transform.position.x && other.normal.y < 0) ||
			              (contact.point.y > transform.position.y && other.normal.x > 0) ||
			              (contact.point.y < transform.position.y && other.normal.x < 0)));
	}

	// Called when we want to disenable the spiderball mechanic
	// to take care of resetting all the physical properties of the character
	public void ForceQuit()
	{
		GetComponent<PlayerBallControl>().spiderball = false;
		isConnected = false;
		rigidbody2D.gravityScale = originalGravity;
		if (joint != null)
			Destroy (joint);
	}
}
