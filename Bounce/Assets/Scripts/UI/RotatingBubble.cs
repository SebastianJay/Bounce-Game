using UnityEngine;
using System.Collections;

public class RotatingBubble : MonoBehaviour {

	public float speed = 5f;
	void OnEnable() {
		//Debug.Log ("ENABLED");
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (transform.position, Vector3.up, Time.deltaTime * speed);
	}
}
