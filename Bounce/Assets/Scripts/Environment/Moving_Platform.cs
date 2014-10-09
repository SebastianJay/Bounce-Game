using UnityEngine;
using System.Collections;

public class Moving_Platform : MonoBehaviour {
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

}
