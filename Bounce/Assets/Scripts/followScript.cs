using UnityEngine;
using System.Collections;

public class followScript : MonoBehaviour {
	public Transform target;
	public float speed;
	public float minRange;
	public float maxRange;
	public GameObject player;




	void Start() {
		player = GameObject.FindGameObjectWithTag ("Player");
		target = player.transform;
		minRange = 3;
		maxRange = 15;
		speed = 3;
	}
	
	void FixedUpdate() {
		Debug.DrawLine (target.position, transform.position, Color.yellow);
		if (Mathf.Abs (transform.position.x - player.transform.position.x) < maxRange && Mathf.Abs (transform.position.x - player.transform.position.x) > minRange) {
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
