using UnityEngine;
using System.Collections;

public class PlayerBallControl : MonoBehaviour {

	// Physics for lateral movement
	public float moveForce = 75f;			// Amount of force for translational movement
	public float moveTorque = 100f; 		// Amount of force for rotation
	public float translationStoppingMultiplier = 4f;	// Multiplier for translational movement opposite velocity
	public float rotationStoppingMultiplier = 3f;
	public float maxPlayerGeneratedSpeed = 10f;
	public float maxAngularVelocity = 100.0f;
	// Jumping, Boosting
	public float jumpForce = 2800f;
	public float jumpDelay = 0.4f;	//time (in s) delay between jumps
	private float jumpTimer = 0.4f;
	public float boostForgiveness = 0.75f; //Extra time in seconds the player has to get a boosted bounce
	private float timeSinceLeft = 0.0f;
	private float timeSinceRight = 0.0f;
	private float timeSinceJump = 0.0f;
	// bouncy vars
	public float bounciness = 0.225f;
	public float boostedBounciness = 0.9f;
	public float boostThresholdVelocity = 6f;
	public float boostThresholdAngle = 360f;
	// Grounded vars
	private bool grounded = false;			// Whether or not the player is grounded.
	public float groundedThresholdAngle = 45f;
	public float groundedThresholdBonus = 4f;
	//Deformation variables
	private float deformScaleChange = 0.4f;
	public float deformScaleFactor = 0.015f;	//multiplier that determines how deformed ball will get
	public float deformTimeFactor = 0.001f;	//multiplier that determines how quick deformation will be
	[HideInInspector]
	public float deformTime = 0.15f;
	private float deformTimer = 0.0f;
	private float baseScale = 0.0f;	//temp
	private enum DeformationState{Deforming, Reforming,Reformed, Normal};
	private DeformationState dState = DeformationState.Normal;
	//Talking, speech vars
	[HideInInspector]
	public bool isTalking = false;
	[HideInInspector]
	public Interactable npcTalker;
	// Temp Storage Vars
	private Vector2 prevVelocity;
	private float prevAngularVelocity;
	private Vector2 outVelocity;
	private float outAngularVelocity;
	private GameObject scaleObject;
	private bool wasGrounded;
	private float originalMagnitude;

	public int infoOne = 3;
	public int infoTwo = 4;
	public char infoThree = 'b';

	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void Update() {
		recordButtonDelay();
	}

	void OnCollisionEnter2D(Collision2D collision) {


		foreach (ContactPoint2D contact in collision.contacts)
			Debug.DrawRay (contact.point, contact.normal, Color.white);
		foreach (ContactPoint2D contact in collision.contacts)
		{

			if (Mathf.Abs(Vector2.Angle(Vector2.up, contact.normal)) < groundedThresholdAngle)
				grounded = true;

			float velocityToCheck = boostThresholdVelocity;
			//Determine if ball should deform
			if(wasGrounded)
				velocityToCheck += groundedThresholdBonus;

			if(Mathf.Abs(Vector2.Angle(contact.normal,Vector2.up)) <= boostThresholdAngle
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
				deformTimer = 0.0f;

				originalMagnitude = originalVelocity.magnitude;
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
				deformScaleChange = Mathf.Clamp(Mathf.Abs (Vector2.Dot (collision.relativeVelocity,contact.normal))*deformScaleFactor,0.2f, 0.8f);
				deformTime = Mathf.Clamp(Mathf.Abs (Vector2.Dot (collision.relativeVelocity, contact.normal)) * deformTimeFactor, 0.02f, 0.08f);
				return;
			}
		}
	}

	void OnCollisionStay2D(Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts)
			Debug.DrawRay (contact.point, contact.normal, Color.yellow);
		foreach (ContactPoint2D contact in collision.contacts)
		{
			if (Mathf.Abs(Vector2.Angle(Vector2.up, contact.normal)) < groundedThresholdAngle)
				grounded = true;
		}
		ListenForJump ();
	}

	void OnCollisionExit2D(Collision2D collision) {
	}

	private void ListenForJump() {
		float v = Input.GetAxis ("Vertical");
		if (v > 0 && grounded && jumpTimer >= jumpDelay)
		{
			this.rigidbody2D.AddForce(Vector2.up * jumpForce);
			jumpTimer = 0.0f;
		}
	}

	private bool jumpBoosted = false;
	private void checkJumpBoosted()
	{

		if((Input.GetButtonDown("Jump") || timeSinceJump < boostForgiveness) && outVelocity.y != 0)
		{
			jumpBoosted = true;
		}else if((Input.GetButtonDown("Left") || timeSinceLeft < boostForgiveness) && outVelocity.x < 0)
		{
			jumpBoosted = true;
		}else if((Input.GetButtonDown("Right") || timeSinceRight < boostForgiveness) && outVelocity.x > 0)
		{
			jumpBoosted = true;
		}
	}

	private void recordButtonDelay()
	{
		timeSinceJump += Time.fixedDeltaTime;
		timeSinceLeft += Time.fixedDeltaTime;
		timeSinceRight += Time.fixedDeltaTime;
		
		if(Input.GetButtonDown("Jump"))
			timeSinceJump = 0;
		if(Input.GetButtonDown("Left"))
			timeSinceLeft = 0;
		if(Input.GetButtonDown("Right"))
			timeSinceRight = 0;
	}

	void FixedUpdate () {

		if (grounded)
			wasGrounded = true;

		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		if(dState == DeformationState.Normal)
		{
			if(h != 0)
			{
				recordButtonDelay();
				//translational movement
				if(Mathf.Sign(h) != Mathf.Sign (this.rigidbody2D.velocity.x))
				{
					this.rigidbody2D.AddForce(Vector2.right * h * moveForce * translationStoppingMultiplier);
				}
				else if (Mathf.Abs(this.rigidbody2D.velocity.x) < maxPlayerGeneratedSpeed)
				{
					this.rigidbody2D.AddForce(Vector2.right * h * moveForce);
				}

				//rotational movement
				if (Mathf.Sign (h) == Mathf.Sign (rigidbody2D.angularVelocity))
				{
					this.rigidbody2D.AddTorque(-h * moveTorque * rotationStoppingMultiplier);
				}
				else if(Mathf.Abs (this.rigidbody2D.angularVelocity) < maxAngularVelocity)
				{
					this.rigidbody2D.AddTorque(-h * moveTorque);
				}
			}		
		}else if(dState == DeformationState.Deforming)
		{// Scale ball down along z axis of scale object at deformSpeed until it's scale has changed more than the intended change
			recordButtonDelay();
			checkJumpBoosted();

			if(deformTimer < deformTime)
			{
				deformTimer += Time.deltaTime;
				float compressScale = deformScaleChange * Mathf.Clamp (deformTimer / deformTime, 0.0f, 1.0f);
				scaleObject.transform.localScale = new Vector3(scaleObject.transform.localScale.x,scaleObject.transform.localScale.y,1.0f - compressScale);
			}else{
				dState = DeformationState.Reforming;
				deformTimer = 0.0f;
				baseScale = scaleObject.transform.localScale.z;
			}


		}else if(dState == DeformationState.Reforming)
		{//Scale ball up along z axis of scale object at deformSpeed until it's scale has changed more than the intended change

			recordButtonDelay();
			checkJumpBoosted();

			if(deformTimer < deformTime)
			{
				deformTimer += Time.deltaTime;
				float compressScale = deformScaleChange * Mathf.Clamp (deformTimer / deformTime, 0.0f, 1.0f);
				scaleObject.transform.localScale = new Vector3(scaleObject.transform.localScale.x,scaleObject.transform.localScale.y,baseScale + compressScale);
			}else{
				//Unparent from scaleObject and destroy scaleObject
				this.transform.parent.parent = null;
				GameObject.Destroy (scaleObject);
				
				this.rigidbody2D.isKinematic = false;//Re-enabled rigidbody physics
				if(jumpBoosted)
				{
					jumpBoosted = false;
					timeSinceJump = boostForgiveness;
					timeSinceLeft = boostForgiveness;
					timeSinceRight = boostForgiveness;

					outVelocity = outVelocity.normalized*originalMagnitude*boostedBounciness;

					if(outVelocity.magnitude < (jumpForce/this.rigidbody2D.mass)*Time.fixedDeltaTime)
					{
						outVelocity.Normalize();
						outVelocity*=(jumpForce/this.rigidbody2D.mass)*Time.fixedDeltaTime;
					}

				}

				this.rigidbody2D.angularVelocity = outAngularVelocity;
				this.rigidbody2D.velocity = outVelocity;
				dState = DeformationState.Reformed;
			}
		}else if(dState == DeformationState.Reformed)
		{//Allow an extra tick before the ball is allowed to deform again
			recordButtonDelay();
			checkJumpBoosted();
			dState = DeformationState.Normal;
		}


		grounded = false;
		jumpTimer += Time.fixedDeltaTime;
		prevVelocity = this.rigidbody2D.velocity;
		prevAngularVelocity = this.rigidbody2D.angularVelocity;
	}
}
