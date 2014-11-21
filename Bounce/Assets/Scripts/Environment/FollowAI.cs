using UnityEngine;
using System.Collections;

public class FollowAI : MonoBehaviour {

	public float speed = 1.0f;
	public float minRange = 3;
	public float maxRange = 15;
	public float maxSpeed = 5.0f;

	private GameObject player;
	private Transform target;

	void Start() {
		player = GameObject.FindGameObjectWithTag ("Player");
		target = player.transform;
	}
	
	void FixedUpdate() {
		Debug.DrawLine (target.position, transform.position, Color.yellow);
		if (Mathf.Abs (transform.position.x - player.transform.position.x) < maxRange && 
		    Mathf.Abs (transform.position.x - player.transform.position.x) > minRange) 
		{
			if (speed < maxSpeed) {
				speed += 0.1f;
			}		
			if (player.transform.position.x < transform.position.x) {
					transform.position -= transform.right * speed * Time.deltaTime;
			}
			if (player.transform.position.x > transform.position.x) {
					transform.position += transform.right * speed * Time.deltaTime;
			}
		}
		else
			speed = 1.0f;	//Reset speed back to start
		target = player.transform;
	}
}
