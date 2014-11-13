using UnityEngine;
using System.Collections;

public class GuardAI : MonoBehaviour {

	public float speed = 2.5f;
	public float minRange = 1f;
	public float maxRange = 25f;
	public float visibleVelocity = 8.0f;

	private GameObject player;
	private Transform target;

	void Start() {
		player = GameObject.FindGameObjectWithTag ("Player");
		target = player.transform;
	}
	
	void FixedUpdate() {
		Debug.DrawLine (target.position, transform.position, Color.yellow);
		if (Mathf.Abs (transform.position.x - player.transform.position.x) < maxRange && 
		    Mathf.Abs (transform.position.x - player.transform.position.x) > minRange && 
		    (player.rigidbody2D.velocity.magnitude > visibleVelocity)) 
		{
			//DO SOMETHING
		}
		target = player.transform;
	}
}
