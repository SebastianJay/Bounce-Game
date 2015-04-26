using UnityEngine;
using System.Collections;

public class RotatingObject : MonoBehaviour {

	public float rotationalVelocity = 10f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(transform.position, new Vector3(0f, 0f, 1f), rotationalVelocity*Time.deltaTime);
	}
}
