using UnityEngine;
using System.Collections;

public class PlayerBallControl : MonoBehaviour {

	public bool playerLock =false;

	// Physics for lateral movement
	public float moveForce = 75f;			// Amount of force for translational movement
	public float moveTorque = 100f; 		// Amount of force for rotation
	public float translationStoppingMultiplier = 4f;	// Multiplier for translational movement opposite velocity
	public float rotationStoppingMultiplier = 3f;
	public float maxPlayerGeneratedSpeed = 10f;
	public float maxAngularVelocity = 100.0f;
	public float frictionCoefficient = 0.2f;		//percent magnitude of normal for friction force
	public float frictionThresholdVelocity = 0.3f;	//threshold above which to apply friction
	// Jumping, Boosting
	public float jumpForce = 2800f;
	public float jumpDelay = 0.4f;	//time (in s) delay between jumps
	private float jumpTimer = 0.4f;
	[HideInInspector]
	public int jumpFrame = 0;	//used so spiderball doesn't repeat jump
	[HideInInspector]
	public int springFrame = 0;
	// bouncy vars
	public float bounciness = 0.6f;
	public float boostedBounciness = 0.85f;
	public float depressedBounciness = 0.4f;
	public float boostThresholdVelocity = 6f;
	public float boostThresholdAngle = 360f;
	// Grounded vars
	private bool grounded = false;			// Whether or not the player is grounded.
	private bool hasContact = false;		// Whether the player is touching something
	public float groundedThresholdAngle = 45f;
	public float groundedThresholdBonus = 4f;
	//Deformation variables
	private float deformScaleChange = 0.4f;
	public float deformScaleFactor = 0.015f;	//multiplier that determines how deformed ball will get
	public float deformTimeFactor = 0.001f;	//multiplier that determines how quick deformation will be
	[HideInInspector]
	public float deformTime = 0.15f;
	private float deformTimer = 0.0f;
	private Vector3 originalScale; 
	private Vector3 baseScale;	//temp
	private enum DeformationState{Deforming, Reforming,Reformed, Normal};
	private DeformationState dState = DeformationState.Normal;
	//Talking, speech vars
	[HideInInspector]
	public bool inConversation = false;
	[HideInInspector]
	public Transform npcTalker;

	// Temp Storage Vars
	private float originalMagnitude;
	private Vector2 prevVelocity;
	private float prevAngularVelocity;
	private Vector2 outVelocity;
	private float outAngularVelocity;
	private GameObject scaleObject;
	private bool wasGrounded;

	//moving platform detection vars
	private bool onMovingPlatform = false;
	[HideInInspector]
	public bool spiderOnMovingPlatform = false;
	private Transform platformParent;

	public bool spiderball = false;
	public bool balloonActive = false;

	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void Update() {
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.GetComponent<MovingPlatform>() != null
		    && col.GetComponent<MovingPlatform>().reparentPlayer) 
		{
			onMovingPlatform = true;
			platformParent = col.transform.parent;
		}
	}
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.GetComponent<MovingPlatform>() != null) 
		{
			onMovingPlatform = false;
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {


		foreach (ContactPoint2D contact in collision.contacts)
			Debug.DrawRay (contact.point, contact.normal, Color.white);
		foreach (ContactPoint2D contact in collision.contacts)
		{
			if (collision.gameObject.GetComponent<Spring>() != null) {
				//must initiate from here to avoid some race condition
				collision.gameObject.GetComponent<Spring>().SpringCollide(gameObject);
				return;
			}
			if (collision.gameObject.GetComponent<Treadmill>() != null)
				return;
			if (collision.gameObject.GetComponent<Death>() != null)
				return;

			//more hacky code to get spiderball with moving platform working
			if (spiderball && (collision.collider.GetComponent<MovingPlatform>() != null ||
			                   (collision.collider.transform.childCount > 0 && 
			 				    collision.collider.transform.GetChild(0).GetComponent<MovingPlatform>() != null)))
			{
				spiderOnMovingPlatform = true;
				platformParent = collision.collider.transform;
			}

			if (Mathf.Abs(Vector2.Angle(-Physics2D.gravity, contact.normal)) < groundedThresholdAngle)
				grounded = true;

			//Determine if ball should deform
			float velocityToCheck = boostThresholdVelocity;
			if(wasGrounded)
				velocityToCheck += groundedThresholdBonus;

			if(Mathf.Abs(Vector2.Angle(contact.normal,-Physics2D.gravity)) <= boostThresholdAngle
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
	
				//Debug.Log ("deforming");
				//deforming onto a moving platform logic - in this case, the scale object should be child of platform
				//if (collision.gameObject.GetComponent<MovingPlatform>() != null)
				//	scaleObject.transform.parent = collision.gameObject.transform;
				if ((onMovingPlatform && Vector2.Angle(contact.normal, -Physics2D.gravity) <= 5f) || spiderOnMovingPlatform)	//odd angles cause screwiness
				{
					//Debug.Log ("reparenting scale object");
					scaleObject.transform.parent = platformParent;
				}
				else if (collision.gameObject.transform.childCount > 0
				         && collision.gameObject.transform.GetChild(0).GetComponent<MovingPlatform>() != null
						   && Vector2.Angle(contact.normal, -Physics2D.gravity) <= 2f)	//super-hard-coding
				{
					//this is cheating a bit - relying on a straight collision with a horizontal platform
					//we probably won't have any exeptional cases in the game, though, so it should be fine..(?)
					scaleObject.transform.parent = collision.gameObject.transform;
				}

				//Parent the ball to the scaleObject
				transform.parent.parent = scaleObject.transform;
				originalScale = scaleObject.transform.localScale;
				deformTimer = 0.0f;

				originalMagnitude = originalVelocity.magnitude;
				//Calculate the reflection of originalVelocity about the collision normal to get the ball's new velocity vector
				Vector2 reflection = (2 * Vector2.Dot(originalVelocity,collisionNormal) * collisionNormal - originalVelocity);
				//Create a normalized vector representing the difference between the reflection and the original velocity
				//Vector2 bounceModifier = new Vector2(originalVelocity.x-reflection.x, -originalVelocity.y-reflection.y);
				//bounceModifier.Normalize();
				
				//Set the rigidbody velocity to the reflection vector and modify it to make the collision inelastic
				//outVelocity = reflection + bounceModifier/bounciness;
				outVelocity = reflection * bounciness;

				//Decrease/Reverse the angular velocity based on whether the ball changed direction on the x axis
				float angleMod = 0.5f*Mathf.Sign(-originalVelocity.x)*Mathf.Sign (outVelocity.x);

				outAngularVelocity = originalAngularVelocity*angleMod;
				//Set the scaleChange based on collision force
				deformScaleChange = Mathf.Clamp(Mathf.Abs (Vector2.Dot (collision.relativeVelocity,contact.normal))*deformScaleFactor,0.2f, 0.8f);
				deformTime = Mathf.Clamp(Mathf.Abs (Vector2.Dot (collision.relativeVelocity, contact.normal)) * deformTimeFactor, 0.02f, 0.08f);
				return;
			}
			//else if (collision.gameObject.GetComponent<MovingPlatform>() != null)
			//	transform.parent.parent = collision.gameObject.transform;	//if we aren't deforming, just do normal parenting
			//else if (onMovingPlatform)
			//	transform.parent.parent = platformParent;	//if we aren't deforming, just do normal parenting

		}
		hasContact = true;
	}

	void OnCollisionStay2D(Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts)
			Debug.DrawRay (contact.point, contact.normal, Color.yellow);
		foreach (ContactPoint2D contact in collision.contacts)
		{
			if (Mathf.Abs(Vector2.Angle(- Physics2D.gravity, contact.normal)) < groundedThresholdAngle)
				grounded = true;
		}

		foreach (ContactPoint2D contact in collision.contacts)
		{
			if (rigidbody2D.velocity.magnitude > frictionThresholdVelocity)
			{
				Vector2 frictionDir = -(rigidbody2D.velocity.normalized);
				float frictionMag = contact.normal.magnitude * frictionCoefficient;
				Vector2 frictionVec = frictionDir * frictionMag;
				//maybe some safeguards against wall climbing using the normal here
				rigidbody2D.AddForce(frictionVec);
			}
		}
		if (Mathf.Abs(rigidbody2D.velocity.x) < frictionThresholdVelocity
		    && Input.GetAxis("Horizontal") == 0)
		{
			rigidbody2D.velocity = new Vector2(0f, rigidbody2D.velocity.y);
			//aimed to bring motion to a complete stop
		}
		hasContact = true;

	}

	void OnCollisionExit2D(Collision2D collision) {
		hasContact = false;
		//if (spiderball && spiderOnMovingPlatform)
		//	spiderOnMovingPlatform = false;
		if (!onMovingPlatform
		    && dState == DeformationState.Normal
		    && transform.parent.parent != null)
			transform.parent.parent = null;	//get off the platform
	}

	private void ListenForJump() {
		if (Input.GetButton("Jump") && grounded && jumpTimer >= jumpDelay && !spiderball && !balloonActive
		    && Time.frameCount - springFrame > Spring.springJumpFrameThreshold)
		{
			//Debug.Log ("Jump");
			//Debug.Log (rigidbody2D.velocity);
			//DEBUG
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);	//gets rid of the "extra high jump"
			//
			this.rigidbody2D.AddForce(-Physics2D.gravity.normalized * jumpForce);
			jumpTimer = 0.0f;
			hasContact = false;
			jumpFrame = Time.frameCount;
			if (GetComponent<PowerupManager>().currentPowerup == PowerupType.SuperJump)
				GetComponent<PlayerSoundManager>().PlaySound("SuperJump");
			else
				GetComponent<PlayerSoundManager>().PlaySound("Jump");
		}
		//else
		//	jumpFrame = 
	}

	private bool jumpBoosted = false;
	private bool jumpDepressed = false;
	private void checkJumpBoosted()
	{
		//if (springFrame)
		//	return;	//ignore boosts if you hit a spring
		if(((Input.GetButton("Jump") /*|| timeSinceJump < boostForgiveness*/) && outVelocity.y > 0))
		//   ((Input.GetButtonDown("Left") /*|| timeSinceLeft < boostForgiveness*/) && outVelocity.x < 0) ||
		//   ((Input.GetButtonDown("Right") /*|| timeSinceRight < boostForgiveness*/) && outVelocity.x > 0))
		{
			jumpBoosted = true;
		}
		else if(((Input.GetButton("Floor") /*|| timeSinceJump < boostForgiveness*/) && outVelocity.y > 0))
		//		((Input.GetButtonDown("Left") /*|| timeSinceLeft < boostForgiveness*/) && outVelocity.x > 0) ||
		//		((Input.GetButtonDown("Right") /*|| timeSinceRight < boostForgiveness*/) && outVelocity.x < 0))
		{
			jumpDepressed = true;
		}
	}

	/*
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
	*/

	void FixedUpdate () {

		if (grounded)
			wasGrounded = true;
		float h = Input.GetAxis ("Horizontal");

		//locks the player
		if (playerLock == true) {
			h = 0;
			jumpTimer = 0;
		}

		if(dState == DeformationState.Normal)
		{
			ListenForJump ();
			if(h != 0)
			{
				//recordButtonDelay();
				//translational movement
				if (!spiderball && !balloonActive) {
					Vector2 relativeRight = new Vector2(-Physics2D.gravity.y, Physics2D.gravity.x).normalized;
					if(Mathf.Sign(h) != Mathf.Sign (this.rigidbody2D.velocity.x))
					{
						this.rigidbody2D.AddForce(relativeRight * h * moveForce * translationStoppingMultiplier);
					}
					else if (Mathf.Abs(this.rigidbody2D.velocity.x) < maxPlayerGeneratedSpeed)
					{
						this.rigidbody2D.AddForce(relativeRight * h * moveForce);
					}
				}
				//rotational movement
				if (Mathf.Sign (h) == Mathf.Sign (rigidbody2D.angularVelocity) && !hasContact)
				{
					this.rigidbody2D.AddTorque(-h * moveTorque * rotationStoppingMultiplier);
				}
				else if(Mathf.Abs (this.rigidbody2D.angularVelocity) < maxAngularVelocity && !hasContact)
				{
					this.rigidbody2D.AddTorque(-h * moveTorque);
				}
			}		
		}
		else if(dState == DeformationState.Deforming)
		{// Scale ball down along z axis of scale object at deformSpeed until it's scale has changed more than the intended change
			//recordButtonDelay();
			checkJumpBoosted();

			if(deformTimer < deformTime)
			{
				deformTimer += Time.deltaTime;
				float compressScale = deformScaleChange * Mathf.Clamp (deformTimer / deformTime, 0.0f, 1.0f);
				//scaleObject.transform.localScale = new Vector3(scaleObject.transform.localScale.x,
				//                                               scaleObject.transform.localScale.y,
				//                                               1.0f - compressScale);
				scaleObject.transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z * (1.0f - compressScale));
			}else{
				dState = DeformationState.Reforming;
				deformTimer = 0.0f;
				//baseScale = scaleObject.transform.localScale.z;
				baseScale = scaleObject.transform.localScale;
			}
		}
		else if(dState == DeformationState.Reforming)
		{//Scale ball up along z axis of scale object at deformSpeed until it's scale has changed more than the intended change

			//recordButtonDelay();
			checkJumpBoosted();
			if(deformTimer < deformTime)
			{
				deformTimer += Time.deltaTime;
				float compressScale = deformScaleChange * Mathf.Clamp (deformTimer / deformTime, 0.0f, 1.0f);
				//scaleObject.transform.localScale = new Vector3(scaleObject.transform.localScale.x,
				//                                               scaleObject.transform.localScale.y,
				//                                               baseScale + compressScale);
				scaleObject.transform.localScale = baseScale + new Vector3(0f, 0f, originalScale.z * compressScale);
			}else{
				//Unparent from scaleObject and destroy scaleObject
				this.transform.parent.parent = null;
				GameObject.Destroy (scaleObject);
				this.transform.parent.localScale = Vector3.one;

				this.rigidbody2D.isKinematic = false;//Re-enabled rigidbody physics
				if(jumpBoosted)
				{
					jumpBoosted = false;
					jumpFrame = Time.frameCount;
					//timeSinceJump = boostForgiveness;
					//timeSinceLeft = boostForgiveness;
					//timeSinceRight = boostForgiveness;
					//Debug.Log ("Boosted bounce");
					outVelocity = outVelocity.normalized*originalMagnitude*boostedBounciness;

					if(outVelocity.magnitude < (jumpForce/this.rigidbody2D.mass)*Time.fixedDeltaTime)
					{
						//Debug.Log("Back to original height");
						outVelocity.Normalize();
						outVelocity*=(jumpForce/this.rigidbody2D.mass)*Time.fixedDeltaTime;
					}
				}
				if(jumpDepressed)
				{
					jumpDepressed = false;
					outVelocity = outVelocity.normalized*originalMagnitude*depressedBounciness;
				}
				if (GetComponent<Spiderball>().enabled)
				{
					outVelocity = Vector2.zero;	//super sticky!
				}

				this.rigidbody2D.angularVelocity = outAngularVelocity;
				this.rigidbody2D.velocity = outVelocity;
				dState = DeformationState.Reformed;
				GetComponent<PlayerSoundManager>().PlaySound("Bounce");
			}
		}
		else if(dState == DeformationState.Reformed)
		{//Allow an extra tick before the ball is allowed to deform again
			dState = DeformationState.Normal;
		}

		if (!spiderball && spiderOnMovingPlatform)
		{
			spiderOnMovingPlatform = false;
		}
		if ((onMovingPlatform || spiderOnMovingPlatform) && transform.parent.parent == null)
			transform.parent.parent = platformParent;
		else if (!onMovingPlatform && !spiderOnMovingPlatform && platformParent != null)
		{
			transform.parent.parent = null;
			platformParent = null;
		}

		grounded = false;
		jumpTimer += Time.fixedDeltaTime;
		prevVelocity = this.rigidbody2D.velocity;
		prevAngularVelocity = this.rigidbody2D.angularVelocity;
	}

	public void ForceUndoDeformation()
	{
		if (dState == DeformationState.Deforming || dState == DeformationState.Reforming)
		{
			this.transform.parent.parent = null;
			GameObject.Destroy (scaleObject);
			this.transform.parent.localScale = Vector3.one;
			this.rigidbody2D.isKinematic = false;
		}
		dState = DeformationState.Normal;
	}
}
