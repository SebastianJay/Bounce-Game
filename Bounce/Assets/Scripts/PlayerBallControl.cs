using UnityEngine;
using System.Collections;

public class PlayerBallControl : MonoBehaviour {

	public float moveForce = 150f;			// Amount of force added to move the player left and right
	public float moveTorque = 150f; 
	public float stoppingForceMultiplier = 3f;
	public float maxSpeed = 10f;
	public float maxPlayerGeneratedSpeed = 5f;
	public float maxTorque = 100.0f;
	public float bounceForce = 0.2f;
	public float maxBounce = 0.8f;
	public float minBounce = 0.1f;
	public float jumpForce = 100f;

	public float bounciness = 0.5f;

	public float thresholdAngle = 60f;
	public float thresholdVelocity = 5f;
	public float groundedThresholdBonus = 5f;

	private float deformScaleChange = 0.4f;
	public float deformSpeed = 0.15f;
	public float deformScaleFactor = 0.01f;
	//private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.

	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void Update() {
		//Sprite sprite = GetComponent<SpriteRenderer>();
		//do line cast to ground
		//Transform groundVec = new Transform(transform.position.x,
		 //                               transform.position.y - transform.localScale.y * sprite.bounds.size.y,
		  //                              transform.position.z);

		//grounded = Physics2D.Linecast(transform.position, groundVec.position, 1 << LayerMask.NameToLayer("Ground"));  

	}
	
	
	private enum DeformationState{Deforming, Reforming,Reformed, Normal};

	private DeformationState dState = DeformationState.Normal;

	private Vector2 prevVelocity;
	private float prevAngularVelocity;
	private Vector2 outVelocity;
	private float outAngularVelocity;
	private GameObject scaleObject;
	bool wasGrounded;

	void OnCollisionEnter2D(Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts)
			Debug.DrawRay (contact.point, contact.normal, Color.white);
		foreach (ContactPoint2D contact in collision.contacts)
		{
			if (contact.normal.y > 0)
				grounded = true;

			float velocityToCheck = thresholdVelocity;
			//Determine if ball should deform
			if(wasGrounded)
				velocityToCheck += groundedThresholdBonus;

			if(Mathf.Abs(Vector2.Angle(contact.normal,Vector2.up)) <= thresholdAngle
			   && Mathf.Abs (Vector2.Dot (collision.relativeVelocity,contact.normal)) > velocityToCheck
			   && dState == DeformationState.Normal)
			{
				dState = DeformationState.Deforming;
				Vector2 collisionNormal = contact.normal;
				Vector2 originalVelocity = -prevVelocity;
				float originalAngularVelocity = prevAngularVelocity;
				this.rigidbody2D.isKinematic = true;//Disable normal physics
				scaleObject = new GameObject("ScaleObject");//Create a gameObject to scale the ball along an axis
				Quaternion rot = Quaternion.LookRotation(collisionNormal);//Rotate the gameObject so that z+ is towards the collision normal
				scaleObject.transform.rotation = rot;

				scaleObject.transform.position = contact.point;//Center the scaleObject at the contact point so that the ball scales from one end
				this.transform.parent.parent = scaleObject.transform; //Parent the ball to the scaleObject
				scaleChanged = 0;

				//Calculate the reflection of originalVelocity about the collision normal to get the ball's new velocity vector
				Vector2 reflection = (2 * Vector2.Dot(originalVelocity,collisionNormal) * collisionNormal - originalVelocity);
				//Create a normalized vector representing the difference between the reflection and the original velocity
				Vector2 bounceModifier = new Vector2(originalVelocity.x-reflection.x, -originalVelocity.y-reflection.y);
				bounceModifier.Normalize();
				
				//Set the rigidbody velocity to the reflection vector and modify it to make the collision inelastic
				outVelocity = reflection + bounceModifier/bounciness;
				//Decrease/Reverse the angular velocity based on whether the ball changed direction on the x axis
				float angleMod = 0.5f*Mathf.Sign(-originalVelocity.x)*Mathf.Sign (outVelocity.x);

				outAngularVelocity = originalAngularVelocity*angleMod;
				//Set the scaleChange based on collision force
				deformScaleChange = Mathf.Clamp(Mathf.Abs (Vector2.Dot (collision.relativeVelocity,contact.normal))*deformScaleFactor,0.2f,1);
				jumpBoosted = false;
				return;
			}
		}
		//Deform (collision);
		//ListenForJump (collision);
	}

	void OnCollisionStay2D(Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts)
			Debug.DrawRay (contact.point, contact.normal, Color.yellow);
		foreach (ContactPoint2D contact in collision.contacts)
		{
			if (contact.normal.y > 0)
				grounded = true;
		}
		//Reform ();
		ListenForJump (collision);
		//Deform (collision);
	}

	void OnCollisionExit2D(Collision2D collision) {
		//Reform ();
		//print ("exited");
	}

	private void ListenForJump(Collision2D collision) {
		float v = Input.GetAxis ("Vertical");
		foreach (ContactPoint2D contact in collision.contacts)
		{
			if (v > 0 && contact.normal.y > 0)
			{
				this.rigidbody2D.AddForce(Vector2.up * jumpForce);
				break;
			}
		}
	}
	
	private float scaleChanged = 0;
	private bool jumpBoosted = false;

	private bool checkJumpBoosted()
	{
		if(Input.GetButtonDown("Jump") && outVelocity.y != 0)
		{
			jumpBoosted = true;
		}else if(Input.GetButtonDown("Left") && outVelocity.x < 0)
		{
			jumpBoosted = true;
		}else if(Input.GetButtonDown("Right") && outVelocity.x > 0)
		{
			jumpBoosted = true;
		}

		return jumpBoosted;
	}

	void FixedUpdate () {

		if (grounded)
			wasGrounded = true;

		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		if(dState == DeformationState.Normal)
		{
			if(h * this.rigidbody2D.velocity.x < maxPlayerGeneratedSpeed)
			{
				this.rigidbody2D.AddForce(Vector2.right * h * moveForce);

				//Allow faster stopping
				if(Mathf.Sign(h) != Mathf.Sign (this.rigidbody2D.velocity.x))
				{
					this.rigidbody2D.AddForce(Vector2.right * h * Mathf.Abs(this.rigidbody2D.velocity.x)*stoppingForceMultiplier);
				}

				//Add torque based on direction
				if(Mathf.Abs (this.rigidbody2D.angularVelocity) < maxTorque)
				{
					this.rigidbody2D.AddTorque(-h * moveForce);
				}

			}
		
			if(Mathf.Abs(this.rigidbody2D.velocity.x) > maxSpeed)
				this.rigidbody2D.velocity = new Vector2(Mathf.Sign(this.rigidbody2D.velocity.x) * maxSpeed, this.rigidbody2D.velocity.y);

			/*if (v > 0 && grounded) {
				this.rigidbody2D.AddForce (Vector2.up * jumpForce);
			}*/

		}else if(dState == DeformationState.Deforming)
		{// Scale ball down along z axis of scale object at deformSpeed until it's scale has changed more than the intended change
			checkJumpBoosted();

			if(scaleChanged < deformScaleChange)
			{
				scaleObject.transform.localScale = new Vector3(scaleObject.transform.localScale.x,scaleObject.transform.localScale.y,scaleObject.transform.localScale.z-deformSpeed);
				scaleChanged += deformSpeed;
			}else{
				dState = DeformationState.Reforming;
				scaleChanged = 0;
			}


		}else if(dState == DeformationState.Reforming)
		{//Scale ball up along z axis of scale object at deformSpeed until it's scale has changed more than the intended change
			checkJumpBoosted();

			if(scaleChanged < deformScaleChange)
			{
				scaleObject.transform.localScale = new Vector3(scaleObject.transform.localScale.x,scaleObject.transform.localScale.y,scaleObject.transform.localScale.z+deformSpeed);
				scaleChanged += deformSpeed;

			}else{
				//Unparent from scaleObject and destroy scaleObject
				this.transform.parent.parent = null;
				GameObject.Destroy (scaleObject);
				
				this.rigidbody2D.isKinematic = false;//Re-enabled rigidbody physics
				if(jumpBoosted)
				{
					outVelocity = outVelocity+outVelocity*bounciness;

					if(outVelocity.magnitude < (jumpForce*2.5f/this.rigidbody2D.mass)*Time.fixedDeltaTime)
					{
						outVelocity.Normalize();
						outVelocity*=(jumpForce*2.5f/this.rigidbody2D.mass)*Time.fixedDeltaTime;
					}
				}

				this.rigidbody2D.angularVelocity = outAngularVelocity;
				this.rigidbody2D.velocity = outVelocity;
				dState = DeformationState.Reformed;
			}
		}else if(dState == DeformationState.Reformed)
		{//Allow an extra tick before the ball is allowed to deform again
			dState = DeformationState.Normal;
		}


		grounded = false;
		prevVelocity = this.rigidbody2D.velocity;
		prevAngularVelocity = this.rigidbody2D.angularVelocity;
	
		//float v = Input.GetAxis ("Vertical");

		/*
		this.collider2D.sharedMaterial.bounciness += v * bounceForce;
		if (this.collider2D.sharedMaterial.bounciness > maxBounce)
			this.collider2D.sharedMaterial.bounciness = maxBounce;
		if (this.collider2D.sharedMaterial.bounciness < minBounce)
			this.collider2D.sharedMaterial.bounciness = minBounce;
		this.collider2D.enabled = false;
		this.collider2D.enabled = true;
		//print(this.collider2D.sharedMaterial.bounciness);
		*/
		/*
		if (v * this.rigidbody2D.velocity.y < maxSpeed)
			this.rigidbody2D.AddForce(Vector2.up * v * moveForce);
		if(Mathf.Abs(this.rigidbody2D.velocity.y) > maxSpeed)
			this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, Mathf.Sign(this.rigidbody2D.velocity.y) * maxSpeed);
		*/
	}
}
