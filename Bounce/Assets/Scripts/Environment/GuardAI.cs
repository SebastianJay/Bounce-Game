using UnityEngine;
using System.Collections;

public class GuardAI : MonoBehaviour {

	public Transform target;
	public float speed;
	public float minRange;
	public float maxRange;
	public GameObject player;
	public float maxSpeed;
	public float visibleVelocity;

	void Start() {
		player = GameObject.FindGameObjectWithTag ("Player");
		target = player.transform;
		minRange = 1;
		maxRange = 25;
		speed = 2.5f;
		maxSpeed = 7.5f;
		visibleVelocity = 8.0f;
	}
	
	void FixedUpdate() {
		Debug.DrawLine (target.position, transform.position, Color.yellow);
		if (Mathf.Abs (transform.position.x - player.transform.position.x) < maxRange && Mathf.Abs (transform.position.x - player.transform.position.x) > minRange && (player.rigidbody2D.velocity.magnitude > visibleVelocity)) {
			//DO SOMETHING
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
		target = player.transform;
	}
}
