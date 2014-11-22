using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {
	public Vector2 initialPoint;
	public Vector2 endPoint;
	public bool useCurrentStartPosition = false;
	public float moveTime = 5.0f;	//in seconds, from start to end
	public float pauseTime = 2.0f;	//in seconds, time at endpoints

	private float moveTimer = 0.0f;
	private float pauseTimer = 0.0f;
	private bool paused = false;
	private Vector2 mFrom;
	private Vector2 mTo;

	void Start() {
		if (useCurrentStartPosition)
		{
			//initialPoint = rigidbody2D.position;
			Vector2 moveVector = endPoint-initialPoint;
			mFrom = rigidbody2D.transform.position;
			mTo = mFrom+moveVector;

		}else
		{
			transform.position = new Vector3(initialPoint.x, initialPoint.y, 0);
			mFrom = initialPoint;
			mTo = endPoint;
		}
	}

	void Update () {
		if (paused)
		{
			pauseTimer += Time.deltaTime;
			if (pauseTimer >= pauseTime)
			{
				pauseTimer = 0.0f;
				paused = false;
			}
		}
		else
		{
			moveTimer += Time.deltaTime;
			if (moveTimer >= moveTime)
			{
				moveTimer = 0.0f;
				Vector2 temp = mFrom;
				mFrom = mTo;
				mTo = temp;
				paused = true;
			}
			float frac = moveTimer / moveTime;
			//rigidbody2D.MovePosition (Vector2.Lerp (mFrom, mTo, Mathf.Clamp (frac, 0.0f, 1.0f)));
			rigidbody2D.transform.position= new Vector3(Vector2.Lerp (mFrom, mTo, Mathf.Clamp (frac, 0.0f, 1.0f)).x,Vector2.Lerp (mFrom, mTo, Mathf.Clamp (frac, 0.0f, 1.0f)).y,0);
		}
	}

	///Alternate implementation
	/*
	public int platformSpeedHorizontal;
	public int platformSpeedVertical;
	private int distance = 5; //distance from initial point
	private float initialX; //x position it starts at
	private float initialY; //y position it starts at
	void Start() {

		//initializes platform with velocities entered by user
		initialX = rigidbody2D.position.x;
		initialY = rigidbody2D.position.y;
		rigidbody2D.velocity = new Vector2(platformSpeedHorizontal, platformSpeedVertical);
	}
	void Update () {
		//when the platform is moving right and reaches the specified distance, it turns around
		if (rigidbody2D.position.x > initialX &&(rigidbody2D.position.x - initialX) >= distance) {
			Vector2 reverseDirection = new Vector2(-rigidbody2D.velocity.x, rigidbody2D.velocity.y);
			rigidbody2D.velocity = reverseDirection;
		}

		//when the platform is moving left and reaches the specified distance, it turns around
		if (rigidbody2D.position.x < initialX &&(initialX - rigidbody2D.position.x) >= distance) {
			Vector2 reverseDirection = new Vector2(-rigidbody2D.velocity.x, rigidbody2D.velocity.y);
			rigidbody2D.velocity = reverseDirection;
		}
		//when the platform is moving up and reaches the specified distance, it turns around
		if (rigidbody2D.position.y > initialY &&(rigidbody2D.position.y - initialY) >= distance) {
			Vector2 reverseDirection = new Vector2(rigidbody2D.velocity.x, -rigidbody2D.velocity.y);
			rigidbody2D.velocity = reverseDirection;
		}
		
		//when the platform is moving down and reaches the specified distance, it turns around
		if (rigidbody2D.position.y < initialY &&(initialY - rigidbody2D.position.y) >= distance) {
			Vector2 reverseDirection = new Vector2(rigidbody2D.velocity.x, -rigidbody2D.velocity.y);
			rigidbody2D.velocity = reverseDirection;
		}
	}
	*/
}
