using UnityEngine;
using System.Collections;

public class Moving_Platform : MonoBehaviour {
	
	bool way = false;  //platform direction
	public int platformSpeedHorizontal;
	public int platformSpeedVertical;

	void Start() {
		//initializes platform with velocities entered by user
		rigidbody2D.velocity = new Vector2(platformSpeedHorizontal, platformSpeedVertical);
		}



	void Update () {

	}
}
